using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using UnityEngine.UI;


namespace Jois
{
    public static class UnityWebReqTools
    {
        public static bool LoggerEnabled = true;

        public static async UniTask<string> RequestGetAsync(string url)
        {
            if (LoggerEnabled)
                Debug.LogFormat("-- [REQ Get]→url({0}) ", url);

            using (var uwr = new UnityWebRequest(url, "GET"))
            {
                uwr.downloadHandler = new DownloadHandlerBuffer();
                //uwr.SetRequestHeader("Content-Type", "text/plain;charset=UTF-8");

                await uwr.SendWebRequest().WithCancellation(CancellationToken.None);

                bool isSuccess = uwr.isNetworkError == false && uwr.isDone;

                var res = isSuccess ? uwr.downloadHandler.text : string.Empty;

                if (LoggerEnabled)
                    Debug.LogFormat("-- [RES Get]→{0} ", res);
                return res;
            }
        }

        public static async UniTask<string> RequestGetQuery(string url, Dictionary<string, string> queries,
            string jsonData = "{}")
        {
            string queryUrl = queries != null && queries.Count != 0
                ? "?" + string.Join("&", queries.Select(elem => elem.Key + "=" + elem.Value))
                : string.Empty;

            var finalUrl = url + queryUrl;

            if (LoggerEnabled)
                Debug.LogFormat("-- [REQ Get]→url({0}) json({1})  ", finalUrl, jsonData);

            using (var uwr = new UnityWebRequest(finalUrl, "GET"))
            {
                byte[] jsonForUtf8 = new System.Text.UTF8Encoding().GetBytes(jsonData);
                uwr.uploadHandler = new UploadHandlerRaw(jsonForUtf8);
                uwr.downloadHandler = new DownloadHandlerBuffer();
                uwr.SetRequestHeader("Content-Type", "application/json");

                await uwr.SendWebRequest().WithCancellation(CancellationToken.None);

                bool isSuccess = uwr.isNetworkError == false && uwr.isDone;

                var res = isSuccess ? uwr.downloadHandler.text : string.Empty;

                if (LoggerEnabled)
                    Debug.LogFormat("-- [RES Get]→{0} ", res);
                return res;
            }
        }

        public static async UniTask<string> RequestJson(string method, string url, string jsonData)
        {
            if (LoggerEnabled)
                Debug.LogFormat("-- [REQ Post]→url({0}) json({1})  ", url, jsonData);

            using (var uwr = new UnityWebRequest(url, method))
            {
                byte[] jsonForUtf8 = new System.Text.UTF8Encoding().GetBytes(jsonData);
                uwr.uploadHandler = new UploadHandlerRaw(jsonForUtf8);
                uwr.downloadHandler = new DownloadHandlerBuffer();
                uwr.SetRequestHeader("Content-Type", "application/json");

                await uwr.SendWebRequest().WithCancellation(CancellationToken.None);

                bool isSuccess = uwr.isNetworkError == false && uwr.isDone;
                var res = isSuccess ? uwr.downloadHandler.text : string.Empty;
                Debug.LogFormat("-- [RES Post]→{0} ", res);
                return res;
            }
        }
        
        public static async UniTask<string> RequestPostJson(string url, string jsonData)
        {
            if (LoggerEnabled)
                Debug.LogFormat("-- [REQ Post]→url({0}) json({1})  ", url, jsonData);

            using (var uwr = new UnityWebRequest(url, "POST"))
            {
                byte[] jsonForUtf8 = new System.Text.UTF8Encoding().GetBytes(jsonData);
                uwr.uploadHandler = new UploadHandlerRaw(jsonForUtf8);
                uwr.downloadHandler = new DownloadHandlerBuffer();
                uwr.SetRequestHeader("Content-Type", "application/json");

                await uwr.SendWebRequest().WithCancellation(CancellationToken.None);

                bool isSuccess = uwr.isNetworkError == false && uwr.isDone;
                var res = isSuccess ? uwr.downloadHandler.text : string.Empty;
                Debug.LogFormat("-- [RES Post]→{0} ", res);
                return res;
            }
        }

        public static async UniTask<Texture2D> GetRemoteTextureAsync(string url)
        {
            using (var uwr = UnityWebRequestTexture.GetTexture(url))
            {
                await uwr.SendWebRequest().WithCancellation(CancellationToken.None);

                bool isSuccess = uwr.isNetworkError == false && uwr.isDone;
                return isSuccess ? DownloadHandlerTexture.GetContent(uwr) : null;
            }
        }

        public static async UniTask<byte[]> DownloadFileAsync(string url)
        {
            using (var uwr = UnityWebRequest.Get(url))
            {
                await uwr.SendWebRequest().WithCancellation(CancellationToken.None);

                //uwr.downloadProgress;

                bool isSuccess = uwr.isNetworkError == false && uwr.isHttpError == false && uwr.isDone;
                return isSuccess ? uwr.downloadHandler.data : null;
            }
        }
    }
}