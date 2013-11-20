using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class ArrayDrawExt
{
    #region Lines
    public static bool DrawLine<T>(this T[,] arr, int from, int to, int on, bool horizontal, DrawAction<T> action)
    {
        if (horizontal)
            return DrawRow(arr, from, to, on, action);
        else
            return DrawCol(arr, from, to, on, action);
    }

    public static bool DrawLine<T>(this T[,] arr, int from, int to, int on, bool horizontal, T setTo)
    {
        return arr.DrawLine(from, to, on, horizontal, SetTo(setTo));
    }

    public static bool DrawCol<T>(this T[,] arr, int y1, int y2, int x, T setTo)
    {
        return DrawCol(arr, y1, y2, x, SetTo(setTo));
    }

    public static bool DrawCol<T>(this T[,] arr, int y1, int y2, int x, DrawAction<T> action)
    {
        if (!action.RunInitialAction(arr)) return false;
        for (; y1 <= y2; y1++)
            if (!action.UnitAction(arr, x, y1)) return false;
        if (action.FinalAction != null)
        {
            return action.FinalAction(arr, new Bounding(x, x, y1, y2));
        }
        return true;
    }

    public static bool DrawRow<T>(this T[,] arr, int xl, int xr, int y, T setTo)
    {
        return DrawRow(arr, xl, xr, y, SetTo(setTo));
    }

    public static bool DrawRow<T>(this T[,] arr, int xl, int xr, int y, DrawAction<T> action)
    {
        if (!action.RunInitialAction(arr)) return false;
        for (; xl <= xr; xl++)
            if (!action.UnitAction(arr, xl, y)) return false;
        if (action.FinalAction != null)
        {
            return action.FinalAction(arr, new Bounding(xl, xr, y, y));
        }
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
    public static bool DrawCircle<T>(this T[,] arr, int centerX, int centerY, int radius, DrawAction<T> action)
    {
        var stroke = action.StrokeAction;
        if (stroke == null)
            return DrawCircleHelper.DrawCircleNoStroke(arr, centerX, centerY, radius, action);

        if (!action.RunInitialAction(arr)) return false;

        int radiusError = 3 - (2 * radius);
        int x = 0;
        int y = radius;
        int lastYWidth = 0;
        DrawAction<T> incrementalAction = action.OnlyIncremental();

        while (x <= y)
        {
            // Draw rows from center extending up/down

            if (!stroke(arr, centerX - y, centerY + x)) return false;
            if (!stroke(arr, centerX - y, centerY - x)) return false;
            if (!stroke(arr, centerX + y, centerY + x)) return false;
            if (!stroke(arr, centerX + y, centerY - x)) return false;
            if (!stroke(arr, centerX - x, centerY + y)) return false;
            if (!stroke(arr, centerX - x, centerY - y)) return false;
            if (!stroke(arr, centerX + x, centerY + y)) return false;
            if (!stroke(arr, centerX + x, centerY - y)) return false;
            if (!arr.DrawRow(centerX - y + 1, centerX + y - 1, centerY + x, incrementalAction)) return false;
            if (!arr.DrawRow(centerX - y + 1, centerX + y - 1, centerY - x, incrementalAction)) return false;
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
                    if (!arr.DrawRow(centerX - lastYWidth, centerX + lastYWidth, centerY + y, incrementalAction)) return false;
                    if (!arr.DrawRow(centerX - lastYWidth, centerX + lastYWidth, centerY - y, incrementalAction)) return false;
                }
                y--;
                lastYWidth = x;
            }
            x++;
        }
        if (action.FinalAction != null)
        {
            return action.FinalAction(arr, new Bounding(centerX - radius, centerX + radius, centerY - radius, centerY + radius)); 
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
        return DrawCircle(arr, centerX, centerY, radius, new DrawAction<T>()
            {
                UnitAction = SetTo(setFill),
                StrokeAction = SetTo(setStroke)
            });
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

    public static bool DrawCircle<T>(this T[,] arr, int radius, DrawAction<T> action)
    {
        Point center = arr.Center();
        return arr.DrawCircle(center.x, center.y, radius, action);
    }
    #endregion
    #endregion
    #region Squares
    public static bool DrawSquare<T>(this T[,] arr, int xl, int xr, int yb, int yt, DrawAction<T> action)
    {
        if (action.StrokeAction == null)
            return DrawSquareHelper.DrawSquare(arr, xl, xr, yb, yt, action);

        if (!action.RunInitialAction(arr)) return false;

        DrawAction<T> stroke = new DrawAction<T>() { UnitAction = action.StrokeAction };
        DrawAction<T> fill = new DrawAction<T>() { UnitAction = action.UnitAction };

        if (!arr.DrawRow(xl, xr, yb, stroke)) return false;
        if (!arr.DrawRow(xl, xr, yt, stroke)) return false;
        yb++;
        yt--;
        if (!arr.DrawCol(yb, yt, xl, stroke)) return false;
        if (!arr.DrawCol(yb, yt, xr, stroke)) return false;
        xl++;
        xr--;
        if (fill != null)
            if (!DrawSquareHelper.DrawSquare(arr, xl, xr, yb, yt, fill)) return false;
        return true;
    }

    #region Alternates
    public static bool DrawSquare<T>(this T[,] arr, int xl, int xr, int yb, int yt, T setTo)
    {
        return arr.DrawSquare(xl, xr, yb, yt, SetTo(setTo));
    }

    public static bool DrawSquare<T>(this T[,] arr, int xl, int xr, int yb, int yt, T fillTo, T strokeTo)
    {
        return arr.DrawSquare(xl, xr, yb, yt, new DrawAction<T>() 
        {
            UnitAction = SetTo(fillTo),
            StrokeAction = SetTo(strokeTo)
        });
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
    public static List<Bounding> GetSquares<T>(this T[,] arr, int width, int height, bool tryFlipped, DrawAction<T> tester, Bounding scope = null)
    {
        SquareFinder<T> finder = new SquareFinder<T>(arr, width, height, tryFlipped, tester, scope);
        return finder.Find();
    }
    #endregion
}

public class DrawCircleHelper
{
    public static bool DrawCircleNoStroke<T>(T[,] arr, int centerX, int centerY, int radius, DrawAction<T> action)
    {
        if (!action.RunInitialAction(arr)) return false;

        int radiusError = 3 - (2 * radius);
        int x = 0;
        int y = radius;
        DrawAction<T> incrementalAction = action.OnlyIncremental();

        while (x <= y)
        {
            // Draw rows from center extending up/down
            if (!arr.DrawRow(centerX - y, centerX + y, centerY + x, incrementalAction)) return false;
            if (!arr.DrawRow(centerX - y, centerX + y, centerY - x, incrementalAction)) return false;
            if (radiusError < 0)
            {
                radiusError += (4 * x) + 6;
            }
            else
            {
                radiusError += 4 * (x - y) + 10;
                // Draw rows from top/bottom only when y is about to change.
                if (!arr.DrawRow(centerX - x, centerX + x, centerY + y, incrementalAction)) return false;
                if (!arr.DrawRow(centerX - x, centerX + x, centerY - y, incrementalAction)) return false;
                y--;
            }
            x++;
        }
        if (action.FinalAction != null)
        {
            return action.FinalAction(arr, new Bounding(centerX - radius, centerX + radius, centerY - radius, centerY + radius));
        }
        return true;
    }
}

public class DrawSquareHelper
{
    public static bool DrawSquare<T>(T[,] arr, int xl, int xr, int yb, int yt, DrawAction<T> action)
    {
        if (!action.RunInitialAction(arr)) return false;
        DrawAction<T> incremental = action.OnlyIncremental();
        for (; yb <= yt; yb++)
            if (!arr.DrawRow(xl, xr, yb, incremental)) return false;
        if (action.FinalAction != null)
        {
            return action.FinalAction(arr, new Bounding(xl, xr, yb, yt));
        }
        return true;
    }
}
