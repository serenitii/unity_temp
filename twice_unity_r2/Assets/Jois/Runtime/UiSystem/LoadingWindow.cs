using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace Jois
{
    public class LoadingWindow : BaseWindow
    {
        [SerializeField] private Image _background;
        [SerializeField] protected Image _spinner;
        [SerializeField] protected float _rotateSpeed = -300f;

        [SerializeField] private Color _backgroundColor;
        
        private RectTransform _rectComponent;

        void Start()
        {
            _rectComponent = _spinner.gameObject.GetComponent<RectTransform>();
        }

        void FixedUpdate()
        {
            _rectComponent.Rotate(0f, 0f, _rotateSpeed * Time.deltaTime);
        }

        // void SetTransparent(bool transparent)
        // {
        //     
        // }

        public void SetNormalMode()
        {
            gameObject.SetActive(true);
            _spinner.gameObject.SetActive(true);
            _background.color = _backgroundColor;
        }

        public void SetTransparent()
        {
            _spinner.gameObject.SetActive(false);
            _background.color= Color.clear;
        }
    }
}