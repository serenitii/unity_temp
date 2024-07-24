using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//using UniRx;


namespace Jois
{
    public class AlertWindow : BaseWindow
    {
//        [SerializeField] protected bool _isPopUpWindow;
//        [SerializeField] protected bool _popOnForeground;
//        [SerializeField] protected bool _useBackButton;
//        [SerializeField] protected bool _isAlertWindow; 

        public enum AlertViewType
        {
            OneButtonStyle,
            TwoButtonStyle
        }

        [SerializeField] private AlertViewType _alertType;
        [SerializeField] private Text _title;
        [SerializeField] private Text _message;
        [SerializeField] private GameObject _oneButtonMode;
        [SerializeField] private GameObject _twoButtonMode;
        [SerializeField] private Button _okButton;
        [SerializeField] private Button _yesButton;
        [SerializeField] private Button _noButton;

        private Action _onOk;
        private Action _onYes;
        private Action _onNo;
        //private AlertViewManager _manager;

        // public AlertViewManager Manager
        // {
        //     set => _manager = value;
        // }

        public AlertViewType AlertType => _alertType;

        void Start()
        {
            // _isPopUpWindow = true;
            // _isAlertWindow = true;

            BindViewController();
        }

        void BindViewController()
        {
            if (_okButton != null)
            {
                _okButton.onClick.AddListener(() =>
                {
                   // _manager.OnAlertViewClosed(this);
                    
                    _onOk?.Invoke();
                    _onOk = null;
                });
            }

            if (_yesButton != null)
            {
                _yesButton.onClick.AddListener(() =>
                {
                   // _manager.OnAlertViewClosed(this);
                    
                    _onYes?.Invoke();
                    _onYes = null;
                });
            }

            if (_noButton != null)
            {
                _noButton.onClick.AddListener(() =>
                {
                  //  _manager.OnAlertViewClosed(this);
                    
                    _onNo?.Invoke();
                    _onNo = null;
                });
            }
        }

        public void ShowWindow(string title, string message)
        {
            //gameObject.SetActive(true);
            //UIManager.Current.OpenWindow(this);

            _title.text = title;
            _message.text = message;
        }

        public void SetMessage(string message, Action onOk)
        {
            _onOk = onOk;
            _title.gameObject.SetActive(false);
            _message.text = message;
        }

        public void SetMessage(string title, string msg, Action onYes, Action onNo)
        {
            _title.gameObject.SetActive(true);
            _title.text = title;
            _message.text = msg;
            _onYes = onYes;
            _onNo = onNo;
        }
    }
}