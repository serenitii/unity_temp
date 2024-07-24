using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace PhotoAr
{
    public class LogoIntroPage : Jois.BaseWindow
    {
        [SerializeField] private RectTransform[] _stars;
        [SerializeField] private Button _testButton;
        [SerializeField] private float _interval = 0.1f;
        [SerializeField] private float _duration = 0.5f;
        
        private Vector3[] _initialScales;
        private static readonly int[] sequence = new[] {0, 4, 2, 3, 1};

        
        void Start()
        {
            _initialScales = new Vector3[_stars.Length];
            for (int i = 0; i < _stars.Length; ++i)
            {
                _initialScales[i] = _stars[i].localScale;
            }
            
            for (int i = 0; i < _stars.Length; ++i)
            {
                _stars[i].gameObject.SetActive(false);
            }

            _testButton.onClick.AddListener(() =>
            {
                //
                AnimateStars();
            });
            
            AnimateStars();
        }

        void AnimateStars()
        {
            float[] delays = new[] {0.5f * _interval, 1f * _interval, 3f * _interval, 4f * _interval, 5f * _interval};
            for (int i = 0; i < 5; ++i)
            {
                var j = sequence[i];

                DOVirtual.DelayedCall(delays[i], () =>
                {
                    _stars[j].gameObject.SetActive(true);
                });
            }
            for (int i = 0; i < 5; ++i)
            {
                var j = sequence[i];

                //_stars[j].DOScale(_initialScales[j], delays[i]);
                //_stars[j].DOPunchScale(_initialScales[j], delays[i], 3, 1.4f);
                _stars[j].DOShakeScale(_duration, _initialScales[j], 3, 4f, false).SetDelay(delays[i] + 0.05f);
            }
        }
    }
}