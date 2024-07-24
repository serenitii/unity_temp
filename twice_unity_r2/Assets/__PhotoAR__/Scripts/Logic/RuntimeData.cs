using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Jois;
using UnityEngine;
using Cysharp.Threading.Tasks;

namespace PhotoAr
{
    public class RuntimeData : MonoBehaviour
    {
        [SerializeField] private PackagesInApp _packagesInApp;

        [SerializeField] private TextAsset _arHeaderJson;
        [SerializeField] private TextAsset _arDataJson;

        private Dictionary<string, PackageData> _packages;
        private Dictionary<string, CardData> _cards;

        public Dictionary<string, PackageData> Packages => _packages;

        public AppHeaderData AppHeader { get; set; }

        public PackageData GetPackage(string packageName)
        {
            return _packages[packageName];
        }

        public PackagesInApp PackagesInApp => _packagesInApp;
        
        // public async UniTask<int> InitializeRuntimeDataAsync()
        // {
        //     Debug.LogFormat("Runtime.InitializeRuntimeData ");
        //
        //     if (APP.Config.UseLocalDataJson)
        //     {
        //         // var header = JsonUtility.FromJson<AppHeaderData>(_arHeaderJson.text);
        //         LoadAppDataJson(_arHeaderJson.text, _arDataJson.text);
        //     }
        //     else
        //     {
        //         var localVersion = PlayerPrefs.GetInt(Consts.PrefKey_AppDataVersion);
        //
        //         var headerJson = await UnityWebReqTools.RequestGetAsync(APP.Config.AppHeaderUrl); // Consts.appHeaderDataUrl);
        //         Debug.LogFormat("[Req] result: {0} ", headerJson);
        //         AppHeader = JsonUtility.FromJson<AppHeaderData>(headerJson);
        //         var shouldUpdate = localVersion < AppHeader.version;
        //         if (shouldUpdate)
        //         {
        //             APP.CacheFileMgr.DownloadFile(APP.Data.AppHeader.content, "", Consts.AppDataFileName, f => { },
        //                 (bool isSuccess, string savedPath) =>
        //                 {
        //                     Debug.LogFormat("! New ar_app_data.json downloaded !");
        //                     var dataJson = File.ReadAllText(savedPath);
        //                     LoadAppDataJson(headerJson, dataJson);
        //                 });
        //         }
        //         else
        //         {
        //             var dataJsonPath = APP.CacheFileMgr.GetCachedFilePath("", Consts.AppDataFileName);
        //             var dataJson = File.ReadAllText(dataJsonPath);
        //             LoadAppDataJson(headerJson, dataJson); 
        //         }
        //     }
        //     return 0;
        // }

        public void LoadAppDataJsonLocal()
        {
            // var header = JsonUtility.FromJson<AppHeaderData>(_arHeaderJson.text);
            LoadAppDataJson(_arHeaderJson.text, _arDataJson.text);
        }

        public void LoadAppDataJson(string headerJson, string dataJson)
        {
            var header = JsonUtility.FromJson<AppHeaderData>(headerJson);
            var bodyData = JsonUtility.FromJson<AppData>(dataJson);
            // Debug.LogFormat("LoadLocalDataFile: {0} ", JsonUtility.ToJson(bodyData));
            
            PlayerPrefs.SetInt(Consts.PrefKey_AppDataVersion, header.version);
            _packagesInApp.SetData(header, bodyData);
            ProcessDataStructure();
            ReadPrefs();
        }

        public void DownloadPackage(string packageName, bool passIfExists)
        {
            if (passIfExists)
            {
                if (_packagesInApp.PackageDownloadCompleted(packageName))
                {
                    Debug.LogFormat("Package ({0}) has already downloaded. so we have pass ", packageName);
                    return;
                }
            }

            // var package = _packagesInApp.GetPackage(packageName);

            // https://cocoa-dev-test.s3.ap-northeast-2.amazonaws.com/twice_r/packages/twice_03/twice_03_01.jpg

            var list = _packagesInApp.GetDownloadUrlsInPackage(packageName);

            Debug.LogFormat("download list: {0} ", String.Join(", ", list));

            APP.CacheFileMgr.DownloadFiles(list, packageName,
                progress =>
                {
                    // 
                    // Debug.LogFormat("progress({0}) ", progress);
                },
                (success, path) =>
                {
                    // 
                    Debug.LogFormat("success({0}) path({1}) ", success, path);
                });
        }

