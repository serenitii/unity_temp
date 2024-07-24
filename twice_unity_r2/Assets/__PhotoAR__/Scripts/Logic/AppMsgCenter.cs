using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PhotoAr
{
    public class AppMsgCenter : MonoBehaviour
    {
        // packageName, targetName, videoUrl
        public Action<string, string, string> OnDownloadVideoRequest;

        // targetName, serialNumber
        public Action<string, string> OnSerialNumberRequest;

        public Action<bool> OnComboStatusChanged;
    }
}