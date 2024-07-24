using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Jois;
using UnityEngine;
using UnityEngine.UI;

namespace PhotoAr
{
    public class ArShopPage : Jois.BaseWindow
    {
        [SerializeField] private Button _exitButton;

        [SerializeField] private List<ArShopExploreItem> _exploreItems; // = new List<ArShopExploreItem>();

        [SerializeField] private List<ArShopPackageItem> _listItems; // = new List<ArShopAlbumItem>(16);

        [SerializeField] private InputField _searchInput;

        [SerializeField] private RectTransform _exploreList;
        [SerializeField] private RectTransform _packageList;
        [SerializeField] private RectTransform _exploreListIPadRef;
        [SerializeField] private RectTransform _packageListIPadRef; 
        
        private bool _isIPadRatio;

        private void Start()
        {
            InitUIs();
        }

        void InitUIs()
        {
            var currentRatio = 768f / 1024f; // Screen.currentResolution.width / Screen.currentResolution.height;
            _isIPadRatio = 1080f / 1920f < currentRatio;
            //if (1080f / 1920f < currentRatio)
            //   if (SystemInfo.deviceModel.Contains("iPad"))
            if (_isIPadRatio)
            {
               // _searchInput.gameObject.SetActive(false);
                
               // UguiTools.MakeFullStretch();
                //var rect = _packageList.rect;
               // rect.Set(0,0,);
               //_packageList.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 10f, 100f);  //= _packageListIPadRef.rect;
               //_packageList.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 400f);
               //_packageList.
               
               
               //_packageList.sizeDelta = new Vector2(0f, -1000f);

               // var rt = _packageList;
               // rt.localScale = Vector3.one;
               // rt.anchorMin = Vector2.zero;
               // rt.anchorMax = Vector2.one;
               // rt.sizeDelta = Vector2.zero;
               // rt.anchoredPosition = Vector2.zero;
               //
               // Debug.LogFormat("> pivot({0}) rect({1}) anchoredPosition({2}) sizeDelta({3}) anchorMin({4}) anchorMax({5}) ", 
               //     rt.pivot, rt.rect, rt.anchoredPosition, rt.sizeDelta, rt.anchorMin, rt.anchorMax);
            }

            _exitButton.onClick.AddListener(() => { UiManager.Current.PopWindow(); });

            _searchInput.onValueChanged.AddListener(val =>
            {
                // 
                FilterSearchWords(val);
            });
        }

        private void FilterSearchWords(string words)
        {
            if (words.Length < 2)
            {
                // remark_this (temp comments)
                // SetData(APP.Data.AppData);
            }
            else
            {
                words = words.ToLower();

                foreach (var item in _listItems)
                {
                    if (item.gameObject.activeInHierarchy)
                    {
                        var artist = item.ArtistText.ToLower();
                        var title = item.TitleText.ToLower();
                        var hit = artist.Contains(words) || title.Contains(words);
                        item.gameObject.SetActive(hit);
                    }
                }
            }
        }

        public override void OnUIViewShown(object param)
        {
            //base.OnUIViewShown(param);

            SetData(APP.Data.PackagesInApp.AppData);
        }

        GameObject CreateListItemPrefab(PackageData package)
        {
            return null;
        }

        void CreateList()
        {
            foreach (var album in APP.Data.PackagesInApp.AppData.packages)
            {
                CreateListItemPrefab(album);
            }
        }

        void ReserveItems(int exploreCount, int listCount)
        {
            if (_exploreItems == null)
                _exploreItems = new List<ArShopExploreItem>();
            // if (_listItems == null)
            //     _listItems = new List<ArShopPackageItem>(16);

            for (int i = 0; i < _exploreItems.Count; ++i)
            {
                _exploreItems[i].gameObject.SetActive(i < exploreCount);
            }

            // for (int i = 0; i < _listItems.Count; ++i)
            // {
            //     _listItems[i].gameObject.SetActive(i < listCount);
            // }
        }

        void SetData(AppData data)
        {
            var exploreItemNames = data.explorePackages;
            var listItems = data.shopPackages;
            var packages = APP.Data.Packages; // data.packages;

            if (exploreItemNames?.Any() == true)
            {
            }

            ReserveItems(exploreItemNames.Count, listItems.Count);

            // 1. Explore 
            for (int i = 0; i < data.explorePackages.Count; ++i)
            {
                var packageName = data.explorePackages[i];
                var package = packages[packageName];
                _exploreItems[i].SetData(package);
            }

            for (int i = data.shopPackages.Count; i < _exploreItems.Count; ++i)
                _exploreItems[i].gameObject.SetActive(false);

            // 2. List 
            for (int i = 0; i < data.shopPackages.Count; ++i)
            {
                var packageName = data.shopPackages[i];
                var package = packages[packageName];
                _listItems[i].SetData(package);
            }

            for (int i = data.shopPackages.Count; i < _listItems.Count; ++i)
                _listItems[i].gameObject.SetActive(false);
        }
    }
}