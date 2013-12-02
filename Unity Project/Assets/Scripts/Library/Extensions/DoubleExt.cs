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
}
