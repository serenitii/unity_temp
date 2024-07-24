using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Jois
{
    public class SizeFitterToChild : MonoBehaviour
    {
        public enum FitType
        {
        }

        bool _canFit;

        [SerializeField] float _padX;


        void OnEnable()
        {
            FitWidth();
        }

        IEnumerator Start()
        {
            yield return null;
            _canFit = true;
            FitWidth();
        }

        void FitWidth()
        {
            if (_canFit == false)
                return;

            var rt = GetComponent<RectTransform>();
            var size = this.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta;
            size.x += _padX;
            size.y = rt.sizeDelta.y;
            rt.sizeDelta = size;
        }
    }
}