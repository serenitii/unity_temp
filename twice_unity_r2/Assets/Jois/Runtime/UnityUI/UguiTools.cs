using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using UniRx;

namespace Jois
{
    public static class UguiTools
    {
        public static void MakeFullStretch(Transform parent, RectTransform targetObj)
        {
            RectTransform rt = targetObj.GetComponent<RectTransform>();

            rt.SetParent(parent);
            
            rt.localScale = Vector3.one;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;
            rt.anchoredPosition = Vector2.zero;
        }

        public static void MakeFullStretch(RectTransform targetObj)
        {
            RectTransform rt = targetObj.GetComponent<RectTransform>();

            rt.localScale = Vector3.one;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.sizeDelta = Vector2.zero;
            rt.anchoredPosition = Vector2.zero;
        }

        public static void DisableAllUiText(Transform root)
        {
            Text[] allChildren = root.GetComponentsInChildren<Text>();
            foreach (Text child in allChildren)
            {
                child.enabled = false;
            }
        }

        static bool _buttonLocked;

        public static bool LockButtonWhile(float buttonLockedTime)
        {
            bool locked = _buttonLocked;
            if (locked)
            {
                _buttonLocked = true;
                //Observable.Timer(System.TimeSpan.FromSeconds(buttonLockedTime)).Subscribe(_ => _buttonLocked = false);
            }

            return locked;
        }

        public static void DisableByName(string name)
        {
        }
    }
}