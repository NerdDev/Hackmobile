using UnityEngine;
using System.Collections;

public static class ArrayExt {

    public static Bounding GetBounds<T>(this T[,] arr)
    {
        return new Bounding(0, arr.GetLength(1), 0, arr.GetLength(0));
    }
}