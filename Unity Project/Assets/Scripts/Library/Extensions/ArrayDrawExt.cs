using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class ArrayDrawExt
{
    public static void DrawCircle<T>(this T[,] arr, int centerX, int centerY, int radius, Action<int, int> fillAction, Action<int, int> strokeAction)
    {
        int x = radius, y = 0;
        int radiusError = 1 - x;

        while (x + 2 >= y)
        {
            if (x >= y)
            {
                strokeAction(x + centerX, y + centerY);
                strokeAction(y + centerX, x + centerY);
                strokeAction(-x + centerX, y + centerY);
                strokeAction(-y + centerX, x + centerY);
                strokeAction(-x + centerX, -y + centerY);
                strokeAction(-y + centerX, -x + centerY);
                strokeAction(x + centerX, -y + centerY);
                strokeAction(y + centerX, -x + centerY);
            }

            arr.DrawCol(centerY - x + 1, centerY + x - 1, y + centerX, fillAction);
            arr.DrawCol(centerY - x + 1, centerY + x - 1, centerX - y, fillAction);

            y++;

            if (radiusError < 0) radiusError += 2 * y + 1;
            else
            {
                x--;
                radiusError += 2 * (y - x + 1);
            }
        }
    }

    public static void DrawCircle<T>(this T[,] arr, int centerX, int centerY, int radius, Action<int, int> fillAction)
    {
        int x = radius, y = 0;
        int radiusError = 1 - x;
        while (x >= y)
        {
            arr.DrawCol(centerY - x, centerY + x, y + centerX, fillAction);
            arr.DrawCol(centerY - x, centerY + x, centerX - y, fillAction);

            y++;

            if (radiusError < 0) radiusError += 2 * y + 1;
            else
            {
                x--;
                radiusError += 2 * (y - x + 1);
            }
        }
        arr.DrawCol(centerY - x, centerY + x, y + centerX, fillAction);
        arr.DrawCol(centerY - x, centerY + x, y + centerX, fillAction);
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