        // void LoadLocalDataFile()
        // {
        //     var header = JsonUtility.FromJson<AppHeaderData>(_arHeaderJson.text);
        //     var bodyData = JsonUtility.FromJson<AppData>(_arDataJson.text);
        //     // Debug.LogFormat("LoadLocalDataFile: {0} ", JsonUtility.ToJson(bodyData));
        //     _packagesInApp.SetData(header, bodyData);
        // }
        //
        // void LoadRemoteDataFileAsync()
        // {
        // }

        public CardData GetCardByName(string cardName)
        {
            Debug.AssertFormat(_cards.ContainsKey(cardName), "NO card({0}) has found ", cardName);
            return _cards[cardName];
        }

        // Raw Data (_packagesInApp) 를 가공한다
        void ProcessDataStructure()
        {
            var appData = _packagesInApp.AppData;

            _packages = new Dictionary<string, PackageData>(32);
            _cards = new Dictionary<string, CardData>(64);

            foreach (var package in appData.packages)
            {
                _packages.Add(package.name, package);

                package.videoShape.scale = package.videoShape.scale < 0.1F ? 1F : package.videoShape.scale;
                var shape = package.videoShape;

                foreach (var card in package.cards)
                {
                    card.packageName = package.name;
                    foreach (var combo in package.combos)
                    {
                        if (combo.cards.Contains(card.name))
                        {
                            card.comboEnabled = true;
                            break;
                        }
                    }

                    if (card.videoShape == null)
                    {
                        card.videoShape = new VideoShape(package.videoShape);
                        continue;
                    }

                    card.videoShape.shape = card.videoShape.shape == 0 ? shape.shape : card.videoShape.shape;
                    card.videoShape.shapeOptions = card.videoShape.shapeOptions == 0
                        ? shape.shapeOptions
                        : card.videoShape.shapeOptions;
                    card.videoShape.scale = card.videoShape.scale == 0 ? shape.scale : card.videoShape.scale;
                    card.videoShape.offsetX = card.videoShape.offsetX == 0 ? shape.offsetX : card.videoShape.offsetX;
                    card.videoShape.offsetY = card.videoShape.offsetY == 0 ? shape.offsetY : card.videoShape.offsetY;
                    card.videoShape.width = card.videoShape.width == 0 ? shape.width : card.videoShape.width;
                    card.videoShape.height = card.videoShape.height == 0 ? shape.height : card.videoShape.height;

                    _cards.Add(card.name, card);
                }
                
                // combo (갯수가 많은 것부터 검사하기 위해!)
                if (package.combos is { Count: > 0 })
                    package.combos.Reverse();
            }
        }

        #region Runtime Textures

        public Dictionary<string, Texture2D> _packageImages = new Dictionary<string, Texture2D>(16);
        public List<Texture2D> _tempImages = new List<Texture2D>();

        public Dictionary<string, Texture2D> _cardTextures = new Dictionary<string, Texture2D>(32);
        public Dictionary<string, Texture2D> CardTextures => _cardTextures;

        public void LoadCardTexture(string packageName, string cardName)
        {
            if (_cardTextures.ContainsKey(cardName))
                return;

            var package = _packages[packageName];
            var fileName = cardName + "." + package.cardImageExtension;
            if (APP.CacheFileMgr.ExistsFileCache(packageName, fileName))
            {
                var path = APP.CacheFileMgr.GetFullPath(packageName, fileName);
                byte[] bytesTexture = File.ReadAllBytes(path);
                if (bytesTexture.Length > 0)
                {
                    var texture = new Texture2D(0, 0);
                    texture.LoadImage(bytesTexture);
                    _cardTextures.Add(cardName, texture);
                    _tempImages.Add(texture);
                }
            }
        }

        public Texture GetPackageTexture(string packageName)
        {
            return _packageImages[packageName];
        }

