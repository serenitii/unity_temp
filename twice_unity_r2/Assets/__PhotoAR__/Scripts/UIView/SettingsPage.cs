using System.Collections;
using System.Collections.Generic;
using Jois;
using UnityEngine;
using UnityEngine.UI;

namespace PhotoAr
{
    public class SettingsPage : Jois.BaseWindow
    {
        [SerializeField] private Button _exitButton;
        [SerializeField] private Button _termsOfUseButton;

        [SerializeField] private Toggle _toggleHighQuality;
        [SerializeField] private Text _versionText;

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

            _termsOfUseButton.onClick.AddListener(() =>
            {
                //
                UiManager.Current.OpenWindow((int) PageViews.TermsOfUse);
            });

            _toggleHighQuality.onValueChanged.AddListener(isOn =>
            {
                APP.Data.SetLowQualityVideoActive(isOn == false);
                Debug.LogFormat("UseHasSetLowQualityVideo ({0}) ", APP.Data.UseHasSetLowQualityVideo);
            });
        }

        public override void OnUIViewShown(object param)
        {
            _toggleHighQuality.isOn = APP.Data.UseHasSetLowQualityVideo == false;
            // _versionText.text = string.Format("{0} ({1})", Application.version, APP.Data.PackagesInApp.HeaderData.version); 
            var isLocalOrServer = APP.Config.UseLocalDataJson ? "L" : "S";
            _versionText.text = string.Format("{0} ({1}-{2}-A)",
                Application.version, APP.Data.PackagesInApp.HeaderData.version, isLocalOrServer);
        }
    }
}