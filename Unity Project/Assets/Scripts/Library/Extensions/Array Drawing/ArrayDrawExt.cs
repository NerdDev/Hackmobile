using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public delegate bool DrawAction<T>(T[,] arr, int x, int y);
public delegate bool DrawEval<T>(T t);

public static class ArrayDrawExt
{
    #region Point
    public static IEnumerable<T> DrawAround<T>(this T[,] arr, int x, int y, bool cornered)
    {
        yield return arr[y + 1, x];
        yield return arr[y - 1, x];
        yield return arr[y, x + 1];
        yield return arr[y, x - 1];
        if (cornered)
        {
            yield return arr[y + 1, x + 1];
            yield return arr[y - 1, x + 1];
            yield return arr[y + 1, x - 1];
            yield return arr[y - 1, x - 1];
        }
    }

    public static bool DrawAround<T>(this T[,] arr, int x, int y, bool cornered, DrawAction<T> action)
    {
        if (!action(arr, x, y + 1)) return false;
        if (!action(arr, x, y - 1)) return false;
        if (!action(arr, x + 1, y)) return false;
        if (!action(arr, x - 1, y)) return false;
        if (cornered)
        {
            if (!action(arr, x + 1, y + 1)) return false;
            if (!action(arr, x + 1, y - 1)) return false;
            if (!action(arr, x - 1, y + 1)) return false;
            if (!action(arr, x - 1, y - 1)) return false;
        }
        return true;
    }

    public static bool DrawDirs<T>(this T[,] arr, int x, int y, GridDirection dir, DrawAction<T> action)
    {
        switch (dir)
        {
            case GridDirection.HORIZ:
                if (!action(arr, x - 1, y)) return false;
                if (!action(arr, x + 1, y)) return false;
                break;
            case GridDirection.VERT:
                if (!action(arr, x, y + 1)) return false;
                if (!action(arr, x, y - 1)) return false;
                break;
            case GridDirection.DIAGTLBR:
                if (!action(arr, x - 1, y + 1)) return false;
                if (!action(arr, x + 1, y - 1)) return false;
                break;
            case GridDirection.DIAGBLTR:
                if (!action(arr, x - 1, y - 1)) return false;
                if (!action(arr, x + 1, y + 1)) return false;
                break;
        }
        return true;
    }

    public static bool DrawLocation<T>(this T[,] arr, int x, int y, GridLocation loc, DrawAction<T> action)
    {
        switch (loc)
        {
            case GridLocation.BOTTOMLEFT:
                if (!action(arr, x - 1, y - 1)) return false;
                break;
            case GridLocation.BOTTOMRIGHT:
                if (!action(arr, x + 1, y - 1)) return false;
                break;
            case GridLocation.DOWN:
                if (!action(arr, x, y - 1)) return false;
                break;
            case GridLocation.LEFT:
                if (!action(arr, x - 1, y)) return false;
                break;
            case GridLocation.RIGHT:
                if (!action(arr, x + 1, y)) return false;
                break;
            case GridLocation.TOPLEFT:
                if (!action(arr, x - 1, y + 1)) return false;
                break;
            case GridLocation.TOPRIGHT:
                if (!action(arr, x + 1, y + 1)) return false;
                break;
            case GridLocation.UP:
                if (!action(arr, x, y + 1)) return false;
                break;
        }
        return true;
    }

    public static List<Value2D<T>> GetAllAround<T>(this T[,] arr, int x, int y, bool cornered, DrawAction<T> tester)
    {
        List<Value2D<T>> ret = new List<Value2D<T>>();
        arr.DrawAround(x, y, cornered, new DrawAction<T>((arr2, x2, y2) =>
        {
            if (tester(arr2, x2, y2))
                ret.Add(new Value2D<T>(x2, y2, arr[y2, x2]));
            return true;
        }));
        return ret;
    }

    public static List<T> GetAllAround<T>(this T[,] arr, int x, int y, bool cornered, DrawEval<T> tester)
    {
        List<T> ret = new List<T>();
        arr.DrawAround(x, y, cornered, (arr2, x2, y2) =>
        {
            T t = arr2[y2, x2];
            if (tester(t))
                ret.Add(t);
            return true;
        });
        return ret;
    }

