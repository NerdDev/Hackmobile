using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class ArrayDrawExt
{
    #region Basics
    public static bool DrawCol<T>(this T[,] arr, int y1, int y2, int x, T setTo)
    {
        return DrawCol(arr, y1, y2, x, SetTo(setTo));
    }

    public static bool DrawCol<T>(this T[,] arr, int y1, int y2, int x, Func<T[,], int, int, bool> action)
    {
        for (; y1 <= y2; y1++)
            if (!action(arr, x, y1)) return false;
        return true;
    }

    public static bool DrawRow<T>(this T[,] arr, int xl, int xr, int y, T setTo)
    {
        return DrawRow(arr, xl, xr, y, SetTo(setTo));
    }

    public static bool DrawRow<T>(this T[,] arr, int xl, int xr, int y, Func<T[,], int, int, bool> action)
    {
        for (; xl <= xr; xl++)
            if (!action(arr, xl, y)) return false;
        return true;
    }

    public static Func<T[,], int, int, bool> SetTo<T>(T to)
    {
        return new Func<T[,], int, int, bool>((arr, x, y) =>
        {
            arr[y, x] = to;
            return true;
        });
    }
    #endregion
    #region Circles
    /*
     * Uses Bressenham's Midpoint Algo
     */
    public static bool DrawCircle<T>(this T[,] arr, int centerX, int centerY, int radius, Func<T[,], int, int, bool> fillAction, Func<T[,], int, int, bool> strokeAction)
    {
        if (strokeAction == null)
            return DrawCircle(arr, centerX, centerY, radius, fillAction);

        int radiusError = 3 - (2 * radius);
        int x = 0;
        int y = radius;
        int lastYWidth = 0;

        while (x <= y)
        {
            // Draw rows from center extending up/down

            if (!strokeAction(arr, centerX - y, centerY + x)) return false;
            if (!strokeAction(arr, centerX - y, centerY - x)) return false;
            if (!strokeAction(arr, centerX + y, centerY + x)) return false;
            if (!strokeAction(arr, centerX + y, centerY - x)) return false;
            if (!strokeAction(arr, centerX - x, centerY + y)) return false;
            if (!strokeAction(arr, centerX - x, centerY - y)) return false;
            if (!strokeAction(arr, centerX + x, centerY + y)) return false;
            if (!strokeAction(arr, centerX + x, centerY - y)) return false;
            if (!arr.DrawRow(centerX - y + 1, centerX + y - 1, centerY + x, fillAction)) return false;
            if (!arr.DrawRow(centerX - y + 1, centerX + y - 1, centerY - x, fillAction)) return false;
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
                    if (!arr.DrawRow(centerX - lastYWidth, centerX + lastYWidth, centerY + y, fillAction)) return false;
                    if (!arr.DrawRow(centerX - lastYWidth, centerX + lastYWidth, centerY - y, fillAction)) return false;
                }
                y--;
                lastYWidth = x;
            }
            x++;
        }
        return true;
    }

    public static bool DrawCircle<T>(this T[,] arr, int centerX, int centerY, int radius, Func<T[,], int, int, bool> fillAction)
    {
        int radiusError = 3 - (2 * radius);
        int x = 0;
        int y = radius;

        while (x <= y)
        {
            // Draw rows from center extending up/down
            if (!arr.DrawRow(centerX - y, centerX + y, centerY + x, fillAction)) return false;
            if (!arr.DrawRow(centerX - y, centerX + y, centerY - x, fillAction)) return false;
            if (radiusError < 0)
            {
                radiusError += (4 * x) + 6;
            }
            else
            {
                radiusError += 4 * (x - y) + 10;
                // Draw rows from top/bottom only when y is about to change.
                if (!arr.DrawRow(centerX - x, centerX + x, centerY + y, fillAction)) return false;
                if (!arr.DrawRow(centerX - x, centerX + x, centerY - y, fillAction)) return false;
                y--;
            }
            x++;
        }
        return true;
    }

    #region Alternates
    public static bool DrawCircle<T>(this T[,] arr, int centerX, int centerY, int radius, T setTo)
    {
        return DrawCircle(arr, centerX, centerY, radius, SetTo(setTo));
    }

    public static bool DrawCircle<T>(this T[,] arr, int centerX, int centerY, int radius, T setFill, T setStroke)
    {
        return DrawCircle(arr, centerX, centerY, radius, SetTo(setFill), SetTo(setStroke));
    }

    public static bool DrawCircle<T>(this T[,] arr, int radius, T setTo)
    {
        Point center = arr.Center();
        return arr.DrawCircle(center.x, center.y, radius, setTo);
    }

    public static bool DrawCircle<T>(this T[,] arr, int radius, T setFill, T strokeFill)
    {
        Point center = arr.Center();
        return arr.DrawCircle(center.x, center.y, radius, setFill, strokeFill);
    }

    public static bool DrawCircle<T>(this T[,] arr, int radius, Func<T[,], int, int, bool> action)
    {
        Point center = arr.Center();
        return arr.DrawCircle(center.x, center.y, radius, action);
    }

    public static bool DrawCircle<T>(this T[,] arr, int radius, Func<T[,], int, int, bool> fillAction, Func<T[,], int, int, bool> strokeAction)
    {
        Point center = arr.Center();
        return arr.DrawCircle(center.x, center.y, radius, fillAction, strokeAction);
    }
    #endregion
    #endregion
    #region Squares
    public static bool DrawSquare<T>(this T[,] arr, int xl, int xr, int yb, int yt, Func<T[,], int, int, bool> action)
    {
        for (; yb <= yt; yb++)
            if (!arr.DrawRow(xl, xr, yb, action)) return false;
        return true;
    }

    public static bool DrawSquare<T>(this T[,] arr, int xl, int xr, int yb, int yt, Func<T[,], int, int, bool> fillAction, Func<T[,], int, int, bool> strokeAction)
    {
        if (strokeAction == null)
            return DrawSquare(arr, xl, xr, yb, yt, fillAction);
        if (!arr.DrawRow(xl, xr, yb, strokeAction)) return false;
        if (!arr.DrawRow(xl, xr, yt, strokeAction)) return false;
        yb++;
        yt--;
        if (!arr.DrawCol(yb, yt, xl, strokeAction)) return false;
        if (!arr.DrawCol(yb, yt, xr, strokeAction)) return false;
        xl++;
        xr--;
        if (fillAction != null)
            if (!arr.DrawSquare(xl, xr, yb, yt, fillAction)) return false;
        return true;
    }

    #region Alternates
    public static bool DrawSquare<T>(this T[,] arr, int xl, int xr, int yb, int yt, T setTo)
    {
        return arr.DrawSquare(xl, xr, yb, yt, SetTo(setTo));
    }

    public static bool DrawSquare<T>(this T[,] arr, int xl, int xr, int yb, int yt, T fillTo, T strokeTo)
    {
        return arr.DrawSquare(xl, xr, yb, yt, SetTo(fillTo), SetTo(strokeTo));
    }

    public static bool DrawSquare<T>(this T[,] arr, int width, int height, T setTo)
    {
        Point center = arr.Center();
        int left = center.x - width / 2;
        int right = center.x + width / 2;
        if (width % 2 == 0) right--;
        int bottom = center.y - height / 2;
        int top = center.y + height / 2;
        if (height % 2 == 0) top--;
        return arr.DrawSquare(left, right, bottom, top, setTo);
    }

    public static bool DrawSquare<T>(this T[,] arr, int width, int height, T setFill, T setStroke)
    {
        Point center = arr.Center();
        int left = center.x - width / 2;
        int right = center.x + width / 2;
        if (width % 2 == 0) right--;
        int bottom = center.y - height / 2;
        int top = center.y + height / 2;
        if (height % 2 == 0) top--;
        return arr.DrawSquare(left, right, bottom, top, setFill, setStroke);
    }
    #endregion
    #endregion
    #region Find
    public static List<Bounding> GetSquares<T>(this T[,] arr, int width, int height, bool tryFlipped, DrawTest<T> tester, Bounding scope = null)
    {
        SquareFinder<T> finder = new SquareFinder<T>(arr, width, height, tryFlipped, tester, scope);
        return finder.Find();
    }
    #endregion
}
