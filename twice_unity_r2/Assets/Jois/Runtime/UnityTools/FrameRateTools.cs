using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Jois
{
    public enum FrameRateQuality
    {
        // Low_1 = 5,
        // Low_2 = 10,
        // Base = 20,
        // Mid = 30,
        // High = 40,
        // Ultra = 60,
        // Max = 300

        Level_0,
        Level_1,
        Level_2,
        Level_3,
        Level_4,
        Level_5,
        Level_6,
        Level_7
    }
    
    public class FrameRateTools : MonoBehaviour
    {
        readonly int[] kFrameRatesByLevel = {2, 5, 10, 15, 20, 40, 50, 60, 300};

        [Header("Level: 2, 5, 10, 15, 20, 40, 50, 60, 300")] [SerializeField]
        FrameRateQuality _baseLevel = FrameRateQuality.Level_3;

        bool _isForceMode;

        private void Start()
        {
            Application.targetFrameRate = kFrameRatesByLevel[(int) _baseLevel];

            //SetQualityFromBaseForceMode(2, 0, 60F); // 시작 후 1분동안은 높은 프레임으로
        }

        void SetQualityFromBaseForceMode(int target, int returnBack, float duration = 1f)
        {
            _isForceMode = true;
        
            Application.targetFrameRate = kFrameRatesByLevel[(int) _baseLevel + target];

            if (duration > 0)
                StartCoroutine(SustainFrameRate(returnBack, duration));
        }
    
        public void SetQualityFromBase(int target, int returnBack, float duration = 1f)
        {
            if (_isForceMode)
                return;
        
            Application.targetFrameRate = kFrameRatesByLevel[(int) _baseLevel + target];

            if (duration > 0)
                StartCoroutine(SustainFrameRate(returnBack, duration));
        }

        IEnumerator SustainFrameRate(int phase, float duration)
        {
            yield return new WaitForSeconds(duration);

            // 한단계 낮춘다
            // int level = (int) quality - 1;
            // level = level < 0 ? 0 : level;
            phase = phase < 0 ? 0 : phase;
            Application.targetFrameRate = kFrameRatesByLevel[phase + (int) _baseLevel];
            _isForceMode = false;
        }
    }
}