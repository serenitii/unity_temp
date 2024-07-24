using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Jois;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace PhotoAr
{
    public class ArFilterPage : BaseWindow
    {
        [SerializeField] private Button _exitButton;
        [SerializeField] private Button _saveButton;
        [SerializeField] private ArFilterItem[] _filterItems;

        [SerializeField] private Text _titleText;

        private List<string> _tempEnables;

        private const int ReviewOnlyNumber = 1;
        
        void Start()
        {
            _exitButton.onClick.AddListener(() =>
            {
                // 
                // gameObject.SetActive(false);
                UiManager.Current.PopWindow();
            });

            _saveButton.onClick.AddListener(() =>
            {
                Debug.LogFormat("save enabled packages: [{0}] ", string.Join(", ", _tempEnables));
                APP.Data.SaveEnabledArPackages(_tempEnables);

                //SceneManager.LoadScene(1);

                UiManager.Current.ShowLoading(28001);

                DOVirtual.DelayedCall(0.2F,  () =>
                {
                    //
                    //gameObject.SetActive(false);
                    //SceneManager.UnloadSceneAsync(1);
                    SceneManager.UnloadScene(1);
                    SceneManager.LoadScene(1);
                });
            });

            // 1. 다운로드된 팩키지 리스트 

            // 2. 
        }

        public override void OnUIViewShown(object param)
        {
            var downloads = APP.Data.GetDownloadedPackages();
            var enabledPackages = APP.Data.EnabledArPackages;

            Debug.LogFormat("enabled packages: [{0}] ", string.Join(", ", enabledPackages));
            
            gameObject.SetActive(true);

            _tempEnables = new List<string>(enabledPackages);

            int count = 0;
            for (int i = 0; i < downloads.Count; ++i)
            {
                var packageName = enabledPackages.Find(x => x == downloads[i]);
                var package = APP.Data.Packages[downloads[i]];
                if (package.reviewOnly)
                    continue;
                Debug.LogFormat("item {0} ", count);
                _filterItems[count++].SetData(this, string.IsNullOrEmpty(packageName) == false, package);
            }

            for (int i = downloads.Count - 1; i < _filterItems.Length; ++i)
                _filterItems[i].gameObject.SetActive(false);

            UpdateTitle();
        }

        public void OnPackageEnable(bool isOn, string packageName)
        {
            if (isOn)
            {
                if (_tempEnables.Count >= Consts.MAXARPackages)
                {
                    var packageToDisable = _tempEnables[0];
                    foreach (var item in _filterItems)
                    {
                        if (item.PackageName == packageToDisable)
                        {
                            item.SetData(this, false, null);
                            break;
                        }
                    }

                    _tempEnables.RemoveAt(ReviewOnlyNumber); // reviewOnly 
                }

                _tempEnables.Add(packageName);
            }
            else
            {
                _tempEnables.Remove(packageName);
            }

            UpdateTitle();
        }

        void UpdateTitle()
        {
            _titleText.text = string.Format("스캔할 컨텐츠를 선택해 주세요 ({0}/{1})",
                _tempEnables.Count - ReviewOnlyNumber, Consts.MAXARPackages - ReviewOnlyNumber);
        }

        // void ReflectUI()
        // {
        //     List<string> enables = new List<string>();
        //     for (int i = 0; i < _filterItems.Length; ++i)
        //     {
        //         if (_filterItems[i].gameObject.activeInHierarchy && _filterItems[i].IsOn)
        //         {
        //             enables.Add(_filterItems[i].PackageName);
        //         }
        //     }
        // }
    }
}