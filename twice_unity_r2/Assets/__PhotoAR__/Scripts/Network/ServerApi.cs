using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Jois;
using UnityEngine;

namespace PhotoAr
{


    public class ServerApi : MonoBehaviour
    {
        // private const string dataVersionUrl = "https://s3.ap-northeast-2.amazonaws.com/mys3.jeneri.net/photoar/app_header.json";
        // private const string dataUrl = "https://s3.ap-northeast-2.amazonaws.com/mys3.jeneri.net/photoar/mock_albums.json";

        private string _server;
        private bool _isServerOk;

        private const string testUrl2 = "http://localhost:5000/api/packages/header";

        private const string HealthCheck = "/users/hello";
        private const string SerialNumbers = "/serial-numbers";
        private const string LogEndpoint = "/ar/ar-logs";
        private const string sn = "sn=";
        private const string pid = "pid=";

        public void SetServerAddress(string value)
        {
            _server = value;
        }

        public async void CheckServerHealthy(Action<bool> onFinnish)
        {
            var result = await UnityWebReqTools.RequestGetAsync(_server + HealthCheck);
            if (string.IsNullOrEmpty(result))
            {
            }
            else
            {
                _isServerOk = result == "hello";
            }

            //Debug.LogFormat("[Is Server Ok]: ({0}) ", _isServerOk);

            onFinnish?.Invoke(_isServerOk);
        }

        public async Task<ResCheckSerialNumberUsableDto> RequestSerialNumberUsable(string serialNumber) //, Action<bool> onFinish)
        {
            var url = _server + SerialNumbers + "?" + sn + serialNumber + "&" + pid + "0";
            var result = await UnityWebReqTools.RequestGetAsync(url);
            var dto = JsonUtility.FromJson<ResCheckSerialNumberUsableDto>(result);
            //bool canUse = dto.serialNumber == serialNumber;
            return dto;
        }

        public async void RequestSerialNumberUse(string serialNumber, Action<bool> onFinish)
        {
            var url = _server + SerialNumbers; // + "?" + sn + serialNumber + "&" + pid + "0";
            var reqDto = new ReqUseSerialNumberDto
            {
                packageName = "",
                cardName = "",
                packageId = 0,
                serialNumber = serialNumber
            };
            var result = await UnityWebReqTools.RequestJson("PATCH", url, JsonUtility.ToJson(reqDto));
            var dto = JsonUtility.FromJson<ResUseSerialNumberDto>(result);
            Debug.LogFormat("result: t", result);
            //bool canUse = dto.serialNumber == serialNumber;
            //return dto;
        }

        public async void RequestLog(string logMsg, string param)
        {
            var url = _server + LogEndpoint;

            var reqDto = new ReqLog
            {
                uid = SystemInfo.deviceUniqueIdentifier,
                action = logMsg,
                param = param
            };

            try
            {
                var result = await UnityWebReqTools.RequestPostJson(url, JsonUtility.ToJson(reqDto));
                var dto = JsonUtility.FromJson<ResLog>(result);
                Debug.LogFormat("result: t", result);
            }
            catch (Exception e)
            {
                Debug.LogWarningFormat("log error : ", e.Message);
            }
        }


        void Start()
        {
            // TestRequestAsync();

            //LoadAppDataVersion();
        }

        // async void TestRequestAsync()
        // {
        //     var result = await UnityWebReqTools.RequestGetAsync(APP.Config.AppHeaderUrl); // Consts.appHeaderDataUrl); //dataVersionUrl);
        //     Debug.LogFormat("[Req] result: {0} ", result);
        //     // var result2 = await UnityWebReqTools.RequestGetAsync(testUrl2);
        //     // Debug.LogFormat("[Req] result: {0} ", result2);
        // }

#if false
    async void LoadAppDataVersion()
    {
        var result = await UnityWebReqTools.RequestGetAsync(dataVersionUrl);
        Debug.LogFormat("[Req] result: {0} ", result);
        var remoteVersion = JsonUtility.FromJson<AppDataVersionDto>(result);
        Debug.LogFormat("[Got] We've got the app version( {0} ) ", remoteVersion.version);

        var localVersion = PlayerPrefs.GetInt(Consts.PrefKey_AppDataVersion);
        Debug.LogFormat("version: {0} ", localVersion);

        if (localVersion < remoteVersion.version)
        {
            APP.CacheFileMgr.DownloadFile(dataUrl, "", Consts.AppDataFileName, f => { },
                (bool isSuccess, string savedPath) => { LoadAppData(savedPath); });
        }
        else
        {
            var path = APP.CacheFileMgr.GetCachedFilePath("", Consts.AppDataFileName);
            LoadAppData(path);
        }
    }

    void LoadAppData(string path)
    {
        var dataJson = File.ReadAllText(path);
        var appData = JsonUtility.FromJson<AppDataDto>(dataJson);
        APP.Data.SetAlbumData(appData);
    }
#endif
    }
}