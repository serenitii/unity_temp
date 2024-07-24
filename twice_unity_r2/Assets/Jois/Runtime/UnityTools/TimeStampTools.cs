using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace Jois
{
    public  class TimeStampTools : MonoBehaviour
    {
        private const string kChargeGuideTimeStamp = "ChargeGuideTimeStamp";
        IEnumerator Start()
        {
            //SetTimeStamp(kChargeGuideTimeStamp);
        
            yield return new WaitForSeconds(3.0F);
        
            int elapsed = GetElapsedTime(kChargeGuideTimeStamp);
        
            Debug.LogFormat("elapsed seconds({0}) mins({1}) ", elapsed, elapsed/60);
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="key"></param>
        /// <returns> 지나간 초를 반환 </returns>
        public static int GetElapsedTime(string key)
        {
            if (PlayerPrefs.HasKey(key) == false)
                return 0;

            var time = PlayerPrefs.GetString(key);
            long binaryTime = Int64.Parse(time);
            var previous = DateTime.FromBinary(binaryTime);
            Debug.LogFormat("Previous Time( {0} )", previous.ToLongTimeString());
            var timeSpan = DateTime.Now.Subtract(previous);
            return (int) timeSpan.TotalSeconds;
        }

        public static void SetTimeStamp(string key)
        {
            Debug.LogFormat("SetTimeStamp( {0} ) ", DateTime.Now.ToLongTimeString());

            var now = DateTime.Now.ToBinary();
            PlayerPrefs.SetString(key, now.ToString());
        }

        public static void ClearTimeStamp(string key)
        {
            PlayerPrefs.DeleteKey(key);
        }

        public static int CalcLeftSecondsByNow(DateTime dateTime)
        {
            var timeSpan = DateTime.Now.Subtract(dateTime);

            return (int) timeSpan.TotalSeconds;
        }
    }
}