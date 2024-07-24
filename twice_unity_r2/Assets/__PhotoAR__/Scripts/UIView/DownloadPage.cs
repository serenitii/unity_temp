using System.Collections;
using System.Collections.Generic;
using Jois;
using UnityEngine;
using UnityEngine.UI;

namespace PhotoAr
{
    public class DownloadPage : Jois.BaseWindow
    {
        [SerializeField] private Button _exitButton;

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
            
            
        }
    }
}
