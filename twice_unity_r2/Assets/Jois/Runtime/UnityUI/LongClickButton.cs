using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class LongClickButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Action OnDown;
    public Action OnUp;
    
    //private bool pointerDown;
    private float pointerDownTimer;

    [SerializeField]
    private float requiredHoldTime;

    public UnityEvent onLongClick;

    //[SerializeField]
    //private Image fillImage;

    public void OnPointerDown(PointerEventData eventData)
    {
        //pointerDown = true;
        Debug.Log("OnPointerDown \n");
        OnDown?.Invoke();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("OnPointerUp \n");
        OnUp?.Invoke();
    }

    // private void Update()
    // {
    //     if (pointerDown)
    //     {
    //         pointerDownTimer += Time.deltaTime;
    //         if (pointerDownTimer >= requiredHoldTime)
    //         {
    //             if (onLongClick != null)
    //                 onLongClick.Invoke();
    //
    //             Reset();
    //         }
    //         //fillImage.fillAmount = pointerDownTimer / requiredHoldTime;
    //     }
    // }
    //
    // private void Reset()
    // {
    //     pointerDown = false;
    //     pointerDownTimer = 0;
    //     //fillImage.fillAmount = pointerDownTimer / requiredHoldTime;
    // }
}
