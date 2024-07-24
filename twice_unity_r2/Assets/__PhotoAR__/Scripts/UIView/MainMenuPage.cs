using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Jois;
using UnityEngine;
using UnityEngine.UI;

namespace PhotoAr
{
    public class MainMenuPage : Jois.BaseWindow
    {
        [SerializeField] private Button _exitButton;

        [SerializeField] private Button _settingsButton;

        // [SerializeField] private Button _arShopButton;
        // [SerializeField] private Button _arButton;
        // [SerializeField] private Button _myCollectionButton;
        // [SerializeField] private Button _beyondLiveButton;
        [SerializeField] private Button[] _buttonsX3;
        [SerializeField] private Button[] _buttonsX4;

        private bool _firstTimeLoad;

        static private bool _firstTimeRun;
        
        private void Start()
        {
            InitData();

            //InitFirstTime();
            // // 1. 설정 데이터 받음 (remote) 
            // if (_firstTimeLoad == false)
            // {
            //     _firstTimeLoad = true;
            //     // APP.Data.InitializeRuntimeData();
            // }
            //
            // // 2. Read Prefs
            //
            //
            // InitUIs();
            //
            // // 3. Download Package Images
            // APP.Data.DownloadPackageImages();
        }

        async UniTask<int> InitData()
        {
            if (APP.Config.UseLocalDataJson)
            {
                if (_firstTimeLoad == false)
                {
                    _firstTimeLoad = true;
                    APP.Data.LoadAppDataJsonLocal();
                }

                if (_firstTimeRun == false)
                {
                    _firstTimeRun = true;
                    Debug.LogFormat("App Run ! ");
                    APP.Api.RequestLog(LogActionNames.AppExec, "");
                }
                
                InitUIs();
                APP.Data.DownloadPackageImages();
            }
            else
            {
                var localVersion = PlayerPrefs.GetInt(Consts.PrefKey_AppDataVersion);

                var headerJson = await UnityWebReqTools.RequestGetAsync(APP.Config.AppHeaderUrl); // Consts.appHeaderDataUrl);
                Debug.LogFormat("[Req] result: {0} ", headerJson);
                var headerData = JsonUtility.FromJson<AppHeaderData>(headerJson);
                APP.Data.AppHeader = headerData;
                var shouldUpdate = localVersion < headerData.version;
                
                if (_firstTimeRun == false)
                {
                    _firstTimeRun = true;
                    Debug.LogFormat("App Run ! ");
                    APP.Api.RequestLog(LogActionNames.AppExec, "");
                }
                
                if (shouldUpdate)
                {
                    APP.Api.RequestLog(LogActionNames.UpdateJson, headerData.version.ToString());
                    
                    APP.CacheFileMgr.DownloadFile(APP.Data.AppHeader.content, "", Consts.AppDataFileName, f => { },
                        (bool isSuccess, string savedPath) =>
                        {
                            var dataJson = File.ReadAllText(savedPath);

                            APP.Data.LoadAppDataJson(headerJson, dataJson);
                            InitUIs();
                            APP.Data.DownloadPackageImages();
                        });
                }
                else
                {
                    var dataJsonPath = APP.CacheFileMgr.GetCachedFilePath("", Consts.AppDataFileName);
                    var dataJson = File.ReadAllText(dataJsonPath);

                    if (_firstTimeLoad == false)
                    {
                        _firstTimeLoad = true;
                        
                        APP.Data.LoadAppDataJson(headerJson, dataJson);
                    }

                    InitUIs();
                    APP.Data.DownloadPackageImages();
                }
            }

            return 0;
        }

        // async void InitFirstTime()
        // {
        //     // 1. 설정 데이터 받음 (remote) 
        //     if (_firstTimeLoad == false)
        //     {
        //         _firstTimeLoad = true;
        //         await APP.Data.InitializeRuntimeDataAsync();
        //     }
        //
        //     InitUIs();
        //     // 3. Download Package Images
        //     APP.Data.DownloadPackageImages();
        // }

