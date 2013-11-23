using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DrawPresets
{
    public static DrawAction<T> EqualTo<T>(T t)
    {
        return new DrawAction<T>((arr, x, y) =>
        {
            return t.Equals(arr[y, x]);
        });
    }

    public static DrawAction<T> SetTo<T>(T g)
    {
        return new DrawAction<T>((arr, x, y) =>
        {
            arr[y, x] = g;
            return true;
        });
    }

    public static DrawAction<T> NotEdgeOfArray<T>()
    {
        return new DrawAction<T>((arr, x, y) =>
        {
            if (x <= 0
                || y <= 0
                || y >= arr.GetLength(0) - 1
                || x >= arr.GetLength(1) - 1) return false;
            return true;
        });
    }
}
