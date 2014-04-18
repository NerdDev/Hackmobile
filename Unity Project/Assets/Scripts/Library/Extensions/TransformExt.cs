using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

public static class TransformExt
{
    public static void Reset(this Transform trans)
    {
        trans.localPosition = Vector3.zero;
        trans.localRotation = Quaternion.identity;
        trans.localScale = Vector3.one;
    }
}
