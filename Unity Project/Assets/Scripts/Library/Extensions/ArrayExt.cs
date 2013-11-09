using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

public static class ArrayExt {

    public static Bounding GetBounds<T>(this T[,] arr)
    {
        return new Bounding(0, arr.GetLength(1), 0, arr.GetLength(0));
    }

    public static T Random<T>(this T[] arr, System.Random rand)
    {
        if (arr.Length > 0)
            return arr[rand.Next(arr.Length)];
        return default(T);
    }

    static public bool Contains<T>(this T[] arr, T val)
    {
        foreach (T t in arr)
        {
            if (t.Equals(val))
            {
                return true;
            }
        }
        return false;
    }

    static public List<string> ToRowStrings<T>(this T[,] array, Func<T, char> converter = null)
    {
        if (converter == null)
        {
            converter = new Func<T, char>((t) =>
                {
                    string str = t.ToString();
                    return str.Length > 0 ? str[0] : ' ';
                });
        }
        List<string> ret = new List<string>();
        for (int y = array.GetLength(0) - 1; y >= 0; y -= 1)
        {
            StringBuilder sb = new StringBuilder();
            for (int x = 0; x < array.GetLength(1); x += 1)
            {
                sb.Append(converter(array[y, x]));
            }
            ret.Add(sb.ToString());
        }
        return ret;
    }

    public static void ToLog<T>(this T[,] array, Logs log, Func<T, char> converter, params string[] customContent)
    {
        if (BigBoss.Debug.logging(log))
        {
            foreach (string s in customContent)
            {
                BigBoss.Debug.w(log, s);
            }
            foreach (string s in array.ToRowStrings(converter))
            {
                BigBoss.Debug.w(log, s);
            }
        }
    }

    public static void ToLog<T>(this T[,] array, Logs log, params string[] customContent)
    {
        ToLog(array, log, null, customContent);
    }
}
