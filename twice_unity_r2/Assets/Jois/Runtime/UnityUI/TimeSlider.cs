using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace joi
{
    public class TimeSlider : MonoBehaviour
    {
        [SerializeField] Slider _slider;

        float sliderTime;
        float leftTime;
        System.Action onCompleteCallback;

        void Awake()
        {
            this.enabled = false;
        }

        void Update() 
        {
            leftTime -= Time.deltaTime;

            _slider.value = leftTime / sliderTime;

            if (leftTime <= 0f)
            {
                leftTime = 0f;
                _slider.value = 0f;

                if (onCompleteCallback != null)
                    onCompleteCallback();

                StopTimer();
            }
        }

        public void StartTimer(float delay, float time, System.Action onComplete)
        {
            if (_slider == null)
                gameObject.GetComponent<Slider>();
        
            sliderTime = time;
            leftTime = time;
            onCompleteCallback = onComplete;

            if (delay > 0f)
                StartCoroutine(DelayedEnabled(delay));
            else
                this.enabled = true;
        }

        public void StopTimer()
        {
            this.enabled = false;
        }

        IEnumerator DelayedEnabled(float delay)
        {
//        Debug.LogFormat("slider started \n");

            yield return new WaitForSeconds(delay);
            this.enabled = true;
        }

        public float Value
        {
            get => _slider.value;
            set => _slider.value = value;
        }
    }
}
