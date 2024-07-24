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
    public class ArShopCardItem : MonoBehaviour
    {
        [SerializeField] private Button _downloadButton;
        [SerializeField] private Button _deleteButton;

        [SerializeField] private Image _sliderImage;

        [SerializeField] private RawImage _cardRawImage;

        [SerializeField] private Text _playerNameText;

        private string _packageName;

        private string _cardName;

        private string _serialNumber;

        private string _playerName;

        public enum Status
        {
        }

        private bool _hasDownload;

        public string CardName => _cardName;

        void Start()
        {
            _downloadButton.onClick.AddListener(() =>
            {
                var exists = UFileIoTools.IsExistsInPersistent(_packageName, _cardName + Consts.VideoExt); // "." + _extension);

                if (exists == false)
                {
                    PopupBasic popup = UiManager.Current.OpenWindow((int) PageViews.PopupBasicYesNo) as PopupBasic;
                    if (popup != null)
                    {
                        popup.Init(string.Format(Consts.ConfirmDownload, _playerName),
                            () =>
                            {
                                ///
                                DOVirtual.DelayedCall(0.4f,
                                    () =>
                                    {
                                        //
                                        UiManager.Current.OpenWindow((int) PageViews.PopupSerialNumberInput, _cardName);
                                    });
                            }, () =>
                            {
                                ///
                                /// 
                            });
                    }
                }
            });
            _deleteButton.onClick.AddListener(() =>
            {
                DeleteCard(_cardName, () =>
                {
                    if (APP.Data.ExistsCardVideoMoreThanOne(_packageName) == false)
                    {
                        APP.Data.DeletePackage(_packageName, () =>
                        {
                            // 
                            APP.Data.OnPackageRemoved(_packageName);
                        });
                    }
                });

                DOVirtual.DelayedCall(0.5F, () =>
                {
                    _sliderImage.gameObject.SetActive(false);
                    _deleteButton.gameObject.SetActive(false);
                });
            });
        }

        public void SetData(string packageName, string cardName, string playerName)
        {
            _packageName = packageName;
            _cardName = cardName;
            _playerName = playerName;
            //_extension = APP.Data.GetPackage(packageName).cardImageExtension;

            var path = UFileIoTools.MakePersistentDataPath(packageName, cardName) + Consts.VideoExt;
            var fileExists = File.Exists(path);
            _deleteButton.gameObject.SetActive(fileExists);
            _sliderImage.gameObject.SetActive(fileExists);
            _sliderImage.fillAmount = fileExists ? 1F : 0F;

            _cardRawImage.color = Color.white;
            _cardRawImage.texture = APP.Data.CardTextures[cardName];

            if (string.IsNullOrEmpty(playerName) == false)
            {
                _playerNameText.gameObject.SetActive(true);
                _playerNameText.text = playerName;
            }
            else
                _playerNameText.gameObject.SetActive(false);
        }


        // 일련번호로 개별 카드 다운로드를 한다
        public void DoDownloadCard(string serialNumber)
        {
            // 1. 카드 동영상, AR 이미지 
            var list = APP.Data.PackagesInApp.GetDownloadUrlsInCards(_packageName, _cardName);

            // // 3. Combo 동영상 (remark_this)
            // APP.Data.GetDownloadsComboVideos(_packageName, _cardName, list, true);

            _sliderImage.gameObject.SetActive(true);
            _sliderImage.fillAmount = 0;

            UiManager.Current.ShowLoading(18004).SetTransparent();
            APP.CacheFileMgr.DownloadFiles(list, _packageName, progress =>
            {
                // 
                // Debug.LogFormat("progress({0}) ", progress);
                _sliderImage.fillAmount = progress;
                int percent = (int) (progress * 100F);
                //_percentageText.text = percent + "%";
            }, (success, path) =>
            {
                Debug.LogFormat("success({0}) path({1}) ", success, path);

                APP.Data.OnCardDownloaded(_packageName, _cardName, serialNumber);
                //APP.Data.MapCardToSerialNumber( serialNumber, _serialNumber);
                
                APP.Api.RequestLog(LogActionNames.SongDownload, string.Format("{0} {1} {2}", _packageName, _cardName, serialNumber));
                
                APP.Api.RequestSerialNumberUse(serialNumber, (isSuccess) => { });

                UiManager.Current.HideLoading(18004);
                DOVirtual.DelayedCall(0.1F, () =>
                {
                    //
                    // _radialProgress.SetActive(false);
                    // UpdateUI(packageName);

                    _deleteButton.gameObject.SetActive(true);
                });
            });
        }

        void DownloadPrerequisiteResources(string packageName)
        {
            var arBundleExists = UFileIoTools.IsExistsInPersistent(packageName, packageName);
            if (arBundleExists)
                return;
        }

        void DeleteCard(string cardName, Action onFinish)
        {
            var path = UFileIoTools.MakePersistentDataPath(_packageName, cardName) + Consts.VideoExt;
            if (File.Exists(path))
                File.Delete(path);

            var path2 = UFileIoTools.MakePersistentDataPath(_packageName, cardName) + Consts.LowVideoSuffix + Consts.VideoExt;
            if (File.Exists(path2))
                File.Delete(path2);

            onFinish?.Invoke();
        }
    }
}