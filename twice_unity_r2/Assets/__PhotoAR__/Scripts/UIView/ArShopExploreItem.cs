using System;
using System.Collections;
using System.Collections.Generic;
using Jois;
using UnityEngine;
using UnityEngine.UI;

// TODO: 0715
// TODO: Explore 
// TODO: List 

namespace PhotoAr
{
    public class ArShopExploreItem : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private RawImage _packageRawImage;
        [SerializeField] private Text _titleText;
        [SerializeField] private Text _descText;

        private string _packageName;

        private void Start()
        {
            _button.onClick.AddListener(() =>
            {
                //
                UiManager.Current.OpenWindow((int) PageViews.ArShopPackageInfo, _packageName);
            });
        }

        public void SetData(PackageData package)
        {
            _packageName = package.name;
            _packageRawImage.color = Color.white;
            _packageRawImage.texture = APP.Data._packageImages[package.name];
            _titleText.text = package.title;
            _descText.text = ResizeDescText(package.exploreDesc);
        }

        private const int MAX_LEN = 36;

        string ResizeDescText(string desc)
        {
            var list = desc.Split('\n');

            for (int i = 0; i < list.Length; ++i)
            {
                if (list[i].Length > MAX_LEN)
                    list[i] = list[i].Substring(0, MAX_LEN);
            }

            return string.Join("\n", list);
        }
    }
}