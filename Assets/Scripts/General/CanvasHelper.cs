using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasScaler))]
public class CanvasHelper : MonoBehaviour {


#if UNITY_EDITOR
    int preWidth, preHeight;
#endif

    void Awake()
    {
        var size3_2 = 2f;
        var sizeScene = Utilities.GetScreenDimension();
        var isMatchHeight = sizeScene >= size3_2;
        var canvas = GetComponent<CanvasScaler>();
        canvas.matchWidthOrHeight = isMatchHeight ? 1 : 0;
    }

#if UNITY_EDITOR
    void Update()
    {
        if (preWidth != Screen.width || preHeight != Screen.height)
        {
            preWidth = Screen.width;
            preHeight = Screen.height;
            Awake();
        }
    }
#endif
}
