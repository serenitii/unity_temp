using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace Jois
{
    public class TimeScaleSlider : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private Text _currentValueText;

        private void Start()
        {
            _slider.value = Time.timeScale;
            _currentValueText.text = $"{Time.timeScale:F2}";
            
            _slider.onValueChanged.AddListener(delegate(float value)
            {
                Time.timeScale = value;
                _currentValueText.text = $"{value:F2}";
            });
        }
    }
}