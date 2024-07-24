using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using DG.Tweening;
using Jois;
using UnityEngine;

// using UnityEngine.AddressableAssets;
// using UnityEngine.ResourceManagement.AsyncOperations;
// using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.UI;

namespace PhotoAr
{
    public class ARPage : BaseWindow
    {
        [SerializeField] private Button _exitButton;
        [SerializeField] private Button _filterButton;

        //[SerializeField] private ArFilterPage _filterPage;

        [SerializeField] private Image _comboFlagImage;

        [SerializeField] Slider _downloadProgressBar;

        [SerializeField] private Button _testCacheButton;
        [SerializeField] private Button _clearButton;
        [SerializeField] private Button _testButton;
        [SerializeField] private Button _test2Button;

        [SerializeField] private Button _reloadARButton;

        [SerializeField] private GameObject _arCamera;
        [SerializeField] private GameObject _uiCamera;

        [SerializeField] private Text _noCardsText;
       
        [SerializeField] private Text _debugText;
        
        private const string CURRENT_ASSET = "apink_01";

        private void OnEnable()
        {
            APP.MsgCenter.OnDownloadVideoRequest += OnDownloadVideoRequest;
            APP.MsgCenter.OnComboStatusChanged += OnComboStatusChanged;
        }

        private void OnDisable()
        {
            APP.MsgCenter.OnDownloadVideoRequest -= OnDownloadVideoRequest;
            APP.MsgCenter.OnComboStatusChanged -= OnComboStatusChanged;
        }

        private void Start()
        {
            InitUIs();
            _downloadProgressBar.gameObject.SetActive(false);

            ArPlaceContainer.currentPlayingVideos = 0; // refactor_this
            
            // 1. shop 에서 다운로드 받은 팩키지 리스트를 가져온다 
            //List<string> downloads = APP.Data.GetDownloadedPackages();

            // 2. prefs 에서 활성화한 팩키지를 가져온다
            // 3. 활성화한 팩키지 번들을 인스턴스화 한다 

            _noCardsText.gameObject.SetActive(false);
            if (APP.Data.EnabledArPackages.Count == 0)
            {
                ShowNoCardsText();
            }
        }
        private void FixedUpdate()
        {
            string text0 = string.Format("videos({0}) \n", ArPlaceContainer.currentPlayingVideos);
            string text1 = ComboArObjManager.Current._debugText + "\n";
            _debugText.text = text0 + text1 + string.Join(", \n", TrackedImageManager.Current.GetActiveObjects());
        }
        
        private const float DELAY = 0.3f;
        async void ShowNoCardsText()
        {
            await Task.Delay(TimeSpan.FromSeconds(2F));
            _noCardsText.gameObject.SetActive(true);
            await Task.Delay(TimeSpan.FromSeconds(2F));
            _noCardsText.gameObject.SetActive(false);
            await Task.Delay(TimeSpan.FromSeconds(DELAY));
            _noCardsText.gameObject.SetActive(true);
            await Task.Delay(TimeSpan.FromSeconds(DELAY));
            _noCardsText.gameObject.SetActive(false);
            await Task.Delay(TimeSpan.FromSeconds(DELAY));
            _noCardsText.gameObject.SetActive(true);
            await Task.Delay(TimeSpan.FromSeconds(DELAY));
            _noCardsText.gameObject.SetActive(false);
            await Task.Delay(TimeSpan.FromSeconds(DELAY));
            _noCardsText.gameObject.SetActive(true);
            await Task.Delay(TimeSpan.FromSeconds(8F));
            _noCardsText.gameObject.SetActive(false);
        }

        void InitUIs()
        {
            _exitButton.onClick.AddListener(() =>
            {
                //UiManager.Current.PopWindow();
                UiManager.Current.LoadScene(0);
                //Application.Quit();
            });

            _filterButton.onClick.AddListener(() =>
            {
                //
                Debug.LogFormat("OpenWindow FilterPage");
                UiManager.Current.OpenWindow((int) ArViews.FilterPage);
            });

            return;

            _testCacheButton.onClick.AddListener(() =>
            {
                // var ret = Caching.ClearAllCachedVersions(CURRENT_ASSET);
                // Debug.LogFormat("Caching.ClearAllCachedVersions ({0}) ({1}) ", CURRENT_ASSET, ret);

                //ListCaches();
            });
            _clearButton.onClick.AddListener(() =>
            {
                Caching.ClearCache();
                Debug.LogFormat("Caching.ClearCache ");
            });
            _testButton.onClick.AddListener(() =>
            {
                //
                //LoadAllMarkerPrefabs();
            });
            _test2Button.onClick.AddListener(() =>
            {
                _arCamera.SetActive(true);
                //
                //TestProcess("iu_05");
                //TestProcess("twice_02");
                //TestProcess("TestAlbumPrefab");
                //TestProcess("twice_03");
                //TestProcess("iu_05_t");
            });

            _reloadARButton.onClick.AddListener(() =>
            {
                _uiCamera.SetActive(false);
                _arCamera.SetActive(false);

                DOVirtual.DelayedCall(1F, () =>
                {
                    _uiCamera.SetActive(false);
                    _arCamera.SetActive(true);
                });
            });
        }

