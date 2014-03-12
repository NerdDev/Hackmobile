using UnityEngine;
using System.Collections;
using System;

public static class StringExt {

    public static bool ToEnum<T>(this string str, out T e) where T : struct, IComparable, IConvertible
    {
        try
        {
            e = (T)Enum.Parse(typeof(T), str, true);
            return true;
        }
        catch (Exception)
        {
            e = default(T);
            return false;
        }
    }

    public static int GetHash(this string str)
    {
        return str.GetHashCode();
    }
}
