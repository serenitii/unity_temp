using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpaceCanvasScaler : MonoBehaviour
{
    private RectTransform _canvasRect;

    private void Awake()
    {
        Debug.LogFormat("[WorldSpaceCanvasScaler] Screen Resolution ({0}, {1}) ", Screen.width, Screen.height);
        _canvasRect = GetComponent<RectTransform>();
        _canvasRect.sizeDelta = new Vector2(Screen.width, Screen.height);
    }
}