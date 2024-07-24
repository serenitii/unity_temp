using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestArUI : MonoBehaviour
{
    [SerializeField] private Button _exitButton;
    
    void Start()
    {
        _exitButton.onClick.AddListener(() =>
        {
            Application.Quit(0);
        });
        
    }
}
