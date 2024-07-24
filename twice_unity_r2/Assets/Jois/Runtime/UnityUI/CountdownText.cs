using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;


namespace Jois
{
    [RequireComponent(typeof(Text))]
    public class CountdownText : MonoBehaviour
    {
        Text _text;

        private bool _stopped;

        void Awake()
        {
            _text = GetComponent<Text>();
        }

        private void OnDisable()
        {
            _stopped = true;
        }

        public void StartCountDown(int number)
        {
            gameObject.SetActive(true);
            _text.text = number.ToString();
            _stopped = false;

            //StartCoroutine(CountDownImpl(number));
            CountDownAsync(number);
        }

        async void CountDownAsync(int seconds)
        {
            //Debug.LogFormat("- {0:F0} ", Time.realtimeSinceStartup);
            int number = seconds;
            for (int i = 0; i < seconds; ++i)
            {
                // if (_stopped)
                //     break;

                _text.text = number.ToString();
                --number;
                
                //await Task.Delay(TimeSpan.FromSeconds(1F));

                await UniTask.Delay(TimeSpan.FromSeconds(1F));

                //Debug.LogFormat("- {0:F0} ", Time.realtimeSinceStartup);
            }
        }

        // IEnumerator CountDownImpl(int seconds)
        // {
        //     //Debug.LogFormat("- {0:F0} ", Time.realtimeSinceStartup);
        //     int number = seconds;
        //     for (int i = 0; i < seconds; ++i)
        //     {
        //         _text.text = number.ToString();
        //         --number;
        //         yield return new WaitForSeconds(1F);
        //         //Debug.LogFormat("- {0:F0} ", Time.realtimeSinceStartup);
        //     }
        // }

        public void StopCountDown()
        {
            //StopAllCoroutines();

            _stopped = true;
            gameObject.SetActive(false);
        }
    }
}