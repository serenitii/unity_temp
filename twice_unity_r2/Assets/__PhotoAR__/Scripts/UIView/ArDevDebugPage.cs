using System.Collections;
using System.Collections.Generic;
using PhotoAr;
using UnityEngine;
using UnityEngine.UI;

public class ArDevDebugPage : MonoBehaviour
{
    [SerializeField] Text _comboArText;
    

    void Update()
    {
        if (ComboArObjManager.Current)
            _comboArText.text = string.Format("combos( {0} ) ", ComboArObjManager.Current.ArObjs.Count);
    }
}
