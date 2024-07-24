using System.Collections;
using System.Collections.Generic;
using Jois;
using UnityEngine;
using UnityEngine.UI;

namespace PhotoAr
{
    public class MyCollectionItem : MonoBehaviour
    {
        private Button _button;
        [SerializeField] private RawImage _packageRawImage;
        [SerializeField] private Text _artistText;
        [SerializeField] private Text _expoloreDescText;
        private string _packageName;
        void Start()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(() =>
            {
                //
                UiManager.Current.OpenWindow((int) PageViews.MyCollectionDetail, _packageName);
            });
        }

        public void SetData(PackageData package) 
        {
            gameObject.SetActive(true);
            _packageName = package.name;
            _packageRawImage.color = Color.white;
            _packageRawImage.texture = APP.Data._packageImages[package.name];
            //_artistText.text = package.artist;
            _artistText.gameObject.SetActive(false);
            _expoloreDescText.text = package.exploreDesc;
        }
    }
}