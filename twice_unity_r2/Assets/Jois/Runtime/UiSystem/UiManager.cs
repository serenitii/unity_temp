using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Jois
{
    public enum PopupType
    {
        SimpleAlert, // title, message, ok-button
        OkCancelPopup,
    }

    public enum WindowTransitStatus
    {
        Open,
        Close,
        Foreground,
        Background,
    }

    /// <summary>
    /// 씬 기반의 UI Manager
    /// OpenWindow : 스택에 이미 있는 윈도이면 top 으로 놓고 show, 없으면 push 후 show
    /// PopWindow : 스텍의 탑에 잇는 윈도 제거 
    /// </summary>
    public class UiManager : MonoBehaviour
    {
        #region Configurations

        [Header("-- Debug.Log 출력")] [SerializeField]
        private bool _loggerEnabled = true;

        [SerializeField] protected BaseWindow[] _windows;
        [SerializeField] protected List<string> _prefabPaths;

        [SerializeField] protected LoadingWindow _loadingWindow;

        [Header("-- 윈도 스택이 비었을 때, 마지막 윈도 활성화")] [SerializeField]
        private bool _noEmptyView = true;

        #endregion

        [Header("- For Debug, DO NOT EDIT")] [SerializeField]
        List<string> _debugWindowStack = new List<string>();

        private Stack<BaseWindow> _windowStack = new Stack<BaseWindow>();

        private Dictionary<string, int> _windowNameMap = new Dictionary<string, int>(32);

        private int _loadingCount;

        private int _lastPoppedWindow;

        //private string _previousWindowName = string.Empty;

        public Action<int, string> OnUIViewShown;


        public static UiManager Current;

        public static int ReservedWindowId;

        #region MonoBehaviour

        public enum UiEvent
        {
            ViewPushed,
            ViewPopped
        }

        public System.Action<string, UiEvent> OnViewChanged;

        public void OnSceneLoad(Scene scene, LoadSceneMode mode)
        {
            Debug.LogFormat("UiManager.OnSceneLoad ({0}) \n", scene.name);

            _windowStack = new Stack<BaseWindow>(8);
            _debugWindowStack = new List<string>(8);
        }

        private void OnEnable()
        {
            InitializeUiSystem();
        }

        void Start()
        {
            if (ReservedWindowId > 0)
            {
                OpenWindow(ReservedWindowId);
                ReservedWindowId = 0;
            }
        }

        #endregion


        //public string LatestWindowName => _windowStack.Count > 0 ? _windowStack.Peek().name : string.Empty;

        public string PreviousWindowName => string.Empty;
        // _previousWindowName; //_lastPopWindow > 0 ? _windows[_lastPopWindow].WindowName : string.Empty;

        public int StackCount => _windowStack.Count;

        #region Main Methods

        public bool LoggerEnabled => _loggerEnabled;

        void InitializeUiSystem()
        {
            if (_loggerEnabled)
                Debug.LogFormat("Jois.UiManager.InitializeUiSystem scene({0}) ", SceneManager.GetActiveScene().name);

            Current = this;

            int winId = -1;
            foreach (var window in _windows)
            {
                ++winId;
                if (window != null)
                {
                    window.FirstTimeInit(winId);
                    window.gameObject.SetActive(false);
                }
            }

            for (int i = 0; i < _windows.Length; ++i)
            {
                if (_windows[i] != null)
                {
                    if (_loggerEnabled)
                        Debug.AssertFormat(string.IsNullOrEmpty(_windows[i].WindowName) == false, "No WindowName index({0}) \n", i);

                    _windowNameMap.Add(_windows[i].WindowName, i);
                }
            }

            _windowStack = new Stack<BaseWindow>();
            _debugWindowStack = new List<string>();
        }

        public BaseWindow OpenWindow(int windowId, object param = null)
        {
            return OpenWindow(_windows[windowId], param);
        }

        public IAlertWindow OpenAlertWindow(int windowId)
        {
            var alertWindow = _windows[windowId];

            OpenWindow(alertWindow, null);

            return alertWindow as IAlertWindow;
        }

        // public BaseWindow OpenWindow(string windowName, object param = null)
        // {
        //     int windowId = _windowNameMap[windowName];
        //     return OpenWindow(_windows[windowId], param);
        // }

        public BaseWindow OpenWindow(BaseWindow newWindow, object param)
        {
            // 1. 스택에 이미 윈도가 존재하면 해당 윈도가 Top 이 될 때까지 Pop한
            var openedInStack = OpenInStack(newWindow, param);
            if (openedInStack != null)
            {
                OnViewChanged?.Invoke(newWindow.WindowName, UiEvent.ViewPushed);
                return openedInStack;
            }

            // 2. 오픈할 윈도가 스택에 없는 경우, top을 hide 시키고, push 후 show  
            if (_loggerEnabled)
                Debug.LogFormat("- OpenWindow Stack( {0} ) ↓ < {1} > ", string.Join(", ", _debugWindowStack), newWindow.WindowName);

            OnViewChanged?.Invoke(newWindow.WindowName, UiEvent.ViewPushed);

            // 2.1 Hide
            if (newWindow.IsModalView)
            {
                // 기존 윈도우 스택에 영향을 주지 않는다
            }
            else
            {
                // To Background, Close the window
                if (_windowStack.Count > 0)
                {
                    var topWindow = _windowStack.Peek();
                    if (topWindow.PopStackOnBackground)
                    {
                        _PopWindowInStack();
                    }
                    topWindow.HideWindow(topWindow.PopStackOnBackground);
                }
            }


            // 2.2 Show
            _PushWindowInStack(newWindow);

            newWindow.ShowWindow(param);

            OnUIViewShown?.Invoke(newWindow.WindowId, newWindow.WindowName);

            return newWindow;
        }

        public void PopWindow()
        {
            if (_windowStack.Count <= 1 && _loggerEnabled)
                Debug.LogWarningFormat("PopWindow: No Windows To Pop\n");

            var previousPoppedWindow = _windowStack.Count == 1 ? _lastPoppedWindow : 0;

            Debug.Assert(_debugWindowStack.Count == _windowStack.Count);

            if (_windowStack.Count > 0)
            {
                var popWindow = _PopWindowInStack();
                var nextTop = _windowStack.Count > 0 ? _windowStack.Peek() : null;

                /// !!!!!!!

                if (_loggerEnabled)
                {
                    Debug.LogFormat("- PopWindow Stack( {0} ) ↑ < {1} > ", string.Join(", ", _debugWindowStack), popWindow.WindowName);
                }

                OnViewChanged?.Invoke(popWindow.WindowName, UiEvent.ViewPopped);

                _lastPoppedWindow = popWindow.WindowId;

                //if (useHide)

                popWindow.HideWindow(true);

                //_previousWindowName = popWindow.WindowName;

                if (_windowStack.Count > 0 && popWindow.IsModalView == false)
                {
                    var win = _windowStack.Peek();
                    win.ShowWindow(null);
                    OnUIViewShown?.Invoke(win.WindowId, win.WindowName);
                }
            }

            if (previousPoppedWindow > 0)
                OpenWindow(previousPoppedWindow);
        }

        // void PopWindow(bool handlePrevious, bool useHide)
        // {
        //     if (_windowStack.Count == 0 && _loggerEnabled)
        //         Debug.LogWarningFormat("PopWindow: No Windows To Pop\n");
        //
        //     var lastPoppedWindow = _windowStack.Count == 1 ? _lastPoppedWindow : 0;
        //
        //     Debug.Assert(_debugWindowStack.Count == _windowStack.Count);
        //
        //     if (_windowStack.Count > 0)
        //     {
        //         var popWindow = _PopWindowInStack();
        //
        //         if (_loggerEnabled)
        //             Debug.LogFormat("- PopWindow Stack( {0} ) ↑ < {1} > ", string.Join(", ", _debugWindowStack), popWindow.WindowName);
        //
        //         OnViewChanged?.Invoke(popWindow.WindowName, UiEvent.ViewPopped);
        //
        //         _lastPoppedWindow = popWindow.WindowId;
        //
        //         if (useHide)
        //             popWindow.HideWindow(true);
        //
        //         //_previousWindowName = popWindow.WindowName;
        //
        //         if (handlePrevious && _windowStack.Count > 0) // && popWindow.IsModalView == false)
        //         {
        //             var win = _windowStack.Peek();
        //             win.ShowWindow(null);
        //             OnUIViewShown?.Invoke(win.WindowId, win.WindowName);
        //         }
        //     }
        //
        //     if (lastPoppedWindow > 0)
        //         OpenWindow(lastPoppedWindow);
        // }

        // 스택에 이미 윈도가 존재하면 해당 윈도가 Top 이 될 때까지 Pop한다
        BaseWindow OpenInStack(BaseWindow window, object param)
        {
            Debug.Assert(window != null);

            var exists = false;
            int popCount = 0;
            foreach (var item in _windowStack)
            {
                if (item.WindowId == window.WindowId)
                {
                    exists = true;
                    break;
                }

                ++popCount;
            }

            if (exists)
            {
                if (popCount == 1)
                {
                    PopWindow();
                }
                else if (popCount > 1)
                {
                    for (var i = 0; i < popCount; ++i)
                        PopWindow(); //false, i == 0);

                    Debug.Assert(_windowStack.Peek().WindowId == window.WindowId);

                    var win = _windowStack.Peek();
                    win.ShowWindow(param);
                    OnUIViewShown?.Invoke(win.WindowId, win.WindowName);
                }

                return _windowStack.Peek();
            }

            return null;
        }

        public void ClearAllWindows()
        {
            while (_windowStack.Count > 0)
            {
                this.PopWindow();
            }
        }

        public void OnBackButtonPress(int minimumLeft)
        {
            if (_windowStack.Count > minimumLeft && _windowStack.Peek().CloseOnBackButton)
                PopWindow();
        }

        public void SendUiMessage(string msg)
        {
            _windowStack.Peek().OnUiMessage(msg);
        }


        public void OpenPopup(int windowId, Action<string> onClose)
        {
        }

        #endregion


        #region For Debug

        public virtual string WindowStackInfo()
        {
            return _debugWindowStack.Count > 0 ? string.Join(", ", _debugWindowStack) : string.Empty;
        }

        #endregion

        public LoadingWindow ShowLoading(int id = 0)
        {
            if (_loggerEnabled)
                Debug.LogFormat("OOOO!----ShowLoading {0} id({1}) \n", ++_loadingCount, id);

            Debug.Assert(_loadingWindow.gameObject.activeInHierarchy == false);

            //_loadingWindow.gameObject.SetActive(true);
            _loadingWindow.SetNormalMode();

            return _loadingWindow;
        }

        public void HideLoading(int id = 0)
        {
            if (_loggerEnabled)
                Debug.LogFormat("XXXX!----HideLoading {0} id({1}) \n", --_loadingCount, id);

            _loadingWindow.gameObject.SetActive(false);

            //return _loadingWindow;
        }

        public void LoadScene(int sceneBuildIndex, int windowId = 0)
        {
            ReservedWindowId = windowId;
            SceneManager.LoadScene(sceneBuildIndex);
        }

        void _PushWindowInStack(BaseWindow window)
        {
            _windowStack.Push(window);
            _UpdateDebugWindowStack();
        }

        BaseWindow _PopWindowInStack()
        {
            var window = _windowStack.Pop();
            _UpdateDebugWindowStack();
            return window;
        }

        #region Debug

        void _UpdateDebugWindowStack()
        {
            _debugWindowStack.Clear();
            foreach (var item in _windowStack)
                _debugWindowStack.Add(item.WindowName);
        }

        #endregion
    }
}