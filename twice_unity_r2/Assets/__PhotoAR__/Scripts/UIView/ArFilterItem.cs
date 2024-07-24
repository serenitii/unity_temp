using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace PhotoAr
{
    public class ArFilterItem : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private GameObject _onGroup;
        [SerializeField] private GameObject _offGroup;

        [SerializeField] private Text _titleText;
        [SerializeField] private Text _artistText;
        [SerializeField] private RawImage _packageRawImage;

        private bool _isOn;

        private ArFilterPage _ownerPage;

        private string _packageName;

        void Start()
        {
            _button.onClick.AddListener(() =>
            {
                _isOn = !_isOn;
                SetData(null, _isOn, null);
                if (_ownerPage != null)
                    _ownerPage.OnPackageEnable(_isOn, _packageName);
            });
        }

        public bool IsOn => _isOn;
        public string PackageName => _packageName;

        public void SetData(ArFilterPage ownerPage, bool isOn, PackageData package) // string packageName)
        {
            _isOn = isOn;
            if (ownerPage != null)
                _ownerPage = ownerPage;
            if (package != null)
                _packageName = package.name;

            _onGroup.SetActive(_isOn);
            _offGroup.SetActive(_isOn == false);

            if (package == null)
                return;

            _titleText.text = package.title;
            _artistText.text = package.artist;
            _packageRawImage.color = Color.white;
            _packageRawImage.texture = APP.Data._packageImages[package.name];
        }
    }
}