        public void DownloadPackageImages()
        {
            List<string> files = new List<string>(16);

            // 1. 팩키지 이미지 (AR Shop의 list 에 있는 것만 받는다)
            foreach (var packageName in _packagesInApp.AppData.shopPackages)
            {
                var pack = _packagesInApp.GetPackage(packageName);

                if (string.IsNullOrEmpty(pack.packageImage))
                    continue;

                var fileName = Path.GetFileName(pack.packageImage);
                if (APP.CacheFileMgr.ExistsFileCache(null, fileName) == false)
                {
                    var url = _packagesInApp.AppData.storageUrl + "/" + pack.name + "/" + pack.packageImage;
                    files.Add(url);
                }
            }

            if (files.Count > 0)
            {
                UiManager.Current.ShowLoading(Consts.LOADING_LOAD_PACKAGE_IMAGES);

                APP.CacheFileMgr.DownloadFiles(files, null, progress =>
                {
                    // 
                    //
                }, (success, path) =>
                {
                    // 
                    Debug.LogFormat("");
                    LoadPackageTextures();

                    UiManager.Current.HideLoading(Consts.LOADING_LOAD_PACKAGE_IMAGES);
                    
                    DownloadPackageReviewOnly("review_only_01");
                });
                return;
            }

            LoadPackageTextures();
            DownloadPackageReviewOnly("review_only_01");
        }

        void LoadPackageTextures()
        {
            foreach (var packageName in _packagesInApp.AppData.shopPackages)
            {
                var pack = _packagesInApp.GetPackage(packageName);

                if (string.IsNullOrEmpty(pack.packageImage))
                    continue;

                var fileName = Path.GetFileName(pack.packageImage);
                if (APP.CacheFileMgr.ExistsFileCache(null, fileName))
                {
                    if (_packageImages.ContainsKey(pack.name))
                        continue;

                    var path = APP.CacheFileMgr.GetFullPath(null, pack.packageImage);
                    byte[] bytesTexture = File.ReadAllBytes(path);
                    if (bytesTexture.Length > 0)
                    {
                        var texture = new Texture2D(0, 0);
                        texture.LoadImage(bytesTexture);
                        _packageImages.Add(pack.name, texture);
                        _tempImages.Add(texture);
                    }
                }
            }
        }

        //
        void DownloadPackageReviewOnly(string packageName)
        {
            if (ExistsFilesInPackage(packageName))
                return;
            
            var list = APP.Data.PackagesInApp.GetDownloadUrlsInPackage(packageName);

            UiManager.Current.ShowLoading(18003); //.SetTransparent();
            APP.CacheFileMgr.DownloadFiles(list, packageName, progress =>
            {
                // 
                // Debug.LogFormat("progress({0}) ", progress);
                // _slider.fillAmount = progress;
                // int percent = (int) (progress * 100F);
                // _percentageText.text = percent + "%";
            }, (success, path) =>
            {
                Debug.LogFormat("success({0}) path({1}) ", success, path);

                APP.Data.OnPackageDownloaded(packageName);

                UiManager.Current.HideLoading(18003);

                // DOVirtual.DelayedCall(0.1F, () =>
                // {
                //     //
                //     _radialProgress.SetActive(false);
                //     UpdateUI(packageName);
                // });
            });

            // 2. 
        }
        #endregion

        #region Utils

        public bool ExistsCardImageInLocal(PackageData package, string cardName)
        {
            var fileName = cardName + "." + package.cardImageExtension;
            return APP.CacheFileMgr.ExistsFileCache(package.name, fileName);
        }

        public bool ExistsCardVideoInLocal(PackageData package, string cardName)
        {
            var fileName = cardName + Consts.VideoExt;
            return APP.CacheFileMgr.ExistsFileCache(package.name, fileName);
        }

        public bool ExistsCardVideoMoreThanOne(string packageName)
        {
            var package = GetPackage(packageName);
            foreach (var card in package.cards)
            {
                if (ExistsCardVideoInLocal(package, card.name))
                    return true;
            }

            return false;
        }

        public List<string> GetDownloadedPackages()
        {
            var downloads = new List<string>();

            foreach (var package in _packages)
            {
                if (ExistsFilesInPackage(package.Key))
                    downloads.Add(package.Key);
            }

            return downloads;
        }

