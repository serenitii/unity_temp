using System.Collections;
using System.Collections.Generic;
using Jois;
using UnityEngine;
using UnityEngine.UI;

namespace PhotoAr
{
    public class ArShopPackageInfoPage : Jois.BaseWindow
    {
        [SerializeField] private Button _exitButton;
        [SerializeField] private RawImage _packageRawImage;
        [SerializeField] private Text _titleText;
        [SerializeField] private Text _exploreDescText;

        private void Start()
        {
            InitUIs();
        }

        void InitUIs()
        {
            _exitButton.onClick.AddListener(() => { UiManager.Current.PopWindow(); });
        }

        public override void OnUIViewShown(object param)
        {
            SetData(param as string);
        }

        void SetData(string packageName)
        {
            _packageRawImage.color = Color.white;
            _packageRawImage.texture = APP.Data.GetPackageTexture(packageName);
            var package = APP.Data.GetPackage(packageName);
            _titleText.text = package.title;
            _exploreDescText.text = package.exploreDesc;
        }
    }
}