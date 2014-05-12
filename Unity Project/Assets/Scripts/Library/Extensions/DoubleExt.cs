using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class DoubleExt
{
    public static double Modulo(this double a, double b)
    {
        return a - Math.Floor(a / b) * b;
    }

    public static bool EqualsWithin(this double a, double b, double within = 0.000000001d)
    {
        return Math.Abs(a - b) < within;
    }
}
