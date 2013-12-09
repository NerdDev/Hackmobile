using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

public static class ArrayExt
{
    public static Dictionary<Type, Func<object, char>> Converters;
    static ArrayExt()
    {
        Converters = new Dictionary<Type, Func<object, char>>();
        Converters.Add(typeof(bool), (b) =>
            {
                if ((bool)b) return 'X';
                else return ' ';
            });
    }

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

    static public List<string> ToRowStrings(this bool[,] array)
    {
        return ToRowStrings(array, (b) =>
            {
                if (b) return 'X';
                else return ' ';
            });
    }

    static public List<string> ToRowStrings<T>(this T[,] array, Func<T, char> converter = null)
    {
        if (converter == null)
        {
            Func<object, char> conv;
            if (!Converters.TryGetValue(typeof(T), out conv))
            {
                converter = new Func<T, char>((t) =>
                    {
                        string str = t.ToString();
                        return str.Length > 0 ? str[0] : ' ';
                    });
            }
            else
            {
                converter = new Func<T, char>((t) =>
                    {
                        return conv(t);
                    });
            }
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

    public static void ToLog<T>(this T[,] array, Func<T, char> converter, params string[] customContent)
    {
        foreach (string s in customContent)
        {
            BigBoss.Debug.w(s);
        }
        foreach (string s in array.ToRowStrings(converter))
        {
            BigBoss.Debug.w(s);
        }
    }

    public static void ToLog<T>(this T[,] array, params string[] customContent)
    {
        ToLog(array, null, customContent);
    }

    public static Point Center<T>(this T[,] array)
    {
        return new Point(array.GetLength(0) / 2, array.GetLength(1) / 2);
    }

    public static bool InRange<T>(this T[,] array, int x, int y)
    {
        return x >= 0 && y >= 0 && y < array.GetLength(0) && x < array.GetLength(1);
    }

    public static void Fill<T>(this T[,] array, T to)
    {
        for (int y = 0; y < array.GetLength(0); y++)
            for (int x = 0; x < array.GetLength(1); x++)
                array[y, x] = to;
    }

    public static T[,] Copy<T>(this T[,] array)
    {
        T[,] ret = new T[array.GetLength(0), array.GetLength(1)];
        Array.Copy(array, ret, array.Length);
        return ret;
    }
}
