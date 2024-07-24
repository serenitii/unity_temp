using System;
using UnityEngine;

namespace Jois
{
    public class WorldSpaceUICameraResize : MonoBehaviour
    {
        private void Awake()
        {
            Camera cam = GetComponent<Camera>();
            
            cam.orthographicSize = (float) Screen.height * 0.01F * 0.5F; //5.4F;// 10.80 / 2, 14.40 / 2
        }
    }
}