using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGScaler : MonoBehaviour
{

    double screenDimesion;
    private void Start()
    {
        SetScaleFollowScreenSize();
    }

#if UNITY_EDITOR
    void Update()
    {
        double screenDimesionTmp = Utilities.GetScreenDimension();
        if (screenDimesion != screenDimesionTmp)
        {
            SetScaleFollowScreenSize();
        }
    }
#endif

    void SetScaleFollowScreenSize()
    {

        float ratio = (Utilities.SIZE_HEIGHT * Screen.width) / (Utilities.SIZE_WIDTH * Screen.height);
        var sizeScene = Utilities.GetScreenDimension();

        if (sizeScene >= 2f || Utilities.SIZE_HEIGHT < Screen.height)
            ratio = 1.0F / ratio;

        transform.localScale = Vector3.one * ratio;

    }
    
}