        // 팩키지 내의 모든 파일은 다운받았는지 여부
        public bool ExistsFilesInPackage(string packageName)
        {
            var list = _packagesInApp.GetFileNamesInPackage(packageName);
            foreach (var fileName in list)
            {
                var path = UFileIoTools.MakePersistentDataPath(packageName, fileName);
                if (File.Exists(path))
                    return true;
            }

            return false;
        }

        #endregion

        #region 클래스 분리 (LocalStorage)

        private bool _useHasSetLowQualityVideo;

        public bool UseHasSetLowQualityVideo => _useHasSetLowQualityVideo;

        public void SetLowQualityVideoActive(bool active)
        {
            _useHasSetLowQualityVideo = active;

            PlayerPrefs.SetInt(Consts.PrefKey_LowQuality, active ? 1 : 0);
        }



        public void DeletePackage(string packageName, Action onFinish)
        {
            var path = Application.persistentDataPath + "/" + packageName;
            if (Directory.Exists(path))
                Directory.Delete(path, true);

            onFinish?.Invoke();
            
            APP.Api.RequestLog(LogActionNames.PackageDelete, packageName);
        }

        public void OnPackageDownloaded(string packageName)
        {
            Debug.AssertFormat(string.IsNullOrEmpty(packageName) == false, "OnPackageDownloaded packageName is null ");

            AddEnabledPackage(packageName);
        }
        
        // refactor_this
        public void OnPackageRemoved(string packageName)
        {
            RemoveEnabledPackage(packageName);
        }

        public void ReadPrefs()
        {
            LoadEnabledPackages();
            
            if (PlayerPrefs.HasKey(Consts.PrefKey_LowQuality))
            {
                var isLow = PlayerPrefs.GetInt(Consts.PrefKey_LowQuality);
                _useHasSetLowQualityVideo = isLow == 1;

                Debug.LogFormat("Prefs: LowQuality ({0}) ", _useHasSetLowQualityVideo);
            }

            {
                var str = PlayerPrefs.GetString(Consts.PrefKey_SerialNumberCardMap);
                _serialNumberCardMap = StringTools.StringToDictionary(str);
            }
        }

        #endregion

        #region 클래스 분리??? 시리얼 넘버

        private Dictionary<string, string> _serialNumberCardMap = new Dictionary<string, string>(16);
        public Dictionary<string, string> SerialNumberCardMap => _serialNumberCardMap;

        public void MapCardToSerialNumber(string serialNumber, string cardName)
        {
            _serialNumberCardMap.Add(serialNumber, cardName);

            var stringValue = StringTools.DictionaryToString(_serialNumberCardMap);
            Debug.LogFormat("MapCardToSerialNumber: ({0})", stringValue);

            PlayerPrefs.SetString(Consts.PrefKey_SerialNumberCardMap, stringValue);
        }

        #endregion


        #region Card Download In the AR Shop

        private List<string> _downloadedCards = new();

        public void OnCardDownloaded(string packageName, string cardName, string serialNumber)
        {
            if (_enabledArPackages.Count >= Consts.MAXARPackages && _enabledArPackages.Contains(packageName) == false)
                _enabledArPackages.RemoveAt(0);

            if (_enabledArPackages.Contains(packageName) == false)
                _enabledArPackages.Add(packageName);

            if (_downloadedCards.Contains(packageName) == false)
                _downloadedCards.Add(cardName);

            PlayerPrefs.SetString(Consts.PrefKey_EnabledArPackages, String.Join(" ", _enabledArPackages));

            // 
            MapCardToSerialNumber(serialNumber, cardName);
        }



        #endregion

        #region ETC

