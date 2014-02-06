using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public delegate bool DrawAction<T>(Container2D<T> arr, int x, int y);
public static class DrawActionExt
{
    public static DrawAction<T> And<T>(this DrawAction<T> call, DrawAction<T> rhs)
    {
        return (arr, x, y) =>
        {
            return call(arr, x, y) && rhs(arr, x, y);
        };
    }

    public static DrawAction<T> And<T>(this DrawAction<T> call, params DrawAction<T>[] rhs)
    {
        if (rhs.Length == 0) return call;
        if (rhs.Length == 1) return And(rhs[0]);
        return (arr, x, y) =>
        {
            if (!call(arr, x, y)) return false;
            for (int i = 0; i < rhs.Length; i++)
                if (!rhs[i](arr, x, y)) return false;
            return true;
        };
    }

    public static DrawAction<T> IfThen<T>(this DrawAction<T> call, DrawAction<T> rhs)
    {
        return (arr, x, y) =>
        {
            if (call(arr, x, y))
                return rhs(arr, x, y);
            return true;
        };
    }

    public static DrawAction<T> IfNotThen<T>(this DrawAction<T> call, DrawAction<T> rhs)
    {
        return (arr, x, y) =>
        {
            if (!call(arr, x, y))
                return rhs(arr, x, y);
            return true;
        };
    }

    public static DrawAction<T> AndNot<T>(this DrawAction<T> call, DrawAction<T> call2)
    {
        return (arr, x, y) =>
        {
            return call(arr, x, y) && !call2(arr, x, y);
        };
    }

    public static DrawAction<T> Or<T>(this DrawAction<T> call, DrawAction<T> call2)
    {
        return (arr, x, y) =>
        {
            return call(arr, x, y) || call2(arr, x, y);
        };
    }

    public static DrawAction<T> OrNot<T>(this DrawAction<T> call, DrawAction<T> call2)
    {
        return (arr, x, y) =>
        {
            return call(arr, x, y) || !call2(arr, x, y);
        };
    }

    public static DrawAction<T> Shift<T>(this DrawAction<T> call, int shiftX, int shiftY)
    {
        return (arr, x, y) =>
        {
            return call(arr, x + shiftX, y + shiftY);
        };
    }
}
