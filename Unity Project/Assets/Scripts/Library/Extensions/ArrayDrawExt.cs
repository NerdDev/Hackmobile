using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class ArrayDrawExt
{
    /*
     * Uses Bressenham's Midpoint Algo
     */
    public static void DrawCircle<T>(this T[,] arr, int centerX, int centerY, int radius, Action<T[,], int, int> fillAction, Action<T[,], int, int> strokeAction)
    {
        int radiusError = 3 - (2 * radius);
        int x = 0;
        int y = radius;
        int lastYWidth = 0;

        while (x <= y)
        {
            // Draw rows from center extending up/down

            strokeAction(arr, centerX - y, centerY + x);
            strokeAction(arr, centerX - y, centerY - x);
            strokeAction(arr, centerX + y, centerY + x);
            strokeAction(arr, centerX + y, centerY - x);
            strokeAction(arr, centerX - x, centerY + y);
            strokeAction(arr, centerX - x, centerY - y);
            strokeAction(arr, centerX + x, centerY + y);
            strokeAction(arr, centerX + x, centerY - y);
            arr.DrawRow(centerX - y + 1, centerX + y - 1, centerY + x, fillAction);
            arr.DrawRow(centerX - y + 1, centerX + y - 1, centerY - x, fillAction);
            if (radiusError < 0)
            {
                radiusError += (4 * x) + 6;
            }
            else
            {
                radiusError += 4 * (x - y) + 10;
                // Draw rows from top/bottom only when y is about to change.
                if (y != radius)
                {
                    arr.DrawRow(centerX - lastYWidth, centerX + lastYWidth, centerY + y, fillAction);
                    arr.DrawRow(centerX - lastYWidth, centerX + lastYWidth, centerY - y, fillAction);
                }
                y--;
                lastYWidth = x;
            }
            x++;
        }
    }

    public static void DrawCircle<T>(this T[,] arr, int centerX, int centerY, int radius, Action<T[,], int, int> fillAction)
    {
        int radiusError = 3 - (2 * radius);
        int x = 0;
        int y = radius;

        while (x <= y)
        {
            // Draw rows from center extending up/down
            arr.DrawRow(centerX - y, centerX + y, centerY + x, fillAction);
            arr.DrawRow(centerX - y, centerX + y, centerY - x, fillAction);
            if (radiusError < 0)
            {
                radiusError += (4 * x) + 6;
            }
            else
            {
                radiusError += 4 * (x - y) + 10;
                // Draw rows from top/bottom only when y is about to change.
                arr.DrawRow(centerX - x, centerX + x, centerY + y, fillAction);
                arr.DrawRow(centerX - x, centerX + x, centerY - y, fillAction);
                y--;
            }
            x++;
        }
    }

    public static void DrawCircle<T>(this T[,] arr, int centerX, int centerY, int radius, T setTo)
    {
        DrawCircle(arr, centerX, centerY, radius, SetTo(setTo));
    }

    public static void DrawCircle<T>(this T[,] arr, int centerX, int centerY, int radius, T setFill, T setStroke)
    {
        DrawCircle(arr, centerX, centerY, radius, SetTo(setFill), SetTo(setStroke));
    }

    public static void DrawCol<T>(this T[,] arr, int y1, int y2, int x, T setTo)
    {
        DrawCol(arr, y1, y2, x, SetTo(setTo));
    }

    public static void DrawCol<T>(this T[,] arr, int y1, int y2, int x, Action<T[,], int, int> action)
    {
        for (; y1 <= y2; y1++)
            action(arr, x, y1);
    }

    public static void DrawRow<T>(this T[,] arr, int xl, int xr, int y, T setTo)
    {
        DrawRow(arr, xl, xr, y, SetTo(setTo));
    }

    public static void DrawRow<T>(this T[,] arr, int xl, int xr, int y, Action<T[,], int, int> action)
    {
        for (; xl <= xr; xl++)
            action(arr, xl, y);
    }

    public static Action<T[,], int, int> SetTo<T>(T to)
    {
        return new Action<T[,], int, int>((arr, x, y) =>
        {
            arr[y, x] = to;
        });
    }
}