        // 개별 다운로드를 하는 팩키지에 진입 UI 에 진입했을 때.
        public void DownloadIndividualCardImages(string packageName, Action onFinish)
        {
            List<string> files = new List<string>(16);

            var package = _packages[packageName];
            if (package.serialNumberMode != 2)
                return;

            // 1. 모든 이미지가 다운로드된 상태가 아니면 처음부터 다 받는다.
            // 2. remark_this (temp comments)

            foreach (var card in package.cards)
            {
                var fileName = Path.GetFileName(card.name + "." + package.cardImageExtension);
                if (APP.CacheFileMgr.ExistsFileCache(package.name, fileName) == false)
                {
                    var url = _packagesInApp.AppData.storageUrl + "/" + packageName + "/" + card.name + "." + package.cardImageExtension; // package.storagePath + Consts.CardImageDirectory + fileName;
                    files.Add(url);
                    // var url2 = _packagesInApp.AppData.storageUrl + "/" + packageName + "/" + card.name + "_sq." + package.cardImageExtension; // package.storagePath + Consts.CardImageDirectory + fileName;
                    // files.Add(url2);
                }
            }
            
            if (files.Count > 0)
            {
                UiManager.Current.ShowLoading(18002);
                APP.CacheFileMgr.DownloadFiles(files, packageName, progress =>
                {
                    // 
                    //
                }, (success, path) =>
                {
                    // 
                    Debug.LogFormat("pre - LoadCardTextures");
                    //LoadPackageTextures();
                    // LoadCardTextures(packageName);

                    LoadCardTextures(package);
            
                    UiManager.Current.HideLoading(18002);
                    onFinish();
                });
                return;
            }
            
            LoadCardTextures(package);
            onFinish();
        }

        void LoadCardTextures(PackageData package)
        {
            foreach (var card in package.cards)
            {
                if (_cardTextures.ContainsKey(card.name) == false)
                {
                    LoadCardTexture(package.name, card.name);
                }
                // if (_cardTextures.ContainsKey(card.name + "_sq") == false)
                // {
                //     LoadCardTexture(package.name, card.name + "_sq");
                // }
            }
        }

        #endregion

        #region Enabled Packages

        private List<string> _enabledArPackages = new();
        public List<string> EnabledArPackages => _enabledArPackages;

        public void SaveEnabledArPackages(List<string> enables)
        {
            _enabledArPackages = new List<string>(enables);
            var str = String.Join(" ", _enabledArPackages);
            PlayerPrefs.SetString(Consts.PrefKey_EnabledArPackages, str);
            Debug.LogFormat("[Prefs]: Loaded Enabled Ar Packages count({0}), [{1}] ", _enabledArPackages.Count, str);
        }
        
        public void LoadEnabledPackages()
        {
            if (PlayerPrefs.HasKey(Consts.PrefKey_EnabledArPackages))
            {
                var str = PlayerPrefs.GetString(Consts.PrefKey_EnabledArPackages);

                _enabledArPackages = string.IsNullOrEmpty(str) ? new List<string>() : str.Split(' ').ToList();

                Debug.LogFormat("[Prefs]: Loaded Enabled Ar Packages count({0}), [{1}] ", _enabledArPackages.Count, str);
            }
            else
            {
                _enabledArPackages = new List<string>();
                Debug.LogFormat("[Prefs]: Loaded  NO Enabled Ar Packages ");
            }
        }

        void AddEnabledPackage(string packageName)
        {
            if (_enabledArPackages.Count >= Consts.MAXARPackages)
                _enabledArPackages.RemoveAt(Consts.FirstArContentIndex);
            _enabledArPackages.Add(packageName);

            var str = string.Join(" ", _enabledArPackages);
            PlayerPrefs.SetString(Consts.PrefKey_EnabledArPackages, str);
            Debug.LogFormat("[Prefs]: Added({0}) Enabled Ar Packages count({1}) [{2}] ", packageName, _enabledArPackages.Count, str);
        }

        void RemoveEnabledPackage(string packageName)
        {
            if (string.IsNullOrEmpty(packageName))
            {
                _enabledArPackages.RemoveAt(0);
            }
            else
            {
                var item = _enabledArPackages.Find(x => x == packageName);
                _enabledArPackages.Remove(item);
            }

            var str = string.Join(" ", _enabledArPackages);
            PlayerPrefs.SetString(Consts.PrefKey_EnabledArPackages, str);
            Debug.LogFormat("[Prefs]: Removed({0}) Enabled Ar Packages count({1}) [{2}] ", packageName, _enabledArPackages.Count, str);
        }

        #endregion
    }
}