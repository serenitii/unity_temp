using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.Networking;

namespace Jois
{
    /// <summary>Class <c>CacheFileManager</c>
    /// 1. Application.persistentDataPath 에 파일을 캐시용으로 저장하고 관리한다.
    /// 2. Application.persistentDataPath 에 AssetBundles를 저장하고 관리한다. 
    /// </summary>
    public class CacheFileManager : MonoBehaviour
    {
        private bool _cancelDownload;

        // isSuccess, savedPath
        public delegate void OnFinish(bool isSuccess, string savedPath);

        #region General File Caches

        public bool ExistsFileCache(string subDir, string fileName)
        {
            var path = UFileIoTools.MakePersistentDataPath(subDir, fileName);
            return System.IO.File.Exists(path);
        }

        public bool ExistsAllFilesCache(string subDir, List<string> fileNames)
        {
            foreach (var fileName in fileNames)
            {
                var path = UFileIoTools.MakePersistentDataPath(subDir, fileName);
                if (File.Exists(path) == false)
                    return false;
            }

            return true;
        }

        // 저장된 번들파일이 존재하면 full path, 없으면 empty
        public string GetCachedFilePath(string subDir, string fileName)
        {
            var path = UFileIoTools.MakePersistentDataPath(subDir, fileName);
            var ret = System.IO.File.Exists(path) ? path : String.Empty;
            if (string.IsNullOrEmpty(ret))
                Debug.LogWarningFormat("GetCachedFilePath ({0}) ({1}) No file found", subDir, fileName);
            return ret;
        }

        public void DownloadFile(string uri, string subDir, string fileName, Action<float> progressHandler, OnFinish onFinish)
        {
            StartCoroutine(DownloadFileImpl(uri, subDir, fileName, progressHandler, onFinish));
        }

        IEnumerator DownloadFileImpl(string uri, string subDir, string fileName, Action<float> progressHandler, OnFinish onFinish)
        {
            using (UnityWebRequest www = UnityWebRequest.Get(uri))
            {
                UnityWebRequestAsyncOperation request = www.SendWebRequest();

                while (!request.isDone)
                {
                    yield return null;

                    progressHandler?.Invoke(request.progress);

                    if (_cancelDownload)
                    {
                        _cancelDownload = false;
                        Debug.Log("Cancel: User Aborted");
                        //
                        www.Abort();

                        //CancelEnd();

                        yield break;
                    }

                    progressHandler?.Invoke(request.progress);
                }

                var path = UFileIoTools.MakePersistentDataPath(subDir, fileName);

                Debug.LogFormat("File Exists({0}) path({1}) ", System.IO.File.Exists(path), path);

                var bytes = www.downloadHandler.data;
                if (www.downloadHandler.data != null)
                {
                    System.IO.File.WriteAllBytes(path, bytes);

                    Debug.Assert(System.IO.File.Exists(path));
                }

                onFinish?.Invoke(true, path);
            }
        }

        public string GetFullPath(string subDir, string fileName)
        {
            return UFileIoTools.MakePersistentDataPath(subDir, fileName);
        }

        #endregion


        #region Asset Bundles

        // https://learn.unity.com/tutorial/introduction-to-asset-bundles
        // https://docs.unity3d.com/2021.2/Documentation/ScriptReference/Caching.html

        // 1. Cache 된 버전을 검사하여 없거나 낮으면 다운로드한다

        IEnumerator DownloadAndCacheAssetBundle(string uri, string manifestBundlePath)
        {
            //Load the manifest
            AssetBundle manifestBundle = AssetBundle.LoadFromFile(manifestBundlePath);
            AssetBundleManifest manifest = manifestBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");

            //Create new cache
            string today = DateTime.Today.ToLongDateString();
            Directory.CreateDirectory(today);
            Cache newCache = Caching.AddCache(today);

            //Set current cache for writing to the new cache if the cache is valid
            if (newCache.valid)
                Caching.currentCacheForWriting = newCache;

            //Download the bundle
            Hash128 hash = manifest.GetAssetBundleHash("bundleName");
            UnityWebRequest request = UnityWebRequestAssetBundle.GetAssetBundle(uri, hash, 0);
            yield return request.SendWebRequest();
            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(request);

            //Get all the cached versions
            List<Hash128> listOfCachedVersions = new List<Hash128>();
            Caching.GetCachedVersions(bundle.name, listOfCachedVersions);

            if (!AssetBundleContainsAssetIWantToLoad(bundle)) //Or any conditions you want to check on your new asset bundle
            {
                //If our criteria wasn't met, we can remove the new cache and revert back to the most recent one
                Caching.currentCacheForWriting = Caching.GetCacheAt(Caching.cacheCount);
                Caching.RemoveCache(newCache);

                for (int i = listOfCachedVersions.Count - 1; i > 0; i--)
                {
                    //Load a different bundle from a different cache
                    request = UnityWebRequestAssetBundle.GetAssetBundle(uri, listOfCachedVersions[i], 0);
                    yield return request.SendWebRequest();
                    bundle = DownloadHandlerAssetBundle.GetContent(request);

                    //Check and see if the newly loaded bundle from the cache meets your criteria
                    if (AssetBundleContainsAssetIWantToLoad(bundle))
                        break;
                }
            }
            else
            {
                //This is if we only want to keep 5 local caches at any time
                if (Caching.cacheCount > 5)
                    Caching.RemoveCache(Caching.GetCacheAt(1)); //Removes the oldest user created cache
            }
        }

        bool AssetBundleContainsAssetIWantToLoad(AssetBundle bundle)
        {
            return (bundle.LoadAsset<GameObject>("MyAsset") != null); //this could be any conditional
        }

        #endregion

        public void DownloadFiles(List<string> fileURIs, string subDir, Action<float> progressHandler, OnFinish onFinish)
        {
            UFileIoTools.CreateDirectoryInPersistent(subDir);

            StartCoroutine(DownloadFilesCo(fileURIs, subDir, progressHandler, onFinish));
        }

        private float _progress;
        private float _progressDivision;
        private float _counter;

        IEnumerator DownloadFilesCo(List<string> fileURIs, string subDir, Action<float> progressHandler, OnFinish onFinish)
        {
            // 1. AssetBundle 다운로드 
            // 2. 동영상 다운로드 
            _progress = 0F;
            _progressDivision = 1F / fileURIs.Count;
            _counter = 0F;
            for (var i = 0; i < fileURIs.Count; ++i)
            {
                int count = -1;
                string uri = fileURIs[i];
                string fileName = Path.GetFileName(uri);
                yield return StartCoroutine(DownloadFileImpl(uri, subDir, fileName, progress =>
                {
                    if (++count % 20 == 0)
                    {
                        _progress = _counter * _progressDivision + _progressDivision * progress;
                        progressHandler(_progress);

                        Debug.LogFormat("progress -> ({0}) ", _progress);
                    }
                }, null));

                _counter += 1F;
                _progress = i == fileURIs.Count - 1 ? 1F : _progressDivision * _counter;

                progressHandler(_progress);
            }

            onFinish(true, "");
        }

        // void HandleProgress(float progress)
        // {
        //     _progress = _counter * _progressDivision + _progressDivision * progress;
        //     progressHandler(_progress); 
        //     Debug.LogFormat("progress -> ({0}) ", _progress);
        // }
    }
}