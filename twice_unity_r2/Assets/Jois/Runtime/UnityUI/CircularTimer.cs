using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Jois
{
    public class CircularTimer : MonoBehaviour
    {
        [SerializeField] private Image gaugeImage = null;

        public void StartTimer(float duration, Action onComplete)
        {
            gameObject.SetActive(true);
            gaugeImage.fillAmount = 1f;
            gaugeImage.DOFillAmount(0f, duration).OnComplete(() =>
            {
                onComplete?.Invoke();
            });
        }

        public void CancelTimer()
        {
            gaugeImage.DOKill();
            gameObject.SetActive(false);
        }
    }
}