using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

public static class Vector3Ext
{
    public static bool checkXYPosition(this Vector3 playPos, Vector3 target)
    {
        if (Math.Abs(playPos.x - target.x) > .08f ||
            Math.Abs(playPos.z - target.z) > .08f)
        {
            return false;
        }
        return true;
    }
}
