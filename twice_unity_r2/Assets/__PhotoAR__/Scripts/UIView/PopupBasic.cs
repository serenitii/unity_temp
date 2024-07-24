using System;
using System.Collections;
using System.Collections.Generic;
using Jois;
using UnityEngine;
using UnityEngine.UI;


namespace PhotoAr
{
    public class PopupBasic : BaseWindow
    {
        [SerializeField] private Text _message;
        [SerializeField] private Button _okButton;
        [SerializeField] private Button _cancelButton;

        private Action _yes;
        private Action _no;

        void Start()
        {
            _okButton.onClick.AddListener(() =>
            {
                if (_yes != null)
                {
                    _yes();
                    _yes = null;
                }

                UiManager.Current.PopWindow();
            });

            if (_cancelButton != null)
            {
                _cancelButton.onClick.AddListener(() =>
                {
                    if (_no != null)
                    {
                        _no();
                        _no = null;
                    }

                    UiManager.Current.PopWindow();
                });
            }
        }

        public override void OnUIViewShown(object param)
        {
            _message.text = param as string;
        }

        public void Init(string msg, Action onYes, Action onNo)
        {
            _message.text = msg;
            _yes = onYes;
            _no = onNo;
        }
    }
}