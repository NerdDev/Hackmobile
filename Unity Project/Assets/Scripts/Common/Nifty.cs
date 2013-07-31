using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

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
		for (int y = array.GetLength(0) - 1; y >= 0; y -= 1) {
            string rowStr = "";
    		for (int x = 0; x < array.GetLength(1); x += 1) {
                rowStr += array[y, x].ToString()[0];
    		}
            ret.Add(rowStr);
		}
        return ret;
    }
   
    public static void ToLog<T>(this T[,] array, DebugManager.Logs log, params string[] customContent)
    {
        if (DebugManager.logging(log))
        {
            foreach (string s in customContent)
            {
                DebugManager.w(log, s);
            }
            foreach (string s in array.ToRowStrings())
            {
                DebugManager.w(log, s);
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
}

