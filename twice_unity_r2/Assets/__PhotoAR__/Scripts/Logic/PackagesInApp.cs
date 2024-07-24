using System;
using System.Collections.Generic;
using System.IO;
using Jois;
using UnityEngine;

namespace PhotoAr
{
    public class PackagesInApp : MonoBehaviour
    {
        private AppHeaderData _header;
        private AppData _appData;

        private Dictionary<string, PackageData> _packages;

        private const string VideoExt = ".mp4";

        public AppHeaderData HeaderData => _header;
        public AppData AppData => _appData;

        public void SetData(AppHeaderData header, AppData data)
        {
            _header = header;
            _appData = data;
            APP.Api.SetServerAddress(header.server);
            _packages = new Dictionary<string, PackageData>();
            foreach (var package in _appData.packages)
            {
                _packages.Add(package.name, package);
            }

            Debug.LogFormat("loaded app data ver({0}) ", _header.version);
        }

        public PackageData GetPackage(string packageName)
        {
            Debug.AssertFormat(_packages.ContainsKey(packageName), "({0}) could not find ", packageName);
            return _packages[packageName];
        }

        // string GetArCardImageUrl(string packageName, string cardName)
        // {
        //     var package = _packages[packageName];
        //     return _appData.storageUrl + "/" + packageName + "/" + cardName + "_sq." + package.cardImageExtension;
        // }
        //
        // string GetCardVideoUrl(string packageName, string cardName)
        // {
        //     var package = _packages[packageName];
        //     return _appData.storageUrl + "/" + packageName + "/" + cardName + VideoExt;
        // }
        //
        // string GetCardImageUrl(string packageName, string cardName)
        // {
        //     var package = _packages[packageName];
        //     return _appData.storageUrl + "/" + packageName + "/" + cardName + "." + package.cardImageExtension;
        // }

        public string GetCachedVideoPathForPlay(string subDir, string targetName, bool isLowQuality)
        {
            var fileName = isLowQuality ? targetName + Consts.LowVideoSuffix + Consts.VideoExt : targetName + Consts.VideoExt;
            var videoFileFullPath = UFileIoTools.MakePersistentDataPath(subDir, fileName);
            return System.IO.File.Exists(videoFileFullPath) ? videoFileFullPath : string.Empty;
        }

        public bool PackageDownloadCompleted(String packageName)
        {
            var package = _packages[packageName];
            foreach (var card in package.cards)
            {
                var arImageFile = card.name + Consts.SquaredArImageSuffixPoint + package.arImageExtension;
                if (APP.CacheFileMgr.ExistsFileCache(packageName, arImageFile) == false)
                    return false;
                var videoFile = card.name + VideoExt;
                if (APP.CacheFileMgr.ExistsFileCache(packageName, videoFile) == false)
                    return false;
                var cardImageFile = card.name + "." + package.cardImageExtension;
                if (APP.CacheFileMgr.ExistsFileCache(packageName, cardImageFile) == false)
                    return false;
            }

            foreach (var combo in package.combos)
            {
                if (APP.CacheFileMgr.ExistsFileCache(packageName, combo.file + VideoExt) == false)
                    return false;
            }
            
            return true;
        }

        // 팩키지 하나의 모든 다운로드 링크를 받는다 
        public List<string> GetDownloadUrlsInPackage(string packageName)
        {
            var package = _packages[packageName];
            var list = new List<string>(64);

            foreach (var card in package.cards)
            {
                // 1. AR Card Images (1024 x 1024 로 리사이즈된 AR Image)
                // 2. Videos 
                // 3. Card Images (My Collection 에서 사용)
                // 4. Low Videos
                var arImageUrl = _appData.storageUrl + "/" + packageName + "/" + card.name + Consts.SquaredArImageSuffixPoint +
                                 package.arImageExtension;
                var videoUrl = _appData.storageUrl + "/" + packageName + "/" + card.name + VideoExt;
                var cardImageUrl = _appData.storageUrl + "/" + packageName + "/" + card.name + "." + package.cardImageExtension;
                list.Add(arImageUrl);
                list.Add(videoUrl);
                list.Add(cardImageUrl);
                if (package.hasLowVideos)
                {
                    var lowVideoUrl = _appData.storageUrl + "/" + packageName + "/" + card.name + "_low" + VideoExt;
                    list.Add(lowVideoUrl);
                }
            }

            if (package.combos != null && package.combos.Count > 0)
            {
                foreach (var combo in package.combos)
                {
                    var videoUrl = _appData.storageUrl + "/" + packageName + "/" + combo.file + VideoExt;
                    list.Add(videoUrl);
                }
            }

            return list;
        }

        // 팩키지에 포함된 모든 파일의 이름 
        public List<string> GetFileNamesInPackage(string packageName)
        {
            var package = _packages[packageName];
            var list = new List<string>(64);

            foreach (var card in package.cards)
            {
                // 1. AR Card Images (1024 x 1024 로 리사이즈된 AR Image)
                // 2. Videos 
                // 3. Card Images (My Collection 에서 사용)
                // 4. Low Videos
                var arImageUrl = card.name + Consts.SquaredArImageSuffixPoint + package.arImageExtension;
                var videoUrl = card.name + VideoExt;
                var cardImageUrl = card.name + "." + package.cardImageExtension;
                list.Add(arImageUrl);
                list.Add(videoUrl);
                list.Add(cardImageUrl);
                if (package.hasLowVideos)
                {
                    var lowVideoUrl = card.name + "_low" + VideoExt;
                    list.Add(lowVideoUrl);
                }
            }

            return list;
        }

        // 개별 카드의 다운로드 링크를 얻는다
        public List<string> GetDownloadUrlsInCards(string packageName, string cardName)
        {
            var package = _packages[packageName];
            var list = new List<string>(64);
            var packageStoragePath = _appData.storageUrl + "/" + packageName;
            if (package.combos is {Count: > 0})
            {
                foreach (var combo in package.combos)
                {
                    
                    if (combo.cards.Contains(cardName))
                        list.Add(packageStoragePath + "/" + combo.file + VideoExt);
                }
            }
            
            foreach (var card in package.cards)
            {
                // 1. AR Card Images (1024 x 1024 로 리사이즈된 AR Image)
                // 2. Videos 
                // 4. Low Videos
                if (card.name == cardName)
                {
                    var arImageUrl = packageStoragePath + "/" + card.name + Consts.SquaredArImageSuffixPoint + package.arImageExtension;
                    var videoUrl = packageStoragePath + "/" + card.name + VideoExt;
                    var cardImageUrl = packageStoragePath + "/" + card.name + "." + package.cardImageExtension;
                    list.Add(arImageUrl);
                    list.Add(videoUrl);
                    if (package.hasLowVideos)
                    {
                        var lowVideoUrl = _appData.storageUrl + "/" + packageName + "/" + card.name + "_low" + VideoExt;
                        list.Add(lowVideoUrl);
                    }
                    break;
                }
            }



            return list;
        }

        public List<string> GetDownloadedArImagePaths(string packageName)
        {
            var list = new List<string>(16);
            var package = _packages[packageName];
            foreach (var card in package.cards)
            {
                var path = APP.CacheFileMgr.GetCachedFilePath(packageName,
                    card.name + Consts.SquaredArImageSuffixPoint + package.arImageExtension);
                if (package.serialNumberMode == 2)
                {
                    if (File.Exists(path))
                    {
                        list.Add(path); 
                    }
                }
                else
                {
                    list.Add(path);
                }
            }

            return list;
        }
    }
}