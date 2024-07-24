using System;
using System.Collections;
using System.Collections.Generic;
using Jois;
using UnityEngine;
using UnityEngine.XR.ARFoundation;


namespace PhotoAr
{
    public class APP : MonoBehaviour
    {
        // [SerializeField] private ARTrackedImageManager _arTrackedImageManager;
        [SerializeField] private CacheFileManager _cacheFileManager;
        [SerializeField] private AppConfig _appConfig;
        [SerializeField] private RuntimeData _runtimeData;
        // [SerializeField] private TrackedImageManager _trackedImageManager;
        [SerializeField] private AppMsgCenter _msgCenter;
        [SerializeField] private ServerApi _serverApi;
        [SerializeField] private AppStates _appStates;

        // public static ARTrackedImageManager TrackedImageMgr { get; private set; }
        public static CacheFileManager CacheFileMgr { get; private set; }
        public static AppConfig Config { get; private set; }
        public static RuntimeData Data { get; private set; }
        // public static TrackedImageManager TrackedImageMgr { get; private set; }
        public static AppMsgCenter MsgCenter { get; private set; }
        public static ServerApi Api { get; private set; }
        public static AppStates States { get; private set; }

        public static ComboArObjManager ComboArMgr { get; set; } 
        
        #region Mono Singleton

        Guid _guid;
        private static APP _instance;
        static Guid _instanceGuid;

        void Awake()
        {
            Debug.Log("APP.Awake ");
            if (_instance == null)
            {
                _instance = this;
                InitStaticObjects();
                DontDestroyOnLoad(gameObject);

                _instanceGuid = Guid.NewGuid();
                _guid = _instanceGuid;
                Debug.LogFormat("APP Singleton has been created. guid({0}) ", _guid.ToString());
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void OnDestroy()
        {
            Debug.Log("APP.OnDestroy \n");
            if (_guid == _instanceGuid)
            {
                _instance = null;
                Debug.LogFormat("APP Singleton has been destroyed. guid({0}) ", _guid.ToString());
                Debug.Log("");
            }
        }

        #endregion Mono Singleton

        void Start()
        {
        }

        void InitStaticObjects()
        {
            // TrackedImageMgr = _arTrackedImageManager;
            CacheFileMgr = _cacheFileManager;
            Config = _appConfig;
            Data = _runtimeData;
            // TrackedImageMgr = _trackedImageManager;
            MsgCenter = _msgCenter;
            Api = _serverApi;
            States = _appStates;
        }
    }
}