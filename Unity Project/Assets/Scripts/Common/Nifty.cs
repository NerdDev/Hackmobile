using System;
using System.IO;

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
}

