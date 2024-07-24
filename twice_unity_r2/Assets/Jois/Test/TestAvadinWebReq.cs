#if USE_BESTHTTP2
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Jois;
using LitJson;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using BestHTTP;


public class TestAvadinWebReq : MonoBehaviour
{
    [SerializeField] private Button _uwrButton;

    const string url_TTS = "http://dev.dasomi.ai/ASR";
    const string url_TTS_Add = "/common/set_tts";

    private const string _ttsUrl = url_TTS + url_TTS_Add;

    [SerializeField] private string _testText;
    [SerializeField] AudioSource _audioSource;

    private bool _audioCoroutineEnabled;

    private float _timeStarted;
    
    
    
    void Start()
    {
        AvatarSpeak_Async(_testText);
    }

    async Task AvatarSpeak_Async(string text)
    {
        _timeStarted = Time.realtimeSinceStartup;

        Debug.LogFormat("AvatarSpeak_Async text({0}) time({1:F3}), ttsUrl({2}) ", text, _timeStarted, _ttsUrl);

        Debug.Assert(_audioCoroutineEnabled == false);

        await Task.Delay(TimeSpan.FromSeconds(0.1F));

        var strJson = SerializeTTS(text);

        Debug.LogFormat("TextToVoice strJson: ({0})", strJson);

        var resultString = await PostJsonAsync(_ttsUrl, strJson);

        Debug.LogFormat(">>> Res ({0}) ", resultString);

        JsonData jsonPlayer = JsonMapper.ToObject(resultString); //request.Response.DataAsText);

        string statusCode = jsonPlayer["status_code"].ToString();
        if (statusCode.Equals("0"))
        {
            string audioUrl = jsonPlayer["json"]["audioUrl"].ToString();
            Debug.LogFormat(">>> Audio Url ({0}) ", audioUrl);

            StartCoroutine(GetAudioClip_Co(audioUrl));

            while (_audioCoroutineEnabled)
                await Task.Delay(TimeSpan.FromSeconds(0.1F));

            Debug.LogFormat("AvatarSpeak_Async text({0}) elapsedTime({1:F3})", text,
                Time.realtimeSinceStartup - _timeStarted);

            _audioSource.Play();

            await Task.Delay(TimeSpan.FromSeconds(_audioSource.clip.length));
        }
        else
        {
        }

        Debug.LogFormat("AvatarSpeak_Async text({0}) elapsedTime({1:F3})", text,
            Time.realtimeSinceStartup - _timeStarted);
    }

    async Task<string> PostJsonAsync(string url, string json)
    {
        var request = new HTTPRequest(new Uri(url), HTTPMethods.Post);
        request.AddHeader("Content-Type", "application/json");

        var encoding = new System.Text.UTF8Encoding();
        request.RawData = encoding.GetBytes(json);

        var resultString = await request.GetAsStringAsync();

        Debug.LogFormat("Res ({0}) ", resultString);

        return resultString;
    }

    IEnumerator GetAudioClip_Co(string url)
    {
        _audioCoroutineEnabled = true;

        using (UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.UNKNOWN))
        {
            yield return www.Send();

            if (www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                AudioClip myClip = DownloadHandlerAudioClip.GetContent(www);
                Debug.LogFormat("Successfully Donwloaded Mp3. length({0}) ", myClip.length);

                _audioSource.clip = myClip;
                //_audioSource.Play();
            }

            _audioCoroutineEnabled = false;
        }
    }

    string SerializeTTS(string msg)
    {
        JsonData jsonData = new JsonData
        {
            ["TEXT"] = msg,
            ["VOLUME"] = "0",
            ["SPEED"] = "0",
            ["PITCH"] = "0",
            ["EMOTION"] = "0",
            ["languageCode"] = "ko-KR"
        };
        // langCode(); //"ja-JP"

        var strJson = jsonData.ToJson();

        return strJson;
    }
}
#endif