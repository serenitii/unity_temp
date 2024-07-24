using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Jois;
using UnityEngine;
using UnityEngine.UI;

namespace PhotoAr
{
    public class PopupSerialNumberInput : BaseWindow
    {
        [SerializeField] private InputField _inputField;
        [SerializeField] private Button _button;

        private string _targetName;
        
        void Start()
        {
            _button.onClick.AddListener(() =>
            {
                UiManager.Current.PopWindow();
                
                var serialNumber =  _inputField.text.ToUpper();
                if (string.IsNullOrEmpty(serialNumber) == false)
                {
                    if (APP.Data.SerialNumberCardMap.ContainsKey(serialNumber) && 
                        serialNumber.Contains(Consts.MagicSN) == false)
                    {
                        if (APP.Data.SerialNumberCardMap[serialNumber] != _targetName)
                        {
                            DOVirtual.DelayedCall(0.4f,
                                () =>
                                {
                                    //
                                    UiManager.Current.OpenWindow((int) PageViews.PopupBasic, Consts.AlreadyUsing);
                                });
                            return;
                        }
                    }
                    
                    APP.MsgCenter.OnSerialNumberRequest(_targetName, serialNumber);
                }
                
                _inputField.text = "";
            });
        }

        public override void OnUIViewShown(object param)
        {
            _targetName = param as string;
            _inputField.text = "";
        }
    }
}