    public static Value2D<T> GetAround<T>(this T[,] arr, int x, int y, bool cornered, DrawAction<T> tester)
    {
        Value2D<T> ret = null;
        arr.DrawAround(x, y, cornered, new DrawAction<T>((arr2, x2, y2) =>
        {
            if (tester(arr2, x2, y2))
            {
                ret = new Value2D<T>(x2, y2, arr[y2, x2]);
                return false;
            }
            return true;
        }));
        return ret;
    }

    public static T GetAround<T>(this T[,] arr, int x, int y, bool cornered, DrawEval<T> tester)
    {
        T ret = default(T);
        arr.DrawAround(x, y, cornered, new DrawAction<T>((arr2, x2, y2) =>
        {
            ret = arr2[y2, x2];
            if (tester(ret))
                return false;
            return true;
        }));
        return ret;
    }

    public static List<Value2D<T>> GetAllDirs<T>(this T[,] arr, int x, int y, GridDirection dir, DrawAction<T> tester)
    {
        List<Value2D<T>> ret = new List<Value2D<T>>();
        arr.DrawDirs(x, y, dir, new DrawAction<T>((arr2, x2, y2) =>
        {
            if (tester(arr2, x2, y2))
                ret.Add(new Value2D<T>(x2, y2, arr[y2, x2]));
            return true;
        }));
        return ret;
    }

    public static List<T> GetAllDirs<T>(this T[,] arr, int x, int y, GridDirection dir, DrawEval<T> tester)
    {
        List<T> ret = new List<T>();
        arr.DrawDirs(x, y, dir, new DrawAction<T>((arr2, x2, y2) =>
        {
            T t = arr2[y2, x2];
            if (tester(t))
                ret.Add(t);
            return true;
        }));
        return ret;
    }

    public static Value2D<T> GetDir<T>(this T[,] arr, int x, int y, GridDirection dir, DrawAction<T> tester)
    {
        Value2D<T> ret = null;
        arr.DrawDirs(x, y, dir, new DrawAction<T>((arr2, x2, y2) =>
        {
            if (tester(arr2, x2, y2))
            {
                ret = new Value2D<T>(x2, y2, arr[y2, x2]);
                return false;
            }
            return true;
        }));
        return ret;
    }

    public static T GetDir<T>(this T[,] arr, int x, int y, GridDirection dir, DrawEval<T> tester)
    {
        T ret = default(T);
        arr.DrawDirs(x, y, dir, new DrawAction<T>((arr2, x2, y2) =>
        {
            ret = arr2[y2, x2];
            if (tester(ret))
                return false;
            return true;
        }));
        return ret;
    }

    public static bool Alternates<T>(this T[,] arr, int x, int y, Func<T, bool> evaluator)
    {
        bool pass = evaluator(arr[y, x - 1]);
        if (pass != evaluator(arr[y, x + 1])) return false;
        if (pass == evaluator(arr[y + 1, x])) return false;
        if (pass == evaluator(arr[y - 1, x])) return false;
        return true;
    }

    public static bool Cornered<T>(this T[,] arr, int x, int y, Func<T, bool> evaluator)
    {
        bool pass = evaluator(arr[y, x - 1]);
        if (pass == evaluator(arr[y, x + 1])) return false;
        pass = evaluator(arr[y - 1, x]);
        if (pass == evaluator(arr[y + 1, x])) return false;
        return true;
    }
    #endregion
    #region Lines
    public static bool DrawLine<T>(this T[,] arr, int from, int to, int on, bool horizontal, DrawActions<T> action)
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

    public static bool DrawCol<T>(this T[,] arr, int y1, int y2, int x, DrawActions<T> action)
    {
        for (; y1 <= y2; y1++)
            if (!action.UnitAction(arr, x, y1)) return false;
        return true;
    }

    public static bool DrawRow<T>(this T[,] arr, int xl, int xr, int y, T setTo)
    {
        return DrawRow(arr, xl, xr, y, SetTo(setTo));
    }

    public static bool DrawRow<T>(this T[,] arr, int xl, int xr, int y, DrawActions<T> action)
    {
        for (; xl <= xr; xl++)
            if (!action.UnitAction(arr, xl, y)) return false;
        return true;
    }

