using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Jois
{
    public class UnityWebReqBehaviour : MonoBehaviour
    {
        public void RequestPostRawJson(string url, string jsonData, Action<bool, string> onFinish)
        {
            StartCoroutine(PostRawJson(url, jsonData, onFinish));
        }
        
        IEnumerator PostRawJson(string url, string jsonData, Action<bool, string> onFinish)
        {
            using (var uwr = new UnityWebRequest(url, "POST"))
            {
                byte[] jsonForUtf8 = new System.Text.UTF8Encoding().GetBytes(jsonData);
                uwr.uploadHandler = new UploadHandlerRaw(jsonForUtf8);
                uwr.downloadHandler = new DownloadHandlerBuffer();
                uwr.SetRequestHeader("Content-Type", "application/json");
                
                yield return uwr.SendWebRequest();
                
                Debug.LogFormat("[UWR] Result: responseCode({0}) ", uwr.responseCode);
                if (uwr.isNetworkError || uwr.isHttpError)
                {
                    Debug.LogWarningFormat("Request Failed ({0}) req-json({1}) ", uwr.error, jsonData);
                    
                    onFinish(true, string.Empty);
                }
                else
                {
                    Debug.Log("Form upload complete! ");

                    onFinish(true, uwr.downloadHandler.text);
                }
            }
        } 
    }
}