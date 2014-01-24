using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

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

    public static bool Percent(this System.Random rand, Percent percent)
    {
        return percent.Float > rand.NextDouble();
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

    public static List<string> AddRuler(List<string> list, int xLowerBound = 0, int yLowerBound = 0)
    {
        List<string> ret = new List<string>();
        int yWidth = list.Count.ToString().Length + 1;
        int xLength = 0, xHeight;
        // Add Left vert ruler
        yLowerBound--; // To offset ruler properly
        foreach (string orig in list)
        {
            string num = (list.Count + yLowerBound--).ToString();
            ret.Add(num.PadRight(yWidth, ' ') + orig);
            xLength = Math.Max(xLength, orig.Length);
        }
        ret.Add("");
        // Add Bottom horiz ruler
        xHeight = (xLength + xLowerBound).ToString().Length + 1;
        string pad = "".PadRight(yWidth, ' ');
        for (int y = 0; y < xHeight; y++)
        {
            string line = "";
            for (int x = 0; x < xLength; x++)
            {
                string num = (x + xLowerBound).ToString();
                line += num.Length > y ? num[y] : ' ';
            }
            ret.Add(pad + line);
        }
        return ret;
    }

    public static List<string> AddRuler(List<string> list, Bounding bounds)
    {
        if (bounds != null && bounds.IsValid())
            return AddRuler(list, bounds.XMin, bounds.YMin);
        return AddRuler(list);
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

