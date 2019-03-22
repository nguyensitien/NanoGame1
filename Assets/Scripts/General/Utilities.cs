using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities 
{
    public static float GetRatioDevice()
    {
        return (float)Screen.height / (float)Screen.width;
    }

    public static float ConvertToLocal(float value)
    {
        return value;
    }

    public static bool CheckOverrideRectPoint(Rect rect, Vector2 p)
    {
        return p.x >= rect.x - rect.width / 2 &&
            p.x <= rect.x + rect.width / 2 &&
            p.y >= rect.y - rect.height / 2 &&
            p.y <= rect.y + rect.height / 2;
    }
}
