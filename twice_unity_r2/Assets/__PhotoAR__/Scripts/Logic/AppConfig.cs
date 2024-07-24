using UnityEngine;
using UnityEngine.Serialization;

namespace PhotoAr
{
    public class AppConfig : MonoBehaviour
    {
        [SerializeField] private bool _isReleaseVersion;

        [SerializeField] private bool _useLocalDataJson;

        [SerializeField] private string _appHeaderUrl;
        
        public bool IsReleaseVersion => _isReleaseVersion;

        public bool UseLocalDataJson => _useLocalDataJson;

        public string AppHeaderUrl => _appHeaderUrl;

        public GameObject _directionDetector;
        [FormerlySerializedAs("_rightTarget")] public GameObject _leftTarget;
        
        public bool IsIPadRatio()
        {
#if true 
            if (Jois.UTools.IsRunningOnMobileDevice == false)
                return false;

            float currentRatio = (float)Screen.currentResolution.width / (float)Screen.currentResolution.height;
            return 1080f / 1920f < currentRatio;
#else
            return true;
#endif
        }

        public bool iOSReviewEnabled()
        {
            return Application.platform == RuntimePlatform.IPhonePlayer;
            // return true;
        }
    }
}