        void OnDownloadVideoRequest(string packageName, string targetName, string uri)
        {
            _arCamera.SetActive(false);
            _uiCamera.SetActive(true);
            _downloadProgressBar.gameObject.SetActive(true);

            var fileName = targetName + ".mp4";
            APP.CacheFileMgr.DownloadFile(uri, packageName, fileName,
                progress =>
                {
                    Debug.LogFormat("progress ({0}) ", progress);
                    _downloadProgressBar.value = progress;
                },
                (isSuccess, savedPath) =>
                {
                    // 
                    Debug.LogFormat("isSuccess({0}) savedPath({1}) ", isSuccess, savedPath);

                    _uiCamera.SetActive(false);
                    _arCamera.SetActive(true);
                    DOVirtual.DelayedCall(0.2f, () =>
                    {
                        //
                        _downloadProgressBar.gameObject.SetActive(false);
                    });
                });
        }

        void OnComboStatusChanged(bool comboEnabled)
        {
            if (APP.Config.IsReleaseVersion == false)
                _comboFlagImage.gameObject.SetActive(comboEnabled);
        }

#if false
        public void LoadAllMarkerPrefabs()
        {
            string assetKey = "album_iu_05";

            APP.Logic.PrepareARMode();

            _downloadProgressBar.value = 0F;

            StartCoroutine(DownloadPrefab(assetKey,
                (percentage) => { _downloadProgressBar.value = percentage; },
                handle =>
                {
                    // _debugMsgText.text = string.Format("({0}) Download Completed !", assetKey);
                    // Debug.LogFormat(_debugMsgText.text);

                    // In a production environment, you should add exception handling to catch scenarios such as a null result.
                    var _myGameObject = handle.Result;
                    var newOne = Instantiate(_myGameObject);
                    newOne.transform.localPosition = Vector3.zero;
                    Debug.LogFormat("OnLoadDone ({0}) ", newOne.name);

                    DOVirtual.DelayedCall(1F, () =>
                    {
                        //
                        APP.Logic.SetARActive(true);
                    });
                }));
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


        void TestProcess(string assetKey)
        {
            //string assetKey = "iu_05";
            //string assetKey = "TestAlbumPrefab";

            _arCamera.SetActive(false);
            _downloadProgressBar.value = 0F;

            StartCoroutine(DownloadPrefab(assetKey,
                (percentage) => { _downloadProgressBar.value = percentage; },
                handle =>
                {
                    //_debugMsgText.text = string.Format("({0}) Download Completed !", assetKey);
                    Debug.LogFormat(string.Format("({0}) Download Completed !", assetKey));

                    // In a production environment, you should add exception handling to catch scenarios such as a null result.
                    var _myGameObject = handle.Result;
                    var newOne = Instantiate(_myGameObject);
                    newOne.transform.localPosition = Vector3.zero;
                    Debug.LogFormat("OnLoadDone ({0}) ", newOne.name);

                    DOVirtual.DelayedCall(1F, () =>
                    {
                        Debug.LogFormat("AR is ON ! ");
                        _arCamera.SetActive(true);
                    });
                }));
        }


//         public static void RemoveGameAsset(string gameID, Action onFinish)
//         {
//             #region Temporary solution for clearing game
//             //This solution assume the Caching.GetAllCachePaths will return a list that have first element is the path that addressable use
//             //If not pray and search on Unity forum for better solution;
//             Instance.StartCoroutine(Instance.DeleteGame(gameID, result =>
//             {
//                 List<string> allCachePath = new List<string>();
//                 Caching.GetAllCachePaths(allCachePath);
//                 string path = allCachePath[0];
//                 foreach (var bundle in (List<IAssetBundleResource>)result.Result)
//                 {
//                     try{
//                         Directory.Delete(path + "/" + bundle.GetAssetBundle().name, true);
//                     }
//                     catch(Exception e){
// // This to ignore the fact that _lockfile inside assetbundle cannot be delete
//                     }
//                 }
//                 onFinish?.Invoke();
//             }));
//  
//             #endregion
//         }

        void ListCaches()
        {
            List<string> allCachePath = new List<string>();
            Caching.GetAllCachePaths(allCachePath);
            string path = allCachePath[0];

            Debug.LogFormat("allCachePath ({0})", string.Join(", ", allCachePath));

//             foreach (var bundle in (List<IAssetBundleResource>)result.Result)
//             {
//                 try{
//                     Directory.Delete(path + "/" + bundle.GetAssetBundle().name, true);
//                 }
//                 catch(Exception e){
// // This to ignore the fact that _lockfile inside assetbundle cannot be delete
//                 }
//             }
        }
#endif
    }
}