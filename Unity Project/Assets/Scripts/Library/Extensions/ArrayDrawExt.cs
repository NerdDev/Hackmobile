using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class ArrayDrawExt
{
    public static void DrawCircle<T>(this T[,] arr, int x, int y, int radius, Action<int,int,T> action)
    {
    }

    public static void DrawCol<T>(this T[,] arr, int y1, int y2, int x, Action<int, int> action)
    {
        for (; y1 <= y2; y1++)
            action(x, y1);
    }

    public static void DrawRow<T>(this T[,] arr, int xl, int xr, int y, Action<int, int> action)
    {
        for (; xl <= xr; xl++)
            action(xl, y);
    }
}
