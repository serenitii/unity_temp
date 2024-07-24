using System;
using System.Collections;
using System.Collections.Generic;
using PhotoAr;
using UnityEngine;

public class DirectionDetector : MonoBehaviour
{
    private static int _count;


    private void Start()
    {
        // var scale = transform.localScale;
        // transform.localScale = new Vector3(0.001F * scale.x, 0.001F * scale.y, 0.001F * scale.z);
    }

    private void OnTriggerEnter(Collider other)
    {
        var xform = other.GetComponent<Transform>();
        // if (xform == null)
        //     Debug.LogFormat("xform is null ");
        // else
        // {
        //     // if (xform.parent.parent == null)
        //     Debug.LogFormat("xform.parent {0} ", xform.parent.name);
        //     Debug.LogFormat("xform.parent.parent {0} ", xform.parent.parent.name);
        //     Debug.LogFormat("xform ... ImageTargetHandler: {0} ", xform.parent.parent.GetComponent<ImageTargetHandler>());
        // }

        var name0 = xform.parent.parent.GetComponent<ArPlaceContainer>().CardName;
        Debug.LogFormat("OnTriggerEnter - ({0}) ", name0);

        if (APP.ComboArMgr._objectsRightSide.ContainsKey(name0) == false)
            APP.ComboArMgr._objectsRightSide.Add(name0, ++_count);
    }

    private void OnTriggerStay(Collider other)
    {
        var xform = other.GetComponent<Transform>();
        // if (xform == null)
        //     Debug.LogFormat("xform is null ");
        // else
        // {
        //     // if (xform.parent.parent == null)
        //     Debug.LogFormat("xform.parent {0} ", xform.parent.name);
        //     Debug.LogFormat("xform.parent.parent {0} ", xform.parent.parent.name);
        //     Debug.LogFormat("xform ... ImageTargetHandler: {0} ", xform.parent.parent.GetComponent<ImageTargetHandler>());
        // }

        var name0 = xform.parent.parent.GetComponent<ArPlaceContainer>().CardName;
        //  Debug.LogFormat("OnTriggerStay - ({0}) ", name0);

        if (APP.ComboArMgr._objectsRightSide.ContainsKey(name0) == false)
            APP.ComboArMgr._objectsRightSide.Add(name0, ++_count);
    }

    private void OnTriggerExit(Collider other)
    {
        var name0 = other.GetComponent<Transform>().parent.parent.GetComponent<ArPlaceContainer>().CardName;
        Debug.LogFormat("OnTriggerExit - ({0}) ", name0);
        if (APP.ComboArMgr._objectsRightSide.ContainsKey(name0))
            APP.ComboArMgr._objectsRightSide.Remove(name0);
    }
}