    public static DrawAction<T> SetTo<T>(T to)
    {
        return new DrawAction<T>((arr, x, y) =>
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
    public static bool DrawCircle<T>(this T[,] arr, int centerX, int centerY, int radius, DrawActions<T> action)
    {
        var stroke = action.StrokeAction;
        if (stroke == null)
            return DrawCircleHelper.DrawCircleNoStroke(arr, centerX, centerY, radius, action);

        int radiusError = 3 - (2 * radius);
        int x = 0;
        int y = radius;
        int lastYWidth = 0;

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
            if (!arr.DrawRow(centerX - y + 1, centerX + y - 1, centerY + x, action)) return false;
            if (!arr.DrawRow(centerX - y + 1, centerX + y - 1, centerY - x, action)) return false;
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
                    if (!arr.DrawRow(centerX - lastYWidth, centerX + lastYWidth, centerY + y, action)) return false;
                    if (!arr.DrawRow(centerX - lastYWidth, centerX + lastYWidth, centerY - y, action)) return false;
                }
                y--;
                lastYWidth = x;
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
        return DrawCircle(arr, centerX, centerY, radius, new DrawActions<T>()
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

    public static bool DrawCircle<T>(this T[,] arr, int radius, DrawAction<T> action)
    {
        Point center = arr.Center();
        return arr.DrawCircle(center.x, center.y, radius, action);
    }

    public static bool DrawCircle<T>(this T[,] arr, int radius, DrawActions<T> action)
    {
        Point center = arr.Center();
        return arr.DrawCircle(center.x, center.y, radius, action);
    }
    #endregion
    #endregion
    #region Squares
    public static bool DrawSquare<T>(this T[,] arr, int xl, int xr, int yb, int yt, DrawActions<T> action)
    {
        if (action.StrokeAction == null)
            return DrawSquareHelper.DrawSquare(arr, xl, xr, yb, yt, action);

        DrawActions<T> stroke = new DrawActions<T>() { UnitAction = action.StrokeAction };
        DrawActions<T> fill = new DrawActions<T>() { UnitAction = action.UnitAction };

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
        return arr.DrawSquare(xl, xr, yb, yt, new DrawActions<T>()
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
    #region Find Options
    public static List<Bounding> GetSquares<T>(this T[,] arr, int width, int height, bool tryFlipped, OptionTests<T> tester, Bounding scope = null)
    {
        SquareFinder<T> finder = new SquareFinder<T>(arr, width, height, tryFlipped, tester, scope);
        return finder.Find();
    }
    #endregion
    #region Searches
    public static Stack<Value2D<T>> DepthFirstSearch<T>(this T[,] arr, int x, int y, DrawAction<T> tester)
    {
        #region DEBUG
        //if (BigBoss.Debug.Flag(DebugManager.DebugFlag.SearchSteps) && BigBoss.Debug.logging(Logs.LevelGen))
        //{
        //    BigBoss.Debug.printHeader(Logs.LevelGen, "Depth First Search");
        //    arr.ToLog(Logs.LevelGen, "Starting:");
        //}
        #endregion
        Array2D<bool> blockedPoints = new Array2D<bool>(arr.GetLength(1), arr.GetLength(0));
        return null;
    }

    #endregion
}

public class DrawCircleHelper
{
    public static bool DrawCircleNoStroke<T>(T[,] arr, int centerX, int centerY, int radius, DrawActions<T> action)
    {
        int radiusError = 3 - (2 * radius);
        int x = 0;
        int y = radius;

        while (x <= y)
        {
            // Draw rows from center extending up/down
            if (!arr.DrawRow(centerX - y, centerX + y, centerY + x, action)) return false;
            if (!arr.DrawRow(centerX - y, centerX + y, centerY - x, action)) return false;
            if (radiusError < 0)
            {
                radiusError += (4 * x) + 6;
            }
            else
            {
                radiusError += 4 * (x - y) + 10;
                // Draw rows from top/bottom only when y is about to change.
                if (!arr.DrawRow(centerX - x, centerX + x, centerY + y, action)) return false;
                if (!arr.DrawRow(centerX - x, centerX + x, centerY - y, action)) return false;
                y--;
            }
            x++;
        }
        return true;
    }
}

public class DrawSquareHelper
{
    public static bool DrawSquare<T>(T[,] arr, int xl, int xr, int yb, int yt, DrawActions<T> action)
    {
        for (; yb <= yt; yb++)
            if (!arr.DrawRow(xl, xr, yb, action)) return false;
        return true;
    }
}
