using System.Collections;
using System.Collections.Generic;
using Jois;
using UnityEngine;
using UnityEngine.UI;

public class JoisUnityWebReqTools : MonoBehaviour
{
    [SerializeField] private Button _postRawJsonButton;

    [SerializeField] private Text _postRawJsonUrlText;
    [SerializeField] private Text _postRawJsonText;

    // Start is called before the first frame update
    void Start()
    {
        _postRawJsonButton.onClick.AddListener(() =>
        {
            TestPostRawJson();
        });
    }

    async void TestPostRawJson()
    {
        var resJson = await UnityWebReqTools.RequestPostJson(_postRawJsonUrlText.text, _postRawJsonText.text);

        Debug.LogFormat("Res Json:{0} ", resJson);
    }
}