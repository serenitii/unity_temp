using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using Debug = UnityEngine.Debug;

namespace Jois
{
    public static class UTools
    {
        public static void PrintSystemInfo()
        {
            StringBuilder sb = new StringBuilder(1024);

            sb.Append(string.Format("currentResolution({0}) \n", Screen.currentResolution));

            var ipv4 = IPAddress.Broadcast.MapToIPv4();
            var ipv6 = IPAddress.Broadcast.MapToIPv6();

            // Get the IP Broadcast address and convert it to string.
            string ipAddressString = IPAddress.Broadcast.ToString();
            sb.Append(string.Format("Broadcast IP address: ipv4({0}) ipv6({1}) \n",
                ipv4.ToString(), ipv6.ToString()));

            sb.Append(string.Format("localIP({0}) \n", GetLocalIP()));

            Debug.Log(sb.ToString());

            sb.Append(string.Format("Default EncodingName({0}) \n", System.Text.Encoding.Default.EncodingName));

// #if UNITY_EDITOR
//             Debug.logger.logEnabled = true;
// #else
//   Debug.logger.logEnabled = false;
// #endif
            Debug.unityLogger.logEnabled = false;

            PrintLog(">>> It could be seen. - - -");
        }

        static string GetLocalIP()
        {
            string localIP = "Not available, please check your network seetings!";
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = ip.ToString();
                    break;
                }
            }

            return localIP;
        }

        [Conditional("DEBUG")]
        public static void PrintLog(string str)
        {
            Debug.Log(str);
        }


        public static async UniTask<Texture2D> GetUrlTexture(string url)
        {
            Debug.LogFormat("UTools.GetUrlTexture ({0}) ", url);

            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
            {
                //yield return uwr.SendWebRequest();
                await uwr.SendWebRequest().WithCancellation(CancellationToken.None);

                if (uwr.isNetworkError || uwr.isHttpError)
                {
                    Debug.Log(uwr.error);
                }
                else
                {
                    // Get downloaded asset bundle
                    var texture = DownloadHandlerTexture.GetContent(uwr);
                    return texture;
                }
            }

            return null;
        }

        public static void ClearChildren(this GameObject go)
        {
            for (var i = go.transform.childCount - 1; i >= 0; --i)
            {
                GameObject.Destroy(go.transform.GetChild(i).gameObject);
            }
        }


        public static string GetGameObjectPath(Transform transform)
        {
            string path = transform.name;
            while (transform.parent != null)
            {
                transform = transform.parent;
                path = transform.name + "/" + path;
            }

            return path;
        }

        public static IList<string> GetChildrenNames(Transform parent)
        {
            List<string> names = new List<string>();
            foreach (Transform child in parent)
            {
                names.Add(child.name);
            }

            return names;
        }

        public static bool IsRunningOnMobileDevice =>
            Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.IPhonePlayer;

        public static void ResetTransform(Transform parent)
        {
            parent.localPosition = Vector3.zero;
            parent.localRotation = Quaternion.identity;
            parent.localScale = Vector3.zero;
        }
    }
}