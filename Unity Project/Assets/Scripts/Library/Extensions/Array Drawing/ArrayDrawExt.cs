using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class ArrayDrawExt
{
    #region Point
    #region Draws
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

    public static bool DrawAround<T>(this T[,] arr, int x, int y, bool cornered, DrawActionCall<T> action)
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

    public static bool DrawDirs<T>(this T[,] arr, int x, int y, GridDirection dir, DrawActionCall<T> action)
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

    public static bool DrawLocation<T>(this T[,] arr, int x, int y, GridLocation loc, DrawActionCall<T> action)
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
    #endregion
    #region Around
    public static List<Value2D<T>> GetPointsAround<T>(this T[,] arr, int x, int y, bool cornered, DrawActionCall<T> tester)
    {
        List<Value2D<T>> ret = new List<Value2D<T>>(cornered ? 9 : 4);
        arr.DrawAround(x, y, cornered, new DrawAction<T>((arr2, x2, y2) =>
        {
            if (tester(arr2, x2, y2))
                ret.Add(new Value2D<T>(x2, y2, arr[y2, x2]));
            return true;
        }));
        return ret;
    }

    public static List<T> GetValuesAllAround<T>(this T[,] arr, int x, int y, bool cornered, DrawActionCall<T> tester)
    {
        List<T> ret = new List<T>(cornered ? 9 : 4);
        arr.DrawAround(x, y, cornered, (arr2, x2, y2) =>
        {
            if (tester(arr2, x2, y2))
                ret.Add(arr2[y, x]);
            return true;
        });
        return ret;
    }

    public static bool HasAround<T>(this T[,] arr, int x, int y, bool cornered, DrawActionCall<T> tester)
    {
        return !arr.DrawAround(x, y, cornered, new DrawAction<T>((arr2, x2, y2) =>
        {
            if (tester(arr2, x2, y2))
                return false; // stop drawing around
            return true; // keep drawing around
        }));
    }

    public static bool GetPointAround<T>(this T[,] arr, int x, int y, bool cornered, DrawActionCall<T> tester, out Value2D<T> val)
    {
        Value2D<T> ret = null;
        if (arr.DrawAround(x, y, cornered, new DrawAction<T>((arr2, x2, y2) =>
        {
            if (tester(arr2, x2, y2))
            {
                ret = new Value2D<T>(x2, y2, arr[y2, x2]);
                return false; // stop drawing around
            }
            return true; // keep drawing around
        })))
        {
            val = ret;
            return false;
        }
        val = ret;
        return true;
    }

    public static bool GetValueAround<T>(this T[,] arr, int x, int y, bool cornered, DrawActionCall<T> tester, out T val)
    {
        T ret = default(T);
        if (arr.DrawAround(x, y, cornered, (arr2, x2, y2) =>
        {
            ret = arr2[y2, x2];
            if (tester(arr2, x2, y2))
                return false;
            return true;
        }))
        {
            val = ret;
            return false;
        }
        val = ret;
        return true;
    }

    public static bool GetRandomValueAround<T>(this T[,] arr, int x, int y, bool cornered, Random rand, DrawActionCall<T> tester, out T val)
    {
        List<T> options = GetValuesAllAround(arr, x, y, cornered, tester);
        if (options.Count > 0)
        {
            val = options.Random(rand);
            return true;
        }
        val = default(T);
        return false;
    }

    public static bool GetRandomPointAround<T>(this T[,] arr, int x, int y, bool cornered, Random rand, DrawActionCall<T> tester, out Value2D<T> val)
    {
        List<Value2D<T>> options = GetPointsAround(arr, x, y, cornered, tester);
        if (options.Count > 0)
        {
            val = options.Random(rand);
            return true;
        }
        val = null;
        return false;
    }
    #endregion
    #region Get Direction
    public static List<Value2D<T>> GetPointsOn<T>(this T[,] arr, int x, int y, GridDirection dir, DrawActionCall<T> tester)
    {
        List<Value2D<T>> ret = new List<Value2D<T>>(4);
        arr.DrawDirs(x, y, dir, new DrawAction<T>((arr2, x2, y2) =>
        {
            if (tester(arr2, x2, y2))
                ret.Add(new Value2D<T>(x2, y2, arr[y2, x2]));
            return true;
        }));
        return ret;
    }

    public static List<T> GetValuesOn<T>(this T[,] arr, int x, int y, GridDirection dir, DrawActionCall<T> tester)
    {
        List<T> ret = new List<T>(4);
        arr.DrawDirs(x, y, dir, (arr2, x2, y2) =>
        {
            T t = arr2[y2, x2];
            if (tester(arr2, x2, y2))
                ret.Add(t);
            return true;
        });
        return ret;
    }

    public static bool GetPointOn<T>(this T[,] arr, int x, int y, GridDirection dir, DrawActionCall<T> tester, out Value2D<T> val)
    {
        Value2D<T> ret = null;
        if (arr.DrawDirs(x, y, dir, new DrawAction<T>((arr2, x2, y2) =>
        {
            if (tester(arr2, x2, y2))
            {
                ret = new Value2D<T>(x2, y2, arr[y2, x2]);
                return false;
            }
            return true;
        })))
        {
            val = ret;
            return false;
        }
        val = ret;
        return true;
    }

    public static bool GetValueOn<T>(this T[,] arr, int x, int y, GridDirection dir, DrawActionCall<T> tester, out T val)
    {
        T ret = default(T);
        if (arr.DrawDirs(x, y, dir, (arr2, x2, y2) =>
        {
            ret = arr2[y2, x2];
            if (tester(arr2, x2, y2))
                return false;
            return true;
        }))
        {
            val = ret;
            return false;
        }
        val = ret;
        return true;
    }
    #endregion
    #region Utility
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
    #endregion
    #region Lines
    public static bool DrawLine<T>(this T[,] arr, int from, int to, int on, bool horizontal, StrokedAction<T> action)
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

    public static bool DrawCol<T>(this T[,] arr, int y1, int y2, int x, StrokedAction<T> action)
    {
        for (; y1 <= y2; y1++)
            if (!action.UnitAction(arr, x, y1)) return false;
        return true;
    }

    public static bool DrawRow<T>(this T[,] arr, int xl, int xr, int y, T setTo)
    {
        return DrawRow(arr, xl, xr, y, SetTo(setTo));
    }

    public static bool DrawRow<T>(this T[,] arr, int xl, int xr, int y, StrokedAction<T> action)
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
    public static bool DrawCircle<T>(this T[,] arr, int centerX, int centerY, int radius, StrokedAction<T> action)
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
        return DrawCircle(arr, centerX, centerY, radius, new StrokedAction<T>()
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

    public static bool DrawCircle<T>(this T[,] arr, int radius, StrokedAction<T> action)
    {
        Point center = arr.Center();
        return arr.DrawCircle(center.x, center.y, radius, action);
    }
    #endregion
    #endregion
    #region Squares
    public static bool DrawSquare<T>(this T[,] arr, StrokedAction<T> action, bool includeEdges = true)
    {
        if (includeEdges)
            return DrawSquare(arr, 0, arr.GetLength(1) - 1, 0, arr.GetLength(0) - 1, action);
        else
            return DrawSquare(arr, 1, arr.GetLength(1) - 2, 1, arr.GetLength(0) - 2, action);
    }

    public static bool DrawSquare<T>(this T[,] arr, int xl, int xr, int yb, int yt, StrokedAction<T> action)
    {
        if (action.StrokeAction == null)
            return DrawSquareHelper.DrawSquare(arr, xl, xr, yb, yt, action);

        StrokedAction<T> stroke = new StrokedAction<T>() { UnitAction = action.StrokeAction };
        StrokedAction<T> fill = new StrokedAction<T>() { UnitAction = action.UnitAction };

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
        return arr.DrawSquare(xl, xr, yb, yt, new StrokedAction<T>()
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
    public static Stack<Value2D<T>> DrawDepthFirstSearch<T>(this T[,] arr, int x, int y,
        DrawActionCall<T> allowedSpace,
        DrawActionCall<T> target,
        System.Random rand,
        bool edgeSafe = false)
    {
        #region DEBUG
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.SearchSteps) && BigBoss.Debug.logging(Logs.LevelGen) && typeof(T) == typeof(GridType))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Depth First Search");
            GridType[,] tmpArr = new GridType[arr.GetLength(0), arr.GetLength(1)];
            for (int y1 = 0; y1 < arr.GetLength(0); y1++)
                for (int x1 = 0; x1 < arr.GetLength(1); x1++)
                    tmpArr[y1, x1] = (GridType)(object)arr[y1, x1];
            GridArray tmp = new GridArray(tmpArr);
            tmp[x, y] = GridType.INTERNAL_RESERVED_CUR;
            tmp.ToLog(Logs.LevelGen, "Starting Map:");
        }
        #endregion
        var blockedPoints = new Array2D<bool>(arr.GetLength(1), arr.GetLength(0));
        var pathTaken = new Stack<Value2D<T>>();
        DrawAction<T> filter = new DrawActionCall<T>((arr2, x2, y2) =>
        {
            return !blockedPoints[x2, y2] && allowedSpace(arr2, x2, y2);
        });
        DrawAction<T> foundTarget = new DrawActionCall<T>((arr2, x2, y2) =>
        {
            return !blockedPoints[x2, y2] && target(arr2, x2, y2);
        });
        if (edgeSafe)
        {
            filter = filter.Then(Draw.NotEdgeOfArray<T>());
            foundTarget = foundTarget.Then(Draw.NotEdgeOfArray<T>());
        }
        Value2D<T> curPoint;
        Value2D<T> targetDir;

        // Push start point onto path
        pathTaken.Push(new Value2D<T>(x, y));
        while (pathTaken.Count > 0)
        {
            curPoint = pathTaken.Peek();
            // Don't want to visit the same point on a different route later
            blockedPoints[curPoint] = true;

            // If found target, return path we took
            if (arr.GetPointAround(curPoint.x, curPoint.y, false, foundTarget, out targetDir))
            {
                #region DEBUG
                if (BigBoss.Debug.Flag(DebugManager.DebugFlag.SearchSteps) && BigBoss.Debug.logging(Logs.LevelGen))
                {
                    BigBoss.Debug.w(Logs.LevelGen, "===== FOUND TARGET: " + curPoint);
                    BigBoss.Debug.printFooter(Logs.LevelGen);
                }
                #endregion
                pathTaken.Push(targetDir);
                return pathTaken;
            }

            // Didn't find target, pick random direction
            if (arr.GetRandomPointAround<T>(curPoint.x, curPoint.y, false, rand, filter, out targetDir))
            {
                #region DEBUG
                if (BigBoss.Debug.Flag(DebugManager.DebugFlag.SearchSteps) && BigBoss.Debug.logging(Logs.LevelGen))
                {
                    BigBoss.Debug.w(Logs.LevelGen, "Chose Direction: " + targetDir);
                }
                #endregion
                curPoint = targetDir;
                pathTaken.Push(curPoint);
            }
            else
            { // If all directions are bad, back up
                pathTaken.Pop();
            }
        }
        #region DEBUG
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.SearchSteps) && BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printFooter(Logs.LevelGen);
        }
        #endregion
        return pathTaken;
    }

    public static void DrawBreadthFirstFill<T>(this T[,] arr, int x, int y,
        bool cornered,
        DrawActionCall<T> run,
        bool edgeSafe = false)
    {
        #region DEBUG
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.SearchSteps) && BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Breadth First Fill");
        }
        #endregion
        Queue<Value2D<T>> queue = new Queue<Value2D<T>>();
        queue.Enqueue(new Value2D<T>(x, y));
        bool[,] visited = new bool[arr.GetLength(0), arr.GetLength(1)];
        visited[y, x] = true;
        Point curPoint;
        while (queue.Count > 0)
        {
            curPoint = queue.Dequeue();
            #region DEBUG
            if (BigBoss.Debug.Flag(DebugManager.DebugFlag.SearchSteps) && BigBoss.Debug.logging(Logs.LevelGen))
            {
                visited.ToLog(Logs.LevelGen, "Current Map. Evaluating " + curPoint);
            }
            #endregion
            arr.DrawAround(curPoint.x, curPoint.y, true, (arr2, x2, y2) =>
            {
                if (!edgeSafe || arr.InRange(x2, y2))
                {
                    if (!visited[y2, x2] && run(arr2, x2, y2))
                    {
                        #region DEBUG
                        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.SearchSteps) && BigBoss.Debug.logging(Logs.LevelGen))
                        {
                            BigBoss.Debug.w(Logs.LevelGen, "Queuing " + x2 + " " + y2);
                        }
                        #endregion
                        queue.Enqueue(new Value2D<T>(x2, y2, arr2[y2, x2]));
                    }
                    visited[y2, x2] = true;
                }
                return true;
            });
        }
        #region DEBUG
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.SearchSteps) && BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printFooter(Logs.LevelGen);
        }
        #endregion
    }

    public static void DrawPerimeter<T>(this T[,] arr, DrawActionCall<T> evaluator, StrokedAction<T> action)
    {
        DrawActionCall<T> unit = action.UnitAction;
        DrawActionCall<T> stroke = action.StrokeAction;
        bool hasFill = unit != null;
        bool[,] fillArr = null;
        if (hasFill)
            fillArr = new bool[arr.GetLength(0), arr.GetLength(1)];
        // Get null spaces surrounding room
        arr.DrawBreadthFirstFill(0, 0, true, (arr2, x, y) =>
        {
            if (hasFill)
                fillArr[y, x] = true;
            if (!evaluator(arr, x, y)) return true; // Valid space to continue from
            if (stroke != null)
                stroke(arr2, x, y);
            else
                unit(arr, x, y);
            return false; // We aren't on null, so don't continue using this space
        }, true);
        if (hasFill)
            for (int y = 0; y < arr.GetLength(0); y++)
                for (int x = 0; x < arr.GetLength(1); x++)
                    if (!fillArr[y, x])
                        unit(arr, x, y);
    }
    #endregion
}

#region Helpers
// These functions should be protected functions, but static classes cannot have non-public members.
// Moving them outside serves to "hide" these functions
public class DrawCircleHelper
{
    public static bool DrawCircleNoStroke<T>(T[,] arr, int centerX, int centerY, int radius, StrokedAction<T> action)
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
    public static bool DrawSquare<T>(T[,] arr, int xl, int xr, int yb, int yt, StrokedAction<T> action)
    {
        for (; yb <= yt; yb++)
            if (!arr.DrawRow(xl, xr, yb, action)) return false;
        return true;
    }
}
#endregion
