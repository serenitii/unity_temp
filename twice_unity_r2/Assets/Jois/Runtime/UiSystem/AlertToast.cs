using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Jois
{
    public class AlertToast : MonoBehaviour
    {
        [SerializeField] Text _messageText;

        Image _backgroundImage;

        float _duration;

        const float kMinimumDuration = 1f;

        void Start()
        {
            _backgroundImage = gameObject.GetComponent<Image>();
        }

        IEnumerator ShowToastImpl(string message, float duration)
        {
            gameObject.SetActive(true);
            _duration = duration > kMinimumDuration ? duration : kMinimumDuration;
            _messageText.text = message;
            yield return new WaitForSeconds(_duration);
            gameObject.SetActive(false);
        }

        public void ShowToast(string message, float duration = kMinimumDuration)
        {
            gameObject.SetActive(true);
            StartCoroutine(ShowToastImpl(message, duration));
        }
    }
}