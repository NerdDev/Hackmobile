using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class Nifty
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

    static public int StringToInt(string toParse)
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

    static public bool StringToBool(string toParse)
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
}

