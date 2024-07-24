using System.Collections;
using System.Collections.Generic;
using Jois;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace PhotoAr
{
    public class TermsOfUsePage : Jois.BaseWindow
    {
        [SerializeField] private Button _exitButton;
        [SerializeField] private Toggle _toggle; 
        
        private void Start()
        {
            InitUIs();
        }

        void InitUIs()
        {
            _exitButton.onClick.AddListener(() =>
            {
                //
                if (PlayerPrefs.HasKey(Consts.PrefKey_TermsOfUseDone) == false)
                    Application.Quit();
                else 
                    UiManager.Current.PopWindow();
            });
            
            _toggle.onValueChanged.AddListener(isOn =>
            {
                if (isOn)
                {
                    PlayerPrefs.SetInt(Consts.PrefKey_TermsOfUseDone, 1);
                    SceneManager.LoadScene(0);
                    
                    APP.Api.RequestLog(LogActionNames.FirstExec, "");
                }
            });
            
            if (PlayerPrefs.HasKey(Consts.PrefKey_TermsOfUseDone))
                _toggle.gameObject.SetActive(false);
        }
    }
}
