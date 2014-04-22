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

    public static bool IsLessThan(this Vector3 pos, Vector3 target)
    {
        if (pos.x > target.x) return false;
        if (pos.y > target.y) return false;
        if (pos.z > target.z) return false;
        return true;
    }
}
