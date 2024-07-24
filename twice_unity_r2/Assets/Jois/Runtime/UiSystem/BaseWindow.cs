using System;
using DG.Tweening;
using UnityEngine;

namespace Jois
{
    // 비활성화될 때 스택에서 Pop
    //  활성화될 때  스택에서 Pop
    // Foreground: clearStack, hideStack, popStack,
    // Background: hideView, popStack   
    // clearStackOnForeground
    // hidePreviousOnForeground
    // popPreviousOnForeground

    // popStackOnBackground 
    // hideViewOnBackground
    // hideAllViewsOnBackground

    public delegate void MyTweenCallback();

    [Serializable]
    public enum WindowTransition
    {
        None,
        Left,
        Right,
        Top,
        Bottom
    }

    public class BaseWindow : MonoBehaviour
    {
        [SerializeField] protected string _windowName = string.Empty;

        [SerializeField] private WindowTransition _transitionOnOpen;
        [SerializeField] private WindowTransition _transitionOnClose;
        [SerializeField] private WindowTransition _transitionOnBackground;
        [SerializeField] private WindowTransition _transitionOnForeground;

        [SerializeField] private WindowTransition _transitionIn;
        [SerializeField] private WindowTransition _transitionOut;


        [Header("-- 백그라운드로 갈 때 Stack에서 Pop")] [SerializeField]
        private bool _popStackOnBackground;

        [Header("-- 백그라운드로 갈 때, DO NOT Hide View")] [SerializeField]
        private bool _doNotHideViewOnBackground;

        [Header(("-- 기존 스택에 영향을 끼치지 않는다"))] [SerializeField]
        private bool _isModalView;

        [Header("-- BackButton 시 닫음")] [SerializeField]
        private bool _closeOnBackButton = true;

        private int _id;

        [Space(10)] [HideInInspector] private int _intParam;
        private object _param;
        private bool _hasStarted;

        public bool CloseOnBackButton => _closeOnBackButton;

        public bool PopStackOnBackground => _popStackOnBackground;

        //public bool HideViewOnBackground => _hideViewOnBackground;

        public bool IsModalView => _isModalView;

        public int WindowId => _id;

        //public bool HideAllViewsOnForeground => _hideAllViewsOnForeground;


        // public bool PopPreviousOnBackground => _popPreviousOnBackground;
        // public bool PopPreviousOnForeground => _popPreviousOnForeground;
        // public bool PopStackOnBackButton => _popStackOnBackButton;
        // public bool HideAllWindowsOnForeground => _hideAllWindowsOnForeground;
        //public int IntParam => _intParam;


        public object Param => _param;
        public string WindowName => _windowName;


        public void FirstTimeInit(int winId)
        {
            _id = winId;
        }

        public virtual void OnStart() //int intParam, string stringParam)
        {
        }

        // 뷰가 보일 때 
        public virtual void OnUIViewShown(object param)
        {
            if (UiManager.Current.LoggerEnabled)
                Debug.LogFormat("<{0}> Show ({1})  ", WindowName, param ?? string.Empty);
        }

        // 다른 사라질 때, 다른 뷰가 덥을 때
        public virtual void OnUIViewHide()
        {
            if (UiManager.Current.LoggerEnabled)
                Debug.LogFormat("<{0}> Hide ", WindowName);
        }

        public virtual void OnUiMessage(string msg)
        {
            if (UiManager.Current.LoggerEnabled)
                Debug.LogWarningFormat("GOT Ui-Message ({0}) ", msg);
        }

        public void ShowWindow(object param)
        {
            //_intParam = intParam;
            _param = param;

            gameObject.SetActive(true);

            TransitionIn();

            if (_hasStarted == false)
            {
                _hasStarted = true;
                OnStart();
            }

            OnUIViewShown(param);
        }

        // background 로 간다 
        public void HideWindow(bool popInStack)
        {
            OnUIViewHide();

            TransitionOut(() =>
            {
                Debug.LogFormat("HideWindow transition ENDED !!! ");
                if (_doNotHideViewOnBackground == false)
                    gameObject.SetActive(false);

                if (popInStack)
                    gameObject.SetActive(false);
            });
        }

        public void CloseWindow()
        {
        }

        private const float DELAY = 0.3f;
        private const float WINDOW_GAP = 1500f;

        void TransitionIn()
        {
            var startPosition = Vector2.zero;

            switch (_transitionIn)
            {
                case WindowTransition.None:
                {
                    gameObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    break;
                }
                case WindowTransition.Left:
                {
                    startPosition = new Vector2(-WINDOW_GAP, 0);
                    break;
                }
                case WindowTransition.Right:
                {
                    startPosition = new Vector2(WINDOW_GAP, 0);
                    break;
                }
                case WindowTransition.Top:
                {
                    startPosition = new Vector2(0, WINDOW_GAP * 2.5F);
                    break;
                }
                case WindowTransition.Bottom:
                {
                    startPosition = new Vector2(0, -WINDOW_GAP * 2.5F);
                    break;
                }
            }

            if (_transitionIn != WindowTransition.None)
            {
                gameObject.GetComponent<RectTransform>().anchoredPosition = startPosition;
                gameObject.GetComponent<RectTransform>()
                    .DOAnchorPos(Vector2.zero, DELAY, true)
                    .OnComplete(() =>
                    {
                        //
                        Debug.LogFormat("transition ENDED !!! ");
                    });
            }
        }

        void TransitionOut(TweenCallback onFinish)
        {
            var endPosition = Vector2.zero;
            switch (_transitionOut)
            {
                case WindowTransition.None:
                {
                    gameObject.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                    onFinish();
                    break;
                }
                case WindowTransition.Left:
                {
                    endPosition = new Vector2(-WINDOW_GAP, 0f);
                    break;
                }
                case WindowTransition.Right:
                {
                    endPosition = new Vector2(WINDOW_GAP, 0f);
                    break;
                }
                case WindowTransition.Top:
                {
                    endPosition = new Vector2(0, WINDOW_GAP * 2.5f);
                    break;
                }
                case WindowTransition.Bottom:
                {
                    endPosition = new Vector2(0, -WINDOW_GAP * 2.5f);
                    break;
                }
            }

            if (_transitionOut != WindowTransition.None)
            {
                gameObject.GetComponent<RectTransform>()
                    .DOAnchorPos(endPosition, DELAY, true)
                    .OnComplete(onFinish);
            }
        }
    }

    public interface IAlertWindow
    {
        void OpenAlertWindow(string title, string message, Action onOk = null);
        void OpenAlertWindow(string title, string message, Action onYes, Action onNo);
    }
}