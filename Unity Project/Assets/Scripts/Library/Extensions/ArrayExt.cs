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
        return new Bounding(0, arr.GetLength(1) - 1, 0, arr.GetLength(0) - 1);
    }

    public static T Random<T>(this T[] arr, System.Random rand)
    {
        if (arr.Length == 1) return arr[0];
        if (arr.Length > 0) return arr[rand.Next(arr.Length)];
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
        return ToRowStrings(array, null, (b) =>
            {
                if (b) return 'X';
                else return ' ';
            });
    }

    static private Func<T, char> GetConverter<T>()
    {
        Func<T, char> converter;
        Func<object, char> conv;
        Type type = typeof(T);
        if (!Converters.TryGetValue(type, out conv))
        {
            converter = new Func<T, char>((t) =>
            {
                if (t == null)
                    return ' ';
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
        return converter;
    }

    static public List<string> ToRowStrings<T>(this T[,] array, Bounding bounds = null, Func<T, char> converter = null)
    {
        if (converter == null)
        {
            converter = GetConverter<T>();
        }
        List<string> ret = new List<string>();
        if (bounds == null)
        {
            bounds = array.GetBounds();
        }
        for (int y = bounds.YMax; y >= bounds.YMin; y -= 1)
        {
            StringBuilder sb = new StringBuilder();
            for (int x = bounds.XMin; x <= bounds.XMax; x += 1)
            {
                sb.Append(converter(array[y, x]));
            }
            ret.Add(sb.ToString());
        }
        return ret;
    }

    static public List<string> ToRowStrings<T>(this T[,] array, Container2D<T> highlight, char highlightChar = '*', Bounding bounds = null, Func<T, char> converter = null)
    {
        if (converter == null)
        {
            converter = GetConverter<T>();
        }
        List<string> ret = new List<string>();
        if (bounds == null)
        {
            bounds = array.GetBounds();
        }
        for (int y = bounds.YMax; y >= bounds.YMin; y -= 1)
        {
            StringBuilder sb = new StringBuilder();
            for (int x = bounds.XMin; x <= bounds.XMax; x += 1)
            {
                if (highlight.Contains(x, y))
                {
                    sb.Append(highlightChar);
                }
                else
                {
                    sb.Append(converter(array[y, x]));
                }
            }
            ret.Add(sb.ToString());
        }
        return ret;
    }

    public static void ToLog<T>(this T[,] array, Logs log, Bounding bounding, Func<T, char> converter, params string[] customContent)
    {
        if (BigBoss.Debug.logging(log))
        {
            foreach (string s in customContent)
            {
                BigBoss.Debug.w(log, s);
            }
            foreach (string s in array.ToRowStrings(bounding, converter))
            {
                BigBoss.Debug.w(log, s);
            }
        }
    }

    public static void ToLog<T>(this T[,] array, Logs log, params string[] customContent)
    {
        ToLog(array, log, null, null, customContent);
    }

    public static void ToLog<T>(this T[,] array, Bounding bounds, Func<T, char> converter, params string[] customContent)
    {
        foreach (string s in customContent)
        {
            BigBoss.Debug.w(s);
        }
        foreach (string s in array.ToRowStrings(bounds, converter))
        {
            BigBoss.Debug.w(s);
        }
    }

    public static void ToLog<T>(this T[,] array, Bounding bounds, params string[] customContent)
    {
        ToLog(array, bounds, customContent);
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

    public static T[,] Expand<T>(this T[,] array, int buffer)
    {
        T[,] ret = new T[array.GetLength(0) + 2 * buffer, array.GetLength(1) + 2 * buffer];
        for (int y = 0; y < array.GetLength(0); y++)
        {
            for (int x = 0; x < array.GetLength(1); x++)
            {
                ret[y + buffer, x + buffer] = array[y, x];
            }
        }
        return ret;
    }
}