        void InitUIs()
        {
            _exitButton.onClick.AddListener(() => { Application.Quit(); });

            _settingsButton.onClick.AddListener(() => { UiManager.Current.OpenWindow((int)PageViews.Settings); });

            // AR Shop
            _buttonsX3[0].onClick.AddListener(() =>
            {
                Debug.LogFormat("To AR Shop");
                if (APP.Config.IsIPadRatio())
                    UiManager.Current.OpenWindow((int)PageViews.ArShopIPad);
                else
                {
                    UiManager.Current.OpenWindow((int)PageViews.ArShop);
                }
            });
            _buttonsX4[0].onClick.AddListener(() =>
            {
                if (APP.Config.IsIPadRatio())
                    UiManager.Current.OpenWindow((int)PageViews.ArShopIPad);
                else
                    UiManager.Current.OpenWindow((int)PageViews.ArShop);
            });

            // AR 
            _buttonsX3[1].onClick.AddListener(() =>
            {
                UiManager.Current.ShowLoading(18005);
                if (APP.Config.iOSReviewEnabled())
                {
                    DownloadDemoCard_iOSReview();
                }
                else
                    DOVirtual.DelayedCall(0.1F, () => { UiManager.Current.LoadScene(Consts.SCENE_AR); });
            });
            _buttonsX4[1].onClick.AddListener(() =>
            {
                UiManager.Current.ShowLoading(18005);
                if (APP.Config.iOSReviewEnabled())
                {
                    DownloadDemoCard_iOSReview();
                }
                else
                    DOVirtual.DelayedCall(0.1F, () => { UiManager.Current.LoadScene(Consts.SCENE_AR); });
            });

            // My Collection
            _buttonsX3[2].onClick.AddListener(() => { UiManager.Current.OpenWindow((int)PageViews.MyCollection); });
            _buttonsX4[3].onClick.AddListener(() => { UiManager.Current.OpenWindow((int)PageViews.MyCollection); });

            // WebView (Beyond Live)
            _buttonsX4[2].onClick.AddListener(() => { UiManager.Current.OpenWindow((int)PageViews.BeyondLive); });

            var hasWebView = string.IsNullOrEmpty(APP.Data.PackagesInApp.AppData.appSettings.webViewUrl) == false;
            _buttonsX3[0].transform.parent.gameObject.SetActive(hasWebView == false);
            _buttonsX4[0].transform.parent.gameObject.SetActive(hasWebView);
        }

        public override void OnUIViewShown(object param)
        {
            // 카메라 권한 허용을 처음부터 물어보기 위함
            if (GameObject.Find(Consts.ArSessionOriginPath))
            {
                GameObject.Find(Consts.ArSessionOriginPath).SetActive(false);
                GameObject.Find(Consts.ArSessionPath).SetActive(false);
            }
        }

        // 애플 심사용 기본 카드를 다운받는다
        void DownloadDemoCard_iOSReview()
        {
            List<string> fileNames = new() { "twice_02_01.png", "twice_02_01_sq.png", "twice_02_01.mp4" };
            
            var packageUrl = APP.Data.PackagesInApp.AppData.storageUrl + "/twice_02/";
            List<string> urls = new List<string>
            {
                packageUrl + fileNames[0],
                packageUrl + fileNames[1],
                packageUrl + fileNames[2]
            };

            if (APP.CacheFileMgr.ExistsAllFilesCache(Consts.ReviewSamplePackage, fileNames))
            {
                DOVirtual.DelayedCall(0.1F, () => { UiManager.Current.LoadScene(Consts.SCENE_AR); }); 
            }
            else
            {
                APP.CacheFileMgr.DownloadFiles(urls, Consts.ReviewSamplePackage, progress =>
                {
                    // 
                    // 
                }, (success, path) =>
                {
                    //
                    UiManager.Current.LoadScene(Consts.SCENE_AR);
                });
            }
        }
    }
}