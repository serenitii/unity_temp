using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

using UnityEngine.UI;

#if false
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
namespace PhotoAr
{
    public class DevPage : MonoBehaviour
    {
        [SerializeField] private Button _exitButton;
        [SerializeField] private Button _clearCacheButton;
        [SerializeField] private Button _downloadAssetAButton;
        [SerializeField] private Button _downloadAssetBButton;
        [SerializeField] private Slider _downloadProgressBar;

        [SerializeField] private Button _tempButton;

        [SerializeField] private string _assetKey;

        [SerializeField] private Text _debugMsgText;


        [SerializeField] private GameObject _arCamera;

        private void Start()
        {
            _exitButton.onClick.AddListener(() => { Application.Quit(); });

            _clearCacheButton.onClick.AddListener(() =>
            {
                // Addressables.ClearDependencyCacheAsync(key);
                Caching.ClearCache();
            });

            _downloadAssetAButton.onClick.AddListener(() =>
            {
                _downloadProgressBar.value = 0F;

                StartCoroutine(DownloadPrefab(_assetKey,
                    (percentage) =>
                    {
                        _downloadProgressBar.value = percentage;
                    },
                    handle =>
                    {
                        _debugMsgText.text = string.Format("({0}) Download Completed !", _assetKey);
                        Debug.LogFormat(_debugMsgText.text);

                        // In a production environment, you should add exception handling to catch scenarios such as a null result.
                        var _myGameObject = handle.Result;
                        var newOne = Instantiate(_myGameObject);
                        newOne.transform.localPosition = Vector3.zero;
                        Debug.LogFormat("OnLoadDone ({0}) ", newOne.name);
                    }));
            });

            _downloadAssetBButton.onClick.AddListener(() =>
            {
                string assetKey = "album_iu_05";
                //string assetKey = "TestAlbumPrefab";

                _arCamera.SetActive(false);
                _downloadProgressBar.value = 0F;

                StartCoroutine(DownloadPrefab(assetKey,
                    (percentage) =>
                    {
                        _downloadProgressBar.value = percentage;
                    },
                    handle =>
                    {
                        _debugMsgText.text = string.Format("({0}) Download Completed !", assetKey);
                        Debug.LogFormat(_debugMsgText.text);

                        // In a production environment, you should add exception handling to catch scenarios such as a null result.
                        var _myGameObject = handle.Result;
                        var newOne = Instantiate(_myGameObject);
                        newOne.transform.localPosition = Vector3.zero;
                        Debug.LogFormat("OnLoadDone ({0}) ", newOne.name);

                        DOVirtual.DelayedCall(1F, () => _arCamera.SetActive(true));
                    }));
            });

            _tempButton.onClick.AddListener(() =>
            {
                CheckDownloadSize(_assetKey,
                    hasDownload => { _debugMsgText.text = string.Format("({0}) has been downloaded? ({1}) ", _assetKey, hasDownload); });
            });
        }

        void CheckDownloadSize(string key, System.Action<bool> onComplete)
        {
            Addressables.GetDownloadSizeAsync(key).Completed += handle =>
            {
                var hasDownloaded = false == (handle.Status == AsyncOperationStatus.Succeeded && handle.Result > 0);
                Debug.LogFormat("GetDownloadSizeAsync result({0}) ", handle);
                onComplete(hasDownloaded);
            };
        }

        IEnumerator DownloadPrefab(string key, Action<float> onProgress, Action<AsyncOperationHandle<GameObject>> onDownloadCompleted)
        {
            //Load a GameObject
            AsyncOperationHandle<GameObject> goHandle = Addressables.LoadAssetAsync<GameObject>(key);

            Debug.LogFormat("LoadAssetAsync on START status({0}) DownloadStatus({1}) ",
                goHandle.Status, goHandle.GetDownloadStatus().DownloadedBytes);
            goHandle.Completed += onDownloadCompleted;

            //goHandle.
            while (!goHandle.IsDone)
            {
                var status = goHandle.GetDownloadStatus();
                float progress = status.Percent;
                int percentage = (int) (progress * 100);
                Debug.LogFormat("Percent: ({0}) ", percentage);
                onProgress?.Invoke(progress);
                yield return null;
            }

            Debug.LogFormat("Download Complete ! ");

            onProgress?.Invoke(1F);

            //Addressables.LoadAssetAsync<GameObject>(key).Completed += OnLoadDone;
        }
    }
}
#endif