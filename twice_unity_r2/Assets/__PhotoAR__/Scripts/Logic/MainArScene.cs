using System;
using System.Collections;
using System.Collections.Generic;
using Jois;
using UnityEngine;
using UnityEngine.XR.ARFoundation.Samples;

namespace PhotoAr
{
    public class MainArScene : MonoBehaviour
    {
        private IEnumerator Start()
        {
            APP.Data.LoadEnabledPackages();

            // 0. 이미지 다운로드
#if false //true
            var packages = new[] { "twice_03" };
            // APP.Data.InitializeRuntimeData();
            yield return null;
            Debug.LogFormat("MainArScene.Start PackageDownloadCompleted ({0}) ", APP.Data.PackagesInApp.PackageDownloadCompleted(packages[0]));
            // if (APP.Data.PackagesInApp.PackageDownloadCompleted(packages[0]))
            // {
            // }
            // else
            // {
            //     APP.Data.DownloadPackage(packages[0], true);
            //     yield return new WaitForSeconds(5F);
            //     Debug.Assert(APP.Data.PackagesInApp.PackageDownloadCompleted(packages[0]));
            // }
#else
            yield return null;
            var packages = APP.Data.EnabledArPackages;
#endif
            Debug.LogFormat("> Packages to load: ( {0} ) ", string.Join(", ", packages));

            // 1. 이미지 추가
            // Load AR images of the package
            // var list = APP.Data.PackagesInApp.GetDownloadedArImagePaths(packages[0]);
            var paths = new List<string>(32);
            foreach (var package in packages)
            {
                paths.AddRange(APP.Data.PackagesInApp.GetDownloadedArImagePaths(package));
            }
#if false
            // !!! iOS 심사용 
            if (APP.Config.iOSReviewEnabled())
            {
                if (packages.Contains(Consts.ReviewSamplePackage) == false)
                {
                    string demoArImagePath = APP.CacheFileMgr.GetCachedFilePath(Consts.ReviewSamplePackage, "twice_02_01_sq.png");
                    paths.Add(demoArImagePath);
                }
            }
#endif

            // Debug.LogFormat("GetDownloadedArImagePaths: count({0}) {1} ", paths.Count, string.Join(", ", paths));
            var dynamicLibrary = GameObject.Find(Consts.ArSessionOriginPath).GetComponent<DynamicLibrary>();
            dynamicLibrary.AddArImages(paths);


            // 2. AR 인스턴스 생성
            dynamicLibrary.AdaptImages();

            UiManager.Current.OpenWindow((int)ArViews.ArPage);

            yield return null;
        }
    }
}