using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Jois
{
    public class AudioTools : MonoBehaviour
    {
        [SerializeField] private AudioSource _audioSource;
        [SerializeField] private string _testAudioUrl;
        [SerializeField] private AudioType _audioType;

        IEnumerator Start()
        {
            yield return new WaitForSeconds(2F);

            StartCoroutine(GetAudioClip_Co(_testAudioUrl, _audioType));
        }

        IEnumerator GetAudioClip_Co(string audioUrl, AudioType audioType)
        {
            using (var www = UnityWebRequestMultimedia.GetAudioClip(audioUrl, audioType))
            {
                yield return www.SendWebRequest();

                if (www.isNetworkError || www.isHttpError)
                {
                    Debug.LogErrorFormat("GetAudioClip got Error ({0}) ", www.error);
                }
                else
                {
                    _audioSource.clip = DownloadHandlerAudioClip.GetContent(www);
                    _audioSource.Play();
                }
            }
        }
    }
}