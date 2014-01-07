using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class DrawExt
{
    #region Point
    #region Draws
    public static IEnumerable<T> DrawAround<T>(this Container2D<T> arr, int x, int y, bool cornered)
    {
        if (!cornered)
        {
            yield return arr[x, y + 1];
            yield return arr[x, y - 1];
            yield return arr[x + 1, y];
            yield return arr[x - 1, y];
        }
        else
        {
            yield return arr[x - 1, y - 1]; // bottom left
            yield return arr[x, y - 1]; // bottom
            yield return arr[x + 1, y - 1]; // bottom right
            yield return arr[x + 1, y]; // Right
            yield return arr[x + 1, y + 1]; // Top Right
            yield return arr[x, y + 1]; // Top
            yield return arr[x - 1, y + 1]; // Top Left
            yield return arr[x - 1, y]; // Left
        }
    }

    public static bool DrawAround<T>(this Container2D<T> arr, int x, int y, bool cornered, DrawActionCall<T> action)
    {
        if (!cornered)
        {
            if (!action(arr, x, y + 1)) return false;
            if (!action(arr, x, y - 1)) return false;
            if (!action(arr, x + 1, y)) return false;
            if (!action(arr, x - 1, y)) return false;
        }
        else
        {
            if (!action(arr, x - 1, y - 1)) return false; // Bottom left
            if (!action(arr, x, y - 1)) return false; // Bottom
            if (!action(arr, x + 1, y - 1)) return false; // Bottom right
            if (!action(arr, x + 1, y)) return false; // Right
            if (!action(arr, x + 1, y + 1)) return false; // Top Right
            if (!action(arr, x, y + 1)) return false; // Top
            if (!action(arr, x - 1, y + 1)) return false; // Top Left
            if (!action(arr, x - 1, y)) return false; // Left
        }
        return true;
    }

    public static bool DrawDir<T>(this Container2D<T> arr, int x, int y, GridDirection dir, DrawActionCall<T> action)
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

    public static bool DrawLocation<T>(this Container2D<T> arr, int x, int y, GridLocation loc, DrawActionCall<T> action)
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
    // Returns list of points around that satisfy
    public static List<Value2D<T>> GetPointsAround<T>(this Container2D<T> arr, int x, int y, bool cornered, DrawActionCall<T> tester)
    {
        List<Value2D<T>> ret = new List<Value2D<T>>(cornered ? 9 : 4);
        arr.DrawAround(x, y, cornered, new DrawAction<T>((arr2, x2, y2) =>
        {
            if (tester(arr2, x2, y2))
                ret.Add(new Value2D<T>(x2, y2, arr[x2, y2]));
            return true;
        }));
        return ret;
    }

    // Returns list of values around that satisfy
    public static List<T> GetValuesAllAround<T>(this Container2D<T> arr, int x, int y, bool cornered, DrawActionCall<T> tester)
    {
        List<T> ret = new List<T>(cornered ? 9 : 4);
        arr.DrawAround(x, y, cornered, (arr2, x2, y2) =>
        {
            if (tester(arr2, x2, y2))
                ret.Add(arr2[x, y]);
            return true;
        });
        return ret;
    }

    // Returns true if square around has
    public static bool HasAround<T>(this Container2D<T> arr, int x, int y, bool cornered, DrawActionCall<T> tester)
    {
        return !arr.DrawAround(x, y, cornered, new DrawAction<T>((arr2, x2, y2) =>
        {
            if (tester(arr2, x2, y2))
                return false; // stop drawing around
            return true; // keep drawing around
        }));
    }

    public static bool GetPointAround<T>(this Container2D<T> arr, int x, int y, bool cornered, DrawActionCall<T> tester, out Value2D<T> val)
    {
        Value2D<T> ret = null;
        if (arr.DrawAround(x, y, cornered, new DrawAction<T>((arr2, x2, y2) =>
        {
            if (tester(arr2, x2, y2))
            {
                ret = new Value2D<T>(x2, y2, arr[x2, y2]);
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

    public static bool GetValueAround<T>(this Container2D<T> arr, int x, int y, bool cornered, DrawActionCall<T> tester, out T val)
    {
        T ret = default(T);
        if (arr.DrawAround(x, y, cornered, (arr2, x2, y2) =>
        {
            ret = arr2[x2, y2];
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

    public static bool GetRandomValueAround<T>(this Container2D<T> arr, int x, int y, bool cornered, Random rand, DrawActionCall<T> tester, out T val)
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

    public static bool GetRandomPointAround<T>(this Container2D<T> arr, int x, int y, bool cornered, Random rand, DrawActionCall<T> tester, out Value2D<T> val)
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
    public static List<Value2D<T>> GetPointsOn<T>(this Container2D<T> arr, int x, int y, GridDirection dir, DrawActionCall<T> tester)
    {
        List<Value2D<T>> ret = new List<Value2D<T>>(4);
        arr.DrawDir(x, y, dir, new DrawAction<T>((arr2, x2, y2) =>
        {
            if (tester(arr2, x2, y2))
                ret.Add(new Value2D<T>(x2, y2, arr[x2, y2]));
            return true;
        }));
        return ret;
    }

    public static List<T> GetValuesOn<T>(this Container2D<T> arr, int x, int y, GridDirection dir, DrawActionCall<T> tester)
    {
        List<T> ret = new List<T>(4);
        arr.DrawDir(x, y, dir, (arr2, x2, y2) =>
        {
            T t = arr2[x2, y2];
            if (tester(arr2, x2, y2))
                ret.Add(t);
            return true;
        });
        return ret;
    }

    public static bool GetPointOn<T>(this Container2D<T> arr, int x, int y, GridDirection dir, DrawActionCall<T> tester, out Value2D<T> val)
    {
        Value2D<T> ret = null;
        if (arr.DrawDir(x, y, dir, new DrawAction<T>((arr2, x2, y2) =>
        {
            if (tester(arr2, x2, y2))
            {
                ret = new Value2D<T>(x2, y2, arr[x2, y2]);
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

    public static bool GetValueOn<T>(this Container2D<T> arr, int x, int y, GridDirection dir, DrawActionCall<T> tester, out T val)
    {
        T ret = default(T);
        if (arr.DrawDir(x, y, dir, (arr2, x2, y2) =>
        {
            ret = arr2[x2, y2];
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
    /*
     * _#_         ___
     * ___    or   #_#
     * _#_         ___
     */
    public static bool AlternatesSides<T>(this Container2D<T> arr, int x, int y, Func<T, bool> evaluator)
    {
        bool pass = evaluator(arr[x - 1, y]);
        if (pass != evaluator(arr[x + 1, y])) return false;
        if (pass == evaluator(arr[x, y + 1])) return false;
        if (pass == evaluator(arr[x, y - 1])) return false;
        return true;
    }

    /*
     * ___                            __#
     * #__       or with opposing     #__
     * _#_                            _#_
     */
    public static bool Cornered<T>(this Container2D<T> arr, int x, int y, Func<T, bool> evaluator, bool withOpposing = false)
    {
        bool Xpass = evaluator(arr[x - 1, y]);
        if (Xpass == evaluator(arr[x + 1, y])) return false;
        bool Ypass = evaluator(arr[x, y - 1]);
        if (Ypass == evaluator(arr[x, y + 1])) return false;
        return !withOpposing || evaluator(arr[Xpass ? x + 1 : x - 1, Ypass ? y + 1 : y - 1]);
    }

    /*
     * Walk edges and if alternates more than twice, it's blocking
     */
    public static bool Blocking<T>(this Container2D<T> arr, int x, int y, Func<T, bool> evaluator)
    {
        int count = 0;
        bool status = evaluator(arr[x - 1, y - 1]); // Bottom left
        DrawActionCall<T> func = (arr2, x2, y2) =>
            {
                if (evaluator(arr2[x2, y2]) != status)
                {
                    status = !status;
                    return ++count > 2;
                }
                return false;
            };
        if (func(arr, x, y - 1)) return true; // Bottom
        if (func(arr, x + 1, y - 1)) return true; // Bottom right
        if (func(arr, x + 1, y)) return true; // Right
        if (func(arr, x + 1, y + 1)) return true; // Top Right
        if (func(arr, x, y + 1)) return true; // Top
        if (func(arr, x - 1, y + 1)) return true; // Top Left
        if (func(arr, x - 1, y)) return true; // Left
        return false;
    }
    #endregion
    #endregion
    #region Lines
    public static bool DrawLine<T>(this Container2D<T> arr, int from, int to, int on, bool horizontal, DrawActionCall<T> action)
    {
        if (horizontal)
            return DrawRow(arr, from, to, on, action);
        else
            return DrawCol(arr, from, to, on, action);
    }

    public static bool DrawCol<T>(this Container2D<T> arr, int yb, int yt, int x, DrawActionCall<T> action, bool BottomToTop = true)
    {
        if (BottomToTop)
        {
            for (; yb <= yt; yb++)
                if (!action(arr, x, yb)) return false;
        }
        else
        {
            for (; yb <= yt; yt--)
                if (!action(arr, x, yt)) return false;
        }
        return true;
    }

    public static bool DrawRow<T>(this Container2D<T> arr, int xl, int xr, int y, DrawActionCall<T> action, bool LeftToRight = true)
    {
        if (LeftToRight)
        {
            for (; xl <= xr; xl++)
                if (!action(arr, xl, y)) return false;
        }
        else
        {
            for (; xl <= xr; xr--)
                if (!action(arr, xr, y)) return false;
        }
        return true;
    }

    public static DrawAction<T> SetTo<T>(T to)
    {
        return new DrawAction<T>((arr, x, y) =>
        {
            arr[x, y] = to;
            return true;
        });
    }
    #endregion
    #region Standard
    public static void DrawAll<T>(this Container2D<T> arr, DrawActionCall<T> action)
    {
        foreach (Value2D<T> val in arr)
        {
            action(arr, val.x, val.y);
        }
    }
    #endregion
    #region Circles
    /*
     * Uses Bressenham's Midpoint Algo
     */
    public static bool DrawCircle<T>(this Container2D<T> arr, int centerX, int centerY, int radius, StrokedAction<T> action)
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
    public static bool DrawCircle<T>(this Container2D<T> arr, int centerX, int centerY, int radius, T setTo)
    {
        return DrawCircle(arr, centerX, centerY, radius, SetTo(setTo));
    }

    public static bool DrawCircle<T>(this Container2D<T> arr, int centerX, int centerY, int radius, T setFill, T setStroke)
    {
        return DrawCircle(arr, centerX, centerY, radius, new StrokedAction<T>()
            {
                UnitAction = SetTo(setFill),
                StrokeAction = SetTo(setStroke)
            });
    }

    public static bool DrawCircle<T>(this Container2D<T> arr, int radius, T setTo)
    {
        Point center = arr.Center;
        return arr.DrawCircle(center.x, center.y, radius, setTo);
    }

    public static bool DrawCircle<T>(this Container2D<T> arr, int radius, T setFill, T strokeFill)
    {
        Point center = arr.Center;
        return arr.DrawCircle(center.x, center.y, radius, setFill, strokeFill);
    }

    public static bool DrawCircle<T>(this Container2D<T> arr, int radius, DrawAction<T> action)
    {
        Point center = arr.Center;
        return arr.DrawCircle(center.x, center.y, radius, action);
    }

    public static bool DrawCircle<T>(this Container2D<T> arr, int radius, StrokedAction<T> action)
    {
        Point center = arr.Center;
        return arr.DrawCircle(center.x, center.y, radius, action);
    }
    #endregion
    #endregion
    #region Squares
    public static bool DrawSquare<T>(this Container2D<T> arr, StrokedAction<T> action, bool includeEdges = true)
    {
        if (includeEdges)
            return DrawSquare(arr, 0, arr.Width - 1, 0, arr.Height - 1, action);
        else
            return DrawSquare(arr, 1, arr.Width - 2, 1, arr.Height - 2, action);
    }

    public static bool DrawSquare<T>(this Container2D<T> arr, int xl, int xr, int yb, int yt, StrokedAction<T> action)
    {
        if (action.StrokeAction == null)
            return DrawSquareHelper.DrawSquareNoStroke(arr, xl, xr, yb, yt, action);

        if (xl == xr && yb == yt)
            return action.StrokeAction(arr, xl, yb);

        if (!arr.DrawRow(xl, xr, yb,  action.StrokeAction)) return false;
        if (!arr.DrawRow(xl, xr, yt,  action.StrokeAction)) return false;
        yb++;
        yt--;
        if (!arr.DrawCol(yb, yt, xl,  action.StrokeAction)) return false;
        if (!arr.DrawCol(yb, yt, xr,  action.StrokeAction)) return false;
        xl++;
        xr--;
        if (action.UnitAction != null)
            if (!DrawSquareHelper.DrawSquareNoStroke(arr, xl, xr, yb, yt, action.UnitAction)) return false;
        return true;
    }

    public static bool DrawSquare<T>(this Container2D<T> arr, int x, int y, int radius, StrokedAction<T> action)
    {
        return arr.DrawSquare<T>(x - radius, x + radius, y - radius, y + radius, action);
    }
    #endregion
    #region Expand
    public static bool DrawSquareSpiral<T>(this Container2D<T> arr, int x, int y, DrawAction<T> draw, Bounding bounds = null)
    {
        Bounding arrBound = arr.Bounding;
        if (bounds == null)
            bounds = arrBound;
        else
            bounds.IntersectBounds(arrBound);
        if (!bounds.IsValid() || !bounds.Contains(x, y)) return true;

        if (!draw.Call(arr, x, y)) return false;
        Bounding currentBounds = new Bounding() { XMin = x - 1, XMax = x + 1, YMin = y - 1, YMax = y + 1 };
        while (!currentBounds.Contains(arrBound))
        {
            if (currentBounds.YMin >= bounds.YMin)
            {
                if (!arr.DrawRow(currentBounds.XMin + 1, currentBounds.XMax - 1, currentBounds.YMin, draw)) return false;
                currentBounds.YMin--;
            }
            if (currentBounds.XMax <= bounds.XMax)
            {
                if (!arr.DrawCol(currentBounds.YMin + 1, currentBounds.YMax - 1, currentBounds.XMax, draw)) return false;
                currentBounds.XMax++;
            }
            if (currentBounds.YMax <= bounds.YMax)
            {
                if (!arr.DrawRow(currentBounds.XMin + 1, currentBounds.XMax - 1, currentBounds.YMax, draw, false)) return false;
                currentBounds.YMax++;
            }
            if (currentBounds.XMin >= bounds.XMin)
            {
                if (!arr.DrawCol(currentBounds.YMin + 1, currentBounds.YMax - 1, currentBounds.XMin, draw, false)) return false;
                currentBounds.XMin--;
            }
        }
        return true;
    }
    #endregion
    #region Find Options
    public static List<Bounding> GetSquares<T>(this Container2D<T> arr, int width, int height, bool tryFlipped, StrokedAction<T> tester, Bounding scope = null)
    {
        SquareFinder<T> finder = new SquareFinder<T>(arr, width, height, tryFlipped, tester, scope);
        return finder.Find();
    }
    #endregion
    #region Searches
    public static Stack<Value2D<T>> DrawDepthFirstSearch<T>(this Container2D<T> arr, int x, int y,
        DrawActionCall<T> allowedSpace,
        DrawActionCall<T> target,
        System.Random rand,
        bool edgeSafe = false)
    {
        #region DEBUG
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps) && BigBoss.Debug.logging(Logs.LevelGen) && typeof(T) == typeof(GridType))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Depth First Search");
            Array2D<GridType> tmpArr = new Array2D<GridType>(arr.Width, arr.Height);
            for (int y1 = 0; y1 < arr.Height; y1++)
                for (int x1 = 0; x1 < arr.Width; x1++)
                    tmpArr[x1, y1] = (GridType)(object)arr[x1, y1];
            tmpArr[x, y] = GridType.INTERNAL_RESERVED_CUR;
            tmpArr.ToLog(Logs.LevelGen, "Starting Map:");
        }
        #endregion
        var blockedPoints = new Array2D<bool>(arr.Width, arr.Height);
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
            filter = filter.And(Draw.NotEdgeOfArray<T>());
            foundTarget = foundTarget.And(Draw.NotEdgeOfArray<T>());
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
                if (BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps) && BigBoss.Debug.logging(Logs.LevelGen))
                {
                    BigBoss.Debug.w(Logs.LevelGen, "===== FOUND TARGET: " + curPoint);
                    BigBoss.Debug.printFooter(Logs.LevelGen, "Depth First Search");
                }
                #endregion
                pathTaken.Push(targetDir);
                return pathTaken;
            }

            // Didn't find target, pick random direction
            if (arr.GetRandomPointAround<T>(curPoint.x, curPoint.y, false, rand, filter, out targetDir))
            {
                #region DEBUG
                if (BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps) && BigBoss.Debug.logging(Logs.LevelGen))
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
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps) && BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printFooter(Logs.LevelGen, "Depth First Search");
        }
        #endregion
        return pathTaken;
    }

    public static void DrawBreadthFirstFill<T>(this Container2D<T> arr, int x, int y,
        bool cornered,
        DrawActionCall<T> run)
    {
        #region DEBUG
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps) && BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Breadth First Fill");
            arr.ToLog(Logs.LevelGen, "Container to fill starting on (" + x + "," + y + ")");
        }
        #endregion
        Bounding bounds = arr.Bounding;
        bounds.expand(1);
        Queue<Value2D<T>> queue = new Queue<Value2D<T>>();
        queue.Enqueue(new Value2D<T>(x, y));
        Array2D<bool> visited = new Array2D<bool>(bounds);
        visited[x, y] = true;
        Point curPoint;
        while (queue.Count > 0)
        {
            curPoint = queue.Dequeue();
            arr.DrawAround(curPoint.x, curPoint.y, true, (arr2, x2, y2) =>
            {
                if (!bounds.Contains(x2, y2)) return true;
                if (!visited[x2, y2] && run(arr2, x2, y2))
                {
                    queue.Enqueue(new Value2D<T>(x2, y2, arr2[x2, y2]));
                    #region DEBUG
                    if (BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps) && BigBoss.Debug.logging(Logs.LevelGen))
                    {
                        visited[x2, y2] = true;
                        visited.ToLog(Logs.LevelGen, "Queued " + x2 + " " + y2);
                    }
                    #endregion
                }
                visited[x2, y2] = true;
                return true;
            });
        }
        #region DEBUG
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps) && BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printFooter(Logs.LevelGen, "Breadth First Fill");
        }
        #endregion
    }

    public static void DrawPerimeter<T>(this Container2D<T> cont, DrawActionCall<T> evaluator, StrokedAction<T> action)
    {
        DrawActionCall<T> call;
        Container2D<bool> debugArr;
        if (action.UnitAction != null && action.StrokeAction != null)
        {
            call = (arr, x, y) =>
            {
                if (arr.DrawAround(x, y, true, evaluator))
                    action.UnitAction(arr, x, y);
                else
                    action.StrokeAction(arr, x, y);
                return true;
            };
        }
        else if (action.UnitAction != null)
        {
            call = (arr, x, y) =>
            {
                if (arr.DrawAround(x, y, true, evaluator))
                    action.UnitAction(arr, x, y);
                return true;
            };
        }
        else
        {
            call = (arr, x, y) =>
            {
                if (!arr.DrawAround(x, y, true, evaluator))
                    action.StrokeAction(arr, x, y);
                return true;
            };
        }
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps))
        {
            debugArr = new Array2D<bool>(cont.Bounding);
            call = new DrawAction<T>(call).And((arr, x, y) => 
                {
                    debugArr[x, y] = true;
                    debugArr.ToLog("Draw Perimeter");
                    return true;
                });
        }
        cont.DrawAll(call);
    }
    #endregion
}

#region Helpers
// These functions should be protected functions, but static classes cannot have non-public members.
// Moving them outside serves to "hide" these functions
public class DrawCircleHelper
{
    public static bool DrawCircleNoStroke<T>(Container2D<T> arr, int centerX, int centerY, int radius, StrokedAction<T> action)
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
    public static bool DrawSquareNoStroke<T>(Container2D<T> arr, int xl, int xr, int yb, int yt, StrokedAction<T> action)
    {
        if (xl == xr && yb == yt)
            return action.UnitAction(arr, xl, yb);

        for (; yb <= yt; yb++)
            if (!arr.DrawRow(xl, xr, yb, action)) return false;
        return true;
    }
}
#endregion
