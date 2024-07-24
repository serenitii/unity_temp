using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using Jois;
using UnityEngine;
using UnityEngine.UI;

namespace PhotoAr
{
    public class ArShopPackageItem : MonoBehaviour
    {
        private Button _button;

        [SerializeField] private Text _titleText;
        [SerializeField] private Text _artistText;

        [SerializeField] private Button _downloadButton;
        [SerializeField] private Button _dividedDownloadButton;
        [SerializeField] private Button _deleteButton;

        [SerializeField] private GameObject _radialProgress;
        [SerializeField] private Image _slider;
        [SerializeField] private Text _percentageText;

        [SerializeField] private RawImage _packageRawImage;

        private string _packageName;

        void Start()
        {
            InitUIs();
        }

        void InitUIs()
        {
            _radialProgress.SetActive(false);

            // _slider.fillAmount = 0.6F;
            // _percentageText.text = "22%";

            _button = GetComponent<Button>();
            _button.onClick.AddListener(() =>
            {
                //
                UiManager.Current.OpenWindow((int) PageViews.ArShopPackageInfo);
            });

            _dividedDownloadButton.onClick.AddListener(() =>
            {
                //
                Debug.LogFormat("_dividedDownloadButton packageName({0}) ", _packageName);

                APP.Data.DownloadIndividualCardImages(_packageName,
                    () =>
                    {
                        Debug.LogFormat("pre - OpenWindow((int) PageViews.DividedDownload");
                        UiManager.Current.OpenWindow((int) PageViews.DividedDownload, _packageName);
                    });
            });

            _downloadButton.onClick.AddListener(() =>
            {
                Debug.LogFormat("DownloadPackage ");

                DownloadPackage(_packageName);
            });

            _deleteButton.onClick.AddListener(() =>
            {
                APP.Data.DeletePackage(_packageName, () =>
                {
                    // 
                    APP.Data.OnPackageRemoved(_packageName);
                });

                DOVirtual.DelayedCall(0.2F, () =>
                {
                    //
                    UpdateUI(_packageName);
                });
            });
        }

        public string TitleText => _titleText.text;
        public string ArtistText => _artistText.text;

        public void SetData(PackageData package)
        {
            gameObject.SetActive(true);

            _packageName = package.name;

            _titleText.text = package.title;
            _artistText.text = package.artist;
            if (_packageRawImage != null)
            {
                _packageRawImage.color = Color.white;
                _packageRawImage.texture = APP.Data._packageImages[_packageName];
            }

            UpdateUI(_packageName);
        }

        void UpdateUI(string packageName)
        {
            SerialNumberMode serialNumberMode =
                (SerialNumberMode) APP.Data.GetPackage(packageName).serialNumberMode;

            bool hasDeleteButton = false;

            switch (serialNumberMode)
            {
                case SerialNumberMode.None:
                {
                    hasDeleteButton = APP.Data.PackagesInApp.PackageDownloadCompleted(packageName);
                    _downloadButton.gameObject.SetActive(hasDeleteButton == false);
                    _dividedDownloadButton.gameObject.SetActive(false);
                    _deleteButton.gameObject.SetActive(hasDeleteButton);
                    break;
                }
                case SerialNumberMode.Package:
                {
                    hasDeleteButton = APP.Data.PackagesInApp.PackageDownloadCompleted(packageName);
                    _downloadButton.gameObject.SetActive(hasDeleteButton == false);
                    _dividedDownloadButton.gameObject.SetActive(false);
                    _deleteButton.gameObject.SetActive(hasDeleteButton);
                    break;
                }
                case SerialNumberMode.Card:
                {
                    hasDeleteButton = APP.Data.ExistsFilesInPackage(packageName);
                    _downloadButton.gameObject.SetActive(false);
                    _dividedDownloadButton.gameObject.SetActive(true);
                    _deleteButton.gameObject.SetActive(false);
                    break;
                }
            }
        }

        void DownloadPackage(string packageName)
        {
            _downloadButton.gameObject.SetActive(false);
            _radialProgress.SetActive(true);
            _slider.fillAmount = 0;
            _percentageText.text = "0%";

            APP.Api.RequestLog(LogActionNames.PackageDownload, packageName);
            
            // 1. 
            // APP.Data.PackageData
            var list = APP.Data.PackagesInApp.GetDownloadUrlsInPackage(packageName);

            UiManager.Current.ShowLoading(18003); //.SetTransparent();
            APP.CacheFileMgr.DownloadFiles(list, packageName, progress =>
            {
                // 
                // Debug.LogFormat("progress({0}) ", progress);
                _slider.fillAmount = progress;
                int percent = (int) (progress * 100F);
                _percentageText.text = percent + "%";
            }, (success, path) =>
            {
                Debug.LogFormat("success({0}) path({1}) ", success, path);

                APP.Data.OnPackageDownloaded(packageName);

                UiManager.Current.HideLoading(18003);

                DOVirtual.DelayedCall(0.1F, () =>
                {
                    //
                    _radialProgress.SetActive(false);
                    UpdateUI(packageName);
                });
            });

            // 2. 
        }

    }
}