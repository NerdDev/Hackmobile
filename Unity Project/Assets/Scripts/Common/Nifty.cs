using System;

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

