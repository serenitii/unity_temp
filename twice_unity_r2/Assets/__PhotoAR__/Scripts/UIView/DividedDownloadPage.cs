using System;
using System.Collections.Generic;
using System.Linq;
using Jois;
using UnityEngine;
using UnityEngine.UI;

namespace PhotoAr
{
    public class DividedDownloadPage : Jois.BaseWindow
    {
        [SerializeField] private Button _exitButton;

        [SerializeField] private PopupSerialNumberInput _popupSerialNumber;

        [SerializeField] private ArShopCardItem[] _items;

        private string _packageName;
        private List<string> _targetNames;

        private void OnEnable()
        {
            APP.MsgCenter.OnSerialNumberRequest += OnSerialNumberRequest;
        }

        private void OnDisable()
        {
            APP.MsgCenter.OnSerialNumberRequest -= OnSerialNumberRequest;
        }

        private void Start()
        {
            InitUIs();
        }

        void InitUIs()
        {
            _exitButton.onClick.AddListener(() =>
            {
                //
                UiManager.Current.PopWindow();
            });
        }


        public override void OnUIViewShown(object param)
        {
            if (param == null)
                return;

            _packageName = param as string;
            var package = string.IsNullOrEmpty(_packageName) ? null : APP.Data.Packages[_packageName];

            for (int i = 0; i < package.cards.Count; ++i)
            {
                _items[i].gameObject.SetActive(true);
                _items[i].SetData(package.name, package.cards[i].name, package.cards[i].playerName);
            }

            for (int i = package.cards.Count; i < _items.Length; ++i)
                _items[i].gameObject.SetActive(false);
        }

        void OnSerialNumberRequest(string targetName, string serialNumber)
        {
            Debug.LogFormat("target({0}) serialNumber({1}) ", targetName, serialNumber);

            // AR-Bundle 다운로드 

            // 동영상 다운로드 

            ProcessSerialNumberAsync(targetName, serialNumber);
        }

        public async void ProcessSerialNumberAsync(string targetName, string serialNumber)
        {
            // if (serialNumber == "x780518")
            // {
            //     foreach (var item in _items)
            //     {
            //         if (item.CardName == targetName)
            //         {
            //             item.DoDownloadCard(serialNumber);
            //             break;
            //         }
            //     }
            //
            //     return;
            // }
            //
            // return;
            
            var resUsable = await APP.Api.RequestSerialNumberUsable(serialNumber);

            if (serialNumber.Contains(Consts.MagicSN))
            {
                resUsable.serialNumberCode = 0;
            }
            
            switch (resUsable.serialNumberCode)
            {
                case 0:
                {
                    foreach (var item in _items)
                    {
                        if (item.CardName == targetName)
                        {
                            item.DoDownloadCard(serialNumber);
                            break;
                        }
                    }

                    break;
                }
                case 1:
                {
                    // 유효하지 serial number 팝업 뛰움
                    UiManager.Current.OpenWindow((int) PageViews.PopupBasic, Consts.CheckSerialNumber);
                    break;
                }
                case 2:
                {
                    // 횟수초과 
                    UiManager.Current.OpenWindow((int) PageViews.PopupBasic, Consts.TooManySerialNumber);
                    break;
                }
                case 3:
                {
                    // 횟수초과 
                    UiManager.Current.OpenWindow((int) PageViews.PopupBasic, Consts.TooManySerialNumber);
                    break;
                }
            }
        }
    }
}