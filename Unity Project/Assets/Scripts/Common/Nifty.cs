using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.Threading;
using System.Reflection;

public static class Nifty
{
    static public int GCD(int a, int b)
    {
        a = Math.Abs(a);
        b = Math.Abs(b);
        while (b != 0)
        {
            int tmp = b;
            b = a % b;
            a = tmp;
        }
        return a;
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

    static public List<string> ToRowStrings<T>(this T[,] array)
    {
        List<string> ret = new List<string>();
        for (int y = array.GetLength(0) - 1; y >= 0; y -= 1)
        {
            string rowStr = "";
            for (int x = 0; x < array.GetLength(1); x += 1)
            {
                rowStr += array[y, x].ToString()[0];
            }
            ret.Add(rowStr);
        }
        return ret;
    }

    public static void ToLog<T>(this T[,] array, DebugManager.Logs log, params string[] customContent)
    {
        if (BigBoss.Debug.logging(log))
        {
            foreach (string s in customContent)
            {
                BigBoss.Debug.w(log, s);
            }
            foreach (string s in array.ToRowStrings())
            {
                BigBoss.Debug.w(log, s);
            }
        }
    }

    static public void DeleteContainedFiles(String dirPath, bool recursive)
    {
        DeleteContainedFiles(new DirectoryInfo(dirPath), recursive);
    }

    static public void DeleteContainedFiles(DirectoryInfo dir, bool recursive)
    {
        if (dir.Exists)
        {
            FileInfo[] files = dir.GetFiles("*.*");
            foreach (FileInfo fi in files)
            {
                fi.Delete();
            }
            if (recursive)
            {
                foreach (DirectoryInfo subDir in dir.GetDirectories())
                {
                    DeleteContainedFiles(subDir, recursive);
                }
            }
        }
    }

    /**
     * Grabs a set of random values off a dictionary.
     * 
     * Usage: List<V> values = (List<V>) RandomValues(someDictionary).Take(someNumberOfElements);
     * K = key, V = value.
     */
    public static IEnumerable<V> RandomValues<K, V>(IDictionary<K, V> dict)
    {
        System.Random rand = new System.Random();
        List<V> values = Enumerable.ToList(dict.Values);
        int size = dict.Count;
        while (true)
        {
            yield return values[rand.Next(size)];
        }
    }

    /**
     * Grabs one random value off of a dictionary.
     * 
     * Usage: V value = (V) RandomValue(someDictionary);
     * K = key, V = value.
     */
    public static V RandomValue<K, V>(IDictionary<K, V> dict)
    {
        System.Random rand = new System.Random();
        List<V> values = Enumerable.ToList(dict.Values);
        return values[rand.Next(dict.Count)];
    }

    public static T StringToEnum<T>(string toParse)
    {
        return (T)Enum.Parse(typeof(T), toParse, true);
    }

    public static List<T> Populate<T>(this List<T> list, int num) where T : new()
    {
        for (int i = 0; i < num; i++)
        {
            list.Add(new T());
        }
        return list;
    }

    public static T Take<T>(this List<T> list)
    {
        T item = list[0];
        list.RemoveAt(0);
        return item;
    }

    public static T RandomTake<T>(this List<T> list, System.Random rand)
    {
        int r = rand.Next(list.Count);
        T item = list[r];
        list.RemoveAt(r);
        return item;
    }

    public static T Random<T>(this List<T> list, System.Random rand)
    {
        return list[rand.Next(list.Count)];
    }

    public static bool Percent(this System.Random rand, int percent)
    {
        return percent > rand.Next(100);
    }

    public static int ToInt(this float x)
    {
        return Convert.ToInt32(x);
    }

    static public int ToInt(this string toParse)
    {
        int temp;
        if (int.TryParse(toParse, out temp))
        {
            return temp;
        }
        else
        {
            throw new ArgumentException("String cannot be parsed to integer!");
        }
    }

    static public double ToDouble(this string toParse)
    {
        double temp;
        if (double.TryParse(toParse, out temp))
        {
            return temp;
        }
        else
        {
            throw new ArgumentException("String cannot be parsed to double!");
        }
    }

    static public float ToFloat(this string toParse)
    {
        float temp;
        if (float.TryParse(toParse, out temp))
        {
            return temp;
        }
        else
        {
            throw new ArgumentException("String cannot be parsed to double!");
        }
    }

    static public bool ToBool(this string toParse)
    {
        bool temp;
        if (bool.TryParse(toParse, out temp))
        {
            return temp;
        }
        else
        {
            throw new ArgumentException("String cannot be parsed to boolean!");
        }
    }

    public static float Round(this float f)
    {
        return Mathf.Round(f);
    }

    public static bool diagonal(int firstx, int firsty, int secondx, int secondy)
    {
        if (Math.Abs(firstx - secondx) == 1 && Math.Abs(firsty - secondy) == 1)
        {
            return true;
        }
        return false;
    }

    public static List<Type> GetSubclassesOf(this Type type)
    {
        return AppDomain.CurrentDomain.GetAssemblies().ToList()
            .SelectMany(s => s.GetTypes())
            .Where(p => type.IsAssignableFrom(p) && p.IsClass && !p.IsAbstract && !p.IsInterface).ToList();
    }
}

