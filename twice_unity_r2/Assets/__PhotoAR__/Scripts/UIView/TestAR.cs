using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation.Samples;

namespace PhotoAr
{
    public class TestAR : MonoBehaviour
    {
        [SerializeField] private Button _exitButton;
        [SerializeField] private Button[] _testButtons;
    
        IEnumerator Start()
        {
            yield return null;
            
            // APP.Data.InitializeRuntimeData();
            // yield return null;
            // APP.Data.DownloadPackage("twice_03", true);
            //
            // // yield return new WaitForSeconds(2F);
            
            _exitButton.onClick.AddListener(() =>
            {
                Application.Quit(0);
            });
        
            _testButtons[0].onClick.AddListener(() =>
            {
                // Download a Package
                APP.Data.DownloadPackage("twice_03", false);
            });
            
            _testButtons[1].onClick.AddListener(() =>
            {
                // Load AR images of the package
                // var list = APP.Data.PackagesInApp.GetDownloadedArImagePaths("twice_03");
                var list = new List<string>();
                var dynamicLibrary = GameObject.Find(Consts.ArSessionOriginPath).GetComponent<DynamicLibrary>();
                dynamicLibrary.AddArImages(list); 
            });
            
            _testButtons[2].onClick.AddListener(() =>
            {
                // Load AR images of the package
                var list = APP.Data.PackagesInApp.GetDownloadedArImagePaths("twice_03");
                Debug.LogFormat("GetDownloadedArImagePaths: {0} ", String.Join(", ", list));
                //var list = new List<string>();
                var dynamicLibrary = GameObject.Find(Consts.ArSessionOriginPath).GetComponent<DynamicLibrary>();
                dynamicLibrary.AddArImages(list); 
            });
        } 
    }
}