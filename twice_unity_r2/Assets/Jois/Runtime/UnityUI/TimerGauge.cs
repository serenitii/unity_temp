using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class TimerGauge : MonoBehaviour
{
    [SerializeField] Image _gaugeImage;

    public void SetActive(float time)
    {
        gameObject.SetActive(true);
        _gaugeImage.fillAmount = 1f;
        _gaugeImage.DOFillAmount(0f, time);
    }

    public void CancelTimer(bool hide)
    {
        _gaugeImage.DOKill();
        gameObject.SetActive(!hide); 
    }
}
