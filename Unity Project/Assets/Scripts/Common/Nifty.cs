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
}

