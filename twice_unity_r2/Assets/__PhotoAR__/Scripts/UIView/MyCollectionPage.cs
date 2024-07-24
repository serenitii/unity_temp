using System.Collections;
using System.Collections.Generic;
using Jois;
using UnityEngine;
using UnityEngine.UI;

namespace PhotoAr
{
    public class MyCollectionPage : Jois.BaseWindow
    {
        [SerializeField] private Button _exitButton;

        [SerializeField] private List<MyCollectionItem> _items;

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
            base.OnUIViewShown(param);

            var downloads = CheckMyDownloads();

            for (int i = 0; i < downloads.Count; ++i)
            {
                _items[i].SetData(APP.Data.GetPackage(downloads[i]));
            }

            for (int i = downloads.Count; i < _items.Count; ++i)
                _items[i].gameObject.SetActive(false);
        }

        List<string> CheckMyDownloads()
        {
            var list = new List<string>();
            foreach (var packageName in APP.Data.PackagesInApp.AppData.shopPackages) 
            {
                var package = APP.Data.GetPackage(packageName);
                if (package.serialNumberMode == 2)
                {
                    foreach (var card in package.cards)
                    {
                        if (APP.Data.ExistsCardImageInLocal(package, card.name))
                        {
                            list.Add(packageName);
                            break;
                        }
                    }
                }
                else
                {
                    var downloadsCompleted = APP.Data.PackagesInApp.PackageDownloadCompleted(packageName);
                    if (downloadsCompleted)
                        list.Add(packageName);
                }
            }

            return list;
        }
    }
}