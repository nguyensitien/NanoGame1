using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MathfExtension
{
    public static int FloorToIntAbs(float value)
    {
        if (value >= 0)
        {
            return Mathf.FloorToInt(value);
        }
        else
        {
            return Mathf.CeilToInt(value);
        }
    }

    public static int FloorToInt(float value) {
        float tmp = value - (int)value;
        Debug.Log("value:"+value+" tmp:"+tmp);
        if (tmp < 0) {
            return FloorToIntAbs(value);
        }
        if (tmp <= 0.5f)
        {
            return Mathf.FloorToInt(value);
        }
        else {
            return Mathf.CeilToInt(value);
        }
    }
}
