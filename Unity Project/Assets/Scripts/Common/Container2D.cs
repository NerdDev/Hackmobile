using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

abstract public class Container2D<T> : IEnumerable<Value2D<T>>
{
    public static Container2D<T> CreateArrayFromBounds<R>(Container2D<R> rhs)
    {
        Container2D<T> ret = new Array2D<T>(rhs.Bounding);
        if (rhs is Array2DRaw<R>)
        {
            Point shift;
            ret = ret.RawArray(out shift);
        }
        return ret;
    }

    #region Generic Members
    protected Container2D()
    {
    }

    public Container2D(Container2D<T> rhs)
    {
        PutAll(rhs);
    }

    public Container2D(Container2D<T> rhs, Point shift)
    {
        PutAll(rhs, shift);
    }

    public Container2D(Container2D<T> rhs, int xShift, int yShift)
    {
        PutAll(rhs, xShift, yShift);
    }

    public abstract T this[int x, int y] { get; set; }
    public T this[Point val]
    {
        get
        {
            return this[val.x, val.y];
        }
        set
        {
            this[val.x, val.y] = value;
        }
    }
    public abstract bool TryGetValue(int x, int y, out T val);
    public bool IsEmpty { get { return Count == 0; } }
    public abstract int Count { get; }
    public abstract Bounding Bounding { get; }
    public int Width { get { return Bounding.Width; } }
    public int Height { get { return Bounding.Height; } }
    public Point Center { get { return Bounding.GetCenter(); } }
    public abstract Array2D<T> Array { get; }
    public abstract bool Contains(int x, int y);
    public bool Contains(Point val)
    {
        return Contains(val.x, val.y);
    }
    public abstract bool InRange(int x, int y);
    public bool InRange(Point p)
    {
        return InRange(p.x, p.y);
    }

    public Value2D<T> GetNth(int n)
    {
        if (n >= 0)
        {
            int count = 0;
            foreach (Value2D<T> val in this)
            {
                if (count == n)
                {
                    return val;
                }
                count++;
            }
        }
        return null;
    }

    public abstract bool DrawAll(DrawAction<T> call);

    public abstract void Clear();

    public void Put(Value2D<T> val)
    {
        this[val] = val.val;
    }

    public abstract Array2DRaw<T> RawArray(out Point shift);

    #region Removes
    public bool Remove(Point p)
    {
        return Remove(p.x, p.y);
    }
    public abstract bool Remove(int x, int y);
    public void Remove(int x, int y, int radius)
    {
        for (int yCur = y - radius; yCur <= y + radius; yCur++)
        {
            for (int xCur = x - radius; xCur <= x + radius; xCur++)
            {
                Remove(xCur, yCur);
            }
        }
    }

    public void RemoveAll(Container2D<T> rhs)
    {
        foreach (Value2D<T> val in rhs)
        {
            Remove(val);
        }
    }

    public void RemoveAllBut(params T[] types)
    {
        RemoveAllBut(new HashSet<T>(types));
    }

    public void RemoveAllBut(HashSet<T> types)
    {
        List<Value2D<T>> vals = new List<Value2D<T>>(this);
        foreach (Value2D<T> val in vals)
        {
            if (!types.Contains(val.val))
            {
                Remove(val);
            }
        }
    }
    #endregion

    #region Put All
    public void PutAll(Container2D<T> rhs)
    {
        foreach (Value2D<T> val in rhs)
        {
            this[val] = val.val;
        }
    }
    public void PutAll(Container2D<T> rhs, Point shift)
    {
        PutAll(rhs, shift.x, shift.y);
    }
    public void PutAll(Container2D<T> rhs, int xShift, int yShift)
    {
        foreach (Value2D<T> val in rhs)
        {
            this[val.x + xShift, val.y + yShift] = val.val;
        }
    }
    #endregion

    public abstract void Shift(int x, int y);

    public void Shift(Point p)
    {
        Shift(p.x, p.y);
    }

    public bool Intersects(Container2D<T> rhs)
    {
        Bounding bounds = Bounding;
        Bounding rhsBounds = rhs.Bounding;
        Bounding intersect = bounds.IntersectBounds(rhsBounds);
        if (!intersect.IsValid()) return false;

        foreach (Value2D<T> val in this)
        {
            if (rhs.Contains(val)) return true;
        }
        return false;
    }

    public bool Intersects(IEnumerable<Container2D<T>> options, out Container2D<T> intersect)
    {
        foreach (Container2D<T> c in options)
        {
            if (Intersects(c))
            {
                intersect = c;
                return true;
            }
        }
        intersect = null;
        return false;
    }

    #region Logging
    public virtual List<string> ToRowStrings(Bounding bounds = null)
    {
        return Array.ToRowStrings(bounds);
    }

    public virtual void ToLog(Logs log)
    {
        if (BigBoss.Debug.logging(log))
        {
            ToLog(log, new string[0]);
        }
    }

    public void ToLog()
    {
        ToLog(BigBoss.Debug.LastLog);
    }

    public void ToLog(params string[] customContent)
    {
        ToLog(BigBoss.Debug.LastLog, customContent);
    }

    public virtual void ToLog(Logs log, params string[] customContent)
    {
        if (BigBoss.Debug.logging(log))
        {
            BigBoss.Debug.printHeader(log, ToString());
            foreach (string s in customContent)
            {
                BigBoss.Debug.w(log, s);
            }
            foreach (string s in ToRowStrings())
            {
                BigBoss.Debug.w(log, s);
            }
            BigBoss.Debug.w(log, "Bounds: " + Bounding.ToString());
            BigBoss.Debug.printFooter(log, ToString());
        }
    }
    #endregion

    public static void Smallest<Z>(Z obj1, Z obj2, out Z smallest, out Z largest) where Z : Container2D<T>
    {
        if (obj1.Bounding.Area < obj2.Bounding.Area)
        {
            smallest = obj1;
            largest = obj2;
            return;
        }
        smallest = obj2;
        largest = obj1;
    }

    public bool Random(System.Random random, out Value2D<T> ret, bool take = false)
    {
        List<Value2D<T>> list = Random(random, 1, 0, take);
        if (list.Count == 0)
        {
            ret = null;
            return false;
        }
        ret = list[0];
        return true;
    }

    public virtual List<Value2D<T>> Random(System.Random random, int amount, int distance = 0, bool take = false)
    {
        throw new NotImplementedException();
    }

    public abstract IEnumerator<Value2D<T>> GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
    #endregion
    #region Draw Functions
    #region Point
    #region Draws
    public IEnumerable<T> DrawAround(int x, int y, bool cornered)
    {
        if (!cornered)
        {
            yield return this[x, y + 1];
            yield return this[x, y - 1];
            yield return this[x + 1, y];
            yield return this[x - 1, y];
        }
        else
        {
            yield return this[x - 1, y - 1]; // bottom left
            yield return this[x, y - 1]; // bottom
            yield return this[x + 1, y - 1]; // bottom right
            yield return this[x + 1, y]; // Right
            yield return this[x + 1, y + 1]; // Top Right
            yield return this[x, y + 1]; // Top
            yield return this[x - 1, y + 1]; // Top Left
            yield return this[x - 1, y]; // Left
        }
    }

    public bool DrawAround(int x, int y, bool cornered, DrawAction<T> action)
    {
        if (!cornered)
        {
            if (!action(this, x, y + 1)) return false;
            if (!action(this, x, y - 1)) return false;
            if (!action(this, x + 1, y)) return false;
            if (!action(this, x - 1, y)) return false;
        }
        else
        {
            if (!action(this, x - 1, y - 1)) return false; // Bottom left
            if (!action(this, x, y - 1)) return false; // Bottom
            if (!action(this, x + 1, y - 1)) return false; // Bottom right
            if (!action(this, x + 1, y)) return false; // Right
            if (!action(this, x + 1, y + 1)) return false; // Top Right
            if (!action(this, x, y + 1)) return false; // Top
            if (!action(this, x - 1, y + 1)) return false; // Top Left
            if (!action(this, x - 1, y)) return false; // Left
        }
        return true;
    }

    public bool DrawDir(int x, int y, GridDirection dir, DrawAction<T> action)
    {
        switch (dir)
        {
            case GridDirection.HORIZ:
                if (!action(this, x - 1, y)) return false;
                if (!action(this, x + 1, y)) return false;
                break;
            case GridDirection.VERT:
                if (!action(this, x, y + 1)) return false;
                if (!action(this, x, y - 1)) return false;
                break;
            case GridDirection.DIAGTLBR:
                if (!action(this, x - 1, y + 1)) return false;
                if (!action(this, x + 1, y - 1)) return false;
                break;
            case GridDirection.DIAGBLTR:
                if (!action(this, x - 1, y - 1)) return false;
                if (!action(this, x + 1, y + 1)) return false;
                break;
        }
        return true;
    }

    public bool DrawLocation(int x, int y, GridLocation loc, DrawAction<T> action)
    {
        switch (loc)
        {
            case GridLocation.BOTTOMLEFT:
                if (!action(this, x - 1, y - 1)) return false;
                break;
            case GridLocation.BOTTOMRIGHT:
                if (!action(this, x + 1, y - 1)) return false;
                break;
            case GridLocation.DOWN:
                if (!action(this, x, y - 1)) return false;
                break;
            case GridLocation.LEFT:
                if (!action(this, x - 1, y)) return false;
                break;
            case GridLocation.RIGHT:
                if (!action(this, x + 1, y)) return false;
                break;
            case GridLocation.TOPLEFT:
                if (!action(this, x - 1, y + 1)) return false;
                break;
            case GridLocation.TOPRIGHT:
                if (!action(this, x + 1, y + 1)) return false;
                break;
            case GridLocation.UP:
                if (!action(this, x, y + 1)) return false;
                break;
        }
        return true;
    }

    public bool DrawPoints(IEnumerable<Point> points, DrawAction<T> action)
    {
        foreach (Point p in points)
        {
            if (!action(this, p.x, p.y)) return false;
        }
        return true;
    }
    #endregion
    #region Around
    // Returns list of points around that satisfy
    public List<Value2D<T>> GetPointsAround(int x, int y, bool cornered, DrawAction<T> tester)
    {
        List<Value2D<T>> ret = new List<Value2D<T>>(cornered ? 9 : 4);
        this.DrawAround(x, y, cornered, (arr, x2, y2) =>
        {
            if (tester(arr, x2, y2))
                ret.Add(new Value2D<T>(x2, y2, arr[x2, y2]));
            return true;
        });
        return ret;
    }

    // Returns list of values around that satisfy
    public List<T> GetValuesAllAround(int x, int y, bool cornered, DrawAction<T> tester)
    {
        List<T> ret = new List<T>(cornered ? 9 : 4);
        this.DrawAround(x, y, cornered, (arr, x2, y2) =>
        {
            if (tester(arr, x2, y2))
                ret.Add(arr[x, y]);
            return true;
        });
        return ret;
    }

    // Returns true if square around has
    public bool HasAround(int x, int y, bool cornered, DrawAction<T> tester)
    {
        return !this.DrawAround(x, y, cornered, new DrawAction<T>((arr, x2, y2) =>
        {
            if (tester(arr, x2, y2))
                return false; // stop drawing around
            return true; // keep drawing around
        }));
    }

    public bool GetPointAround(int x, int y, bool cornered, DrawAction<T> tester, out Value2D<T> val)
    {
        Value2D<T> ret = null;
        if (this.DrawAround(x, y, cornered, new DrawAction<T>((arr, x2, y2) =>
        {
            if (tester(arr, x2, y2))
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

    public bool GetValueAround(int x, int y, bool cornered, DrawAction<T> tester, out T val)
    {
        T ret = default(T);
        if (this.DrawAround(x, y, cornered, (arr, x2, y2) =>
        {
            ret = arr[x2, y2];
            if (tester(arr, x2, y2))
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

    public bool GetRandomValueAround(int x, int y, bool cornered, System.Random rand, DrawAction<T> tester, out T val)
    {
        List<T> options = this.GetValuesAllAround(x, y, cornered, tester);
        if (options.Count > 0)
        {
            val = options.Random(rand);
            return true;
        }
        val = default(T);
        return false;
    }

    public bool GetRandomPointAround(int x, int y, bool cornered, System.Random rand, DrawAction<T> tester, out Value2D<T> val)
    {
        List<Value2D<T>> options = this.GetPointsAround(x, y, cornered, tester);
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
    public List<Value2D<T>> GetPointsOn(int x, int y, GridDirection dir, DrawAction<T> tester)
    {
        List<Value2D<T>> ret = new List<Value2D<T>>(4);
        this.DrawDir(x, y, dir, new DrawAction<T>((arr, x2, y2) =>
        {
            if (tester(arr, x2, y2))
                ret.Add(new Value2D<T>(x2, y2, arr[x2, y2]));
            return true;
        }));
        return ret;
    }

    public List<T> GetValuesOn(int x, int y, GridDirection dir, DrawAction<T> tester)
    {
        List<T> ret = new List<T>(4);
        this.DrawDir(x, y, dir, (arr, x2, y2) =>
        {
            T t = arr[x2, y2];
            if (tester(arr, x2, y2))
                ret.Add(t);
            return true;
        });
        return ret;
    }

    public bool GetPointOn(int x, int y, GridDirection dir, DrawAction<T> tester, out Value2D<T> val)
    {
        Value2D<T> ret = null;
        if (this.DrawDir(x, y, dir, new DrawAction<T>((arr, x2, y2) =>
        {
            if (tester(arr, x2, y2))
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

    public bool GetValueOn(int x, int y, GridDirection dir, DrawAction<T> tester, out T val)
    {
        T ret = default(T);
        if (this.DrawDir(x, y, dir, (arr, x2, y2) =>
        {
            ret = arr[x2, y2];
            if (tester(arr, x2, y2))
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
    public bool AlternatesSides(int x, int y, Func<T, bool> evaluator)
    {
        bool pass = evaluator(this[x - 1, y]);
        if (pass != evaluator(this[x + 1, y])) return false;
        if (pass == evaluator(this[x, y + 1])) return false;
        if (pass == evaluator(this[x, y - 1])) return false;
        return true;
    }

    /*
     * ___                            __#
     * #__       or with opposing     #__
     * _#_                            _#_
     */
    public bool Cornered(int x, int y, Func<T, bool> evaluator, bool withOpposing = false)
    {
        bool Xpass = evaluator(this[x - 1, y]);
        if (Xpass == evaluator(this[x + 1, y])) return false;
        bool Ypass = evaluator(this[x, y - 1]);
        if (Ypass == evaluator(this[x, y + 1])) return false;
        return !withOpposing || evaluator(this[Xpass ? x + 1 : x - 1, Ypass ? y + 1 : y - 1]);
    }

    /*
     * Walk edges and if alternates more than twice, it's blocking
     */
    public bool Blocking(int x, int y, Func<T, bool> evaluator)
    {
        int count = 0;
        bool status = evaluator(this[x - 1, y - 1]); // Bottom left
        DrawAction<T> func = (arr, x2, y2) =>
        {
            if (evaluator(arr[x2, y2]) != status)
            {
                status = !status;
                return ++count > 2;
            }
            return false;
        };
        if (func(this, x, y - 1)) return true; // Bottom
        if (func(this, x + 1, y - 1)) return true; // Bottom right
        if (func(this, x + 1, y)) return true; // Right
        if (func(this, x + 1, y + 1)) return true; // Top Right
        if (func(this, x, y + 1)) return true; // Top
        if (func(this, x - 1, y + 1)) return true; // Top Left
        if (func(this, x - 1, y)) return true; // Left
        return false;
    }
    #endregion
    #endregion
    #region Lines
    public bool DrawLine(int from, int to, int on, bool horizontal, DrawAction<T> action)
    {
        if (horizontal)
            return this.DrawRow(from, to, on, action);
        else
            return this.DrawCol(from, to, on, action);
    }

    public bool DrawCol(int yb, int yt, int x, DrawAction<T> action, bool BottomToTop = true)
    {
        if (BottomToTop)
        {
            for (; yb <= yt; yb++)
                if (!action(this, x, yb)) return false;
        }
        else
        {
            for (; yb <= yt; yt--)
                if (!action(this, x, yt)) return false;
        }
        return true;
    }

    public bool DrawRow(int xl, int xr, int y, DrawAction<T> action, bool LeftToRight = true)
    {
        if (LeftToRight)
        {
            for (; xl <= xr; xl++)
                if (!action(this, xl, y)) return false;
        }
        else
        {
            for (; xl <= xr; xr--)
                if (!action(this, xr, y)) return false;
        }
        return true;
    }
    #endregion
    #region Circles
    /*
     * Uses Bressenham's Midpoint Algo
     */
    public bool DrawCircle(int centerX, int centerY, int radius, StrokedAction<T> action)
    {
        var stroke = action.StrokeAction;
        if (stroke == null)
            return DrawCircleNoStroke(centerX, centerY, radius, action);

        int radiusError = 3 - (2 * radius);
        int x = 0;
        int y = radius;
        int lastYWidth = 0;

        while (x <= y)
        {
            // Draw rows from center extending up/down

            if (!stroke(this, centerX - y, centerY + x)) return false;
            if (!stroke(this, centerX - y, centerY - x)) return false;
            if (!stroke(this, centerX + y, centerY + x)) return false;
            if (!stroke(this, centerX + y, centerY - x)) return false;
            if (!stroke(this, centerX - x, centerY + y)) return false;
            if (!stroke(this, centerX - x, centerY - y)) return false;
            if (!stroke(this, centerX + x, centerY + y)) return false;
            if (!stroke(this, centerX + x, centerY - y)) return false;
            if (!this.DrawRow(centerX - y + 1, centerX + y - 1, centerY + x, action)) return false;
            if (!this.DrawRow(centerX - y + 1, centerX + y - 1, centerY - x, action)) return false;
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
                    if (!this.DrawRow(centerX - lastYWidth, centerX + lastYWidth, centerY + y, action)) return false;
                    if (!this.DrawRow(centerX - lastYWidth, centerX + lastYWidth, centerY - y, action)) return false;
                }
                y--;
                lastYWidth = x;
            }
            x++;
        }
        return true;
    }

    public bool DrawCircleNoStroke(int centerX, int centerY, int radius, StrokedAction<T> action)
    {
        int radiusError = 3 - (2 * radius);
        int x = 0;
        int y = radius;

        while (x <= y)
        {
            // Draw rows from center extending up/down
            if (!this.DrawRow(centerX - y, centerX + y, centerY + x, action)) return false;
            if (!this.DrawRow(centerX - y, centerX + y, centerY - x, action)) return false;
            if (radiusError < 0)
            {
                radiusError += (4 * x) + 6;
            }
            else
            {
                radiusError += 4 * (x - y) + 10;
                // Draw rows from top/bottom only when y is about to change.
                if (!this.DrawRow(centerX - x, centerX + x, centerY + y, action)) return false;
                if (!this.DrawRow(centerX - x, centerX + x, centerY - y, action)) return false;
                y--;
            }
            x++;
        }
        return true;
    }
    #endregion
    #region Squares
    public bool DrawSquare(int xl, int xr, int yb, int yt, StrokedAction<T> action)
    {
        if (action.StrokeAction == null)
            return DrawSquareNoStroke(xl, xr, yb, yt, action);

        if (xl == xr && yb == yt)
            return action.StrokeAction(this, xl, yb);

        if (!this.DrawRow(xl, xr, yb, action.StrokeAction)) return false;
        if (!this.DrawRow(xl, xr, yt, action.StrokeAction)) return false;
        yb++;
        yt--;
        if (!this.DrawCol(yb, yt, xl, action.StrokeAction)) return false;
        if (!this.DrawCol(yb, yt, xr, action.StrokeAction)) return false;
        xl++;
        xr--;
        if (action.UnitAction != null)
            if (!this.DrawSquareNoStroke(xl, xr, yb, yt, action.UnitAction)) return false;
        return true;
    }

    public bool DrawSquareNoStroke(int xl, int xr, int yb, int yt, StrokedAction<T> action)
    {
        if (xl == xr && yb == yt)
            return action.UnitAction(this, xl, yb);

        for (; yb <= yt; yb++)
            if (!this.DrawRow(xl, xr, yb, action)) return false;
        return true;
    }

    public bool DrawSquare(int x, int y, int radius, StrokedAction<T> action)
    {
        return this.DrawSquare(x - radius, x + radius, y - radius, y + radius, action);
    }
    #endregion
    #region Expand
    public bool DrawSquareSpiral(int x, int y, DrawAction<T> draw, Bounding bounds = null)
    {
        Bounding arrBound = Bounding;
        if (bounds == null)
            bounds = arrBound;
        else
            bounds.IntersectBounds(arrBound);
        if (!bounds.IsValid() || !bounds.Contains(x, y)) return true;

        if (!draw(this, x, y)) return false;
        Bounding currentBounds = new Bounding() { XMin = x - 1, XMax = x + 1, YMin = y - 1, YMax = y + 1 };
        while (!currentBounds.Contains(arrBound))
        {
            if (currentBounds.YMin >= bounds.YMin)
            {
                if (!this.DrawRow(currentBounds.XMin + 1, currentBounds.XMax - 1, currentBounds.YMin, draw)) return false;
                currentBounds.YMin--;
            }
            if (currentBounds.XMax <= bounds.XMax)
            {
                if (!this.DrawCol(currentBounds.YMin + 1, currentBounds.YMax - 1, currentBounds.XMax, draw)) return false;
                currentBounds.XMax++;
            }
            if (currentBounds.YMax <= bounds.YMax)
            {
                if (!this.DrawRow(currentBounds.XMin + 1, currentBounds.XMax - 1, currentBounds.YMax, draw, false)) return false;
                currentBounds.YMax++;
            }
            if (currentBounds.XMin >= bounds.XMin)
            {
                if (!this.DrawCol(currentBounds.YMin + 1, currentBounds.YMax - 1, currentBounds.XMin, draw, false)) return false;
                currentBounds.XMin--;
            }
        }
        return true;
    }
    #endregion
    #region Find Options
    public List<Bounding> GetSquares(int width, int height, bool tryFlipped, StrokedAction<T> tester, Bounding scope = null)
    {
        SquareFinder<T> finder = new SquareFinder<T>(this, width, height, tryFlipped, tester, scope);
        return finder.Find();
    }
    #endregion
    #region Searches
    public Stack<Value2D<T>> DrawDepthFirstSearch(int x, int y,
        DrawAction<T> allowedSpace,
        DrawAction<T> target,
        System.Random rand,
        bool edgeSafe = false)
    {
        #region DEBUG
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps) && BigBoss.Debug.logging(Logs.LevelGen) && typeof(T) == typeof(GridType))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Depth First Search");
            BigBoss.Debug.w(Logs.LevelGen, "Starting from (" + x + "," + y + ")");
            Array2D<GridType> tmpArr = new Array2D<GridType>(Bounding);
            foreach (Value2D<T> val in this)
                tmpArr[val] = (GridType)(object)this[val];
            tmpArr[x, y] = GridType.INTERNAL_RESERVED_CUR;
            tmpArr.ToLog(Logs.LevelGen, "Starting Map:");
        }
        #endregion
        Container2D<bool> blockedPoints = Container2D<bool>.CreateArrayFromBounds<T>(this);
        var pathTaken = new Stack<Value2D<T>>();
        DrawAction<T> filter = new DrawAction<T>((arr, x2, y2) =>
        {
            return !blockedPoints[x2, y2] && allowedSpace(arr, x2, y2);
        });
        DrawAction<T> foundTarget = new DrawAction<T>((arr, x2, y2) =>
        {
            return !blockedPoints[x2, y2] && target(arr, x2, y2);
        });
        if (edgeSafe && this is Array2DRaw<T>)
        {
            filter = filter.And(Draw.NotEdgeOfArray<T>());
            foundTarget = foundTarget.And(Draw.NotEdgeOfArray<T>());
        }
        Value2D<T> curPoint;
        Value2D<T> targetDir;

        // Push start point onto path
        pathTaken.Push(new Value2D<T>(x, y));
        blockedPoints[x, y] = true;
        while (pathTaken.Count > 0)
        {
            curPoint = pathTaken.Peek();

            // If found target, return path we took
            if (this.GetPointAround(curPoint.x, curPoint.y, false, foundTarget, out targetDir))
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
            if (this.GetRandomPointAround(curPoint.x, curPoint.y, false, rand, filter, out targetDir))
            {
                #region DEBUG
                if (BigBoss.Debug.Flag(DebugManager.DebugFlag.SearchSteps) && BigBoss.Debug.logging(Logs.LevelGen))
                {
                    BigBoss.Debug.w(Logs.LevelGen, "Chose Direction: " + targetDir);
                }
                #endregion
                curPoint = targetDir;
                pathTaken.Push(curPoint);
                blockedPoints[curPoint] = true;
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

    public bool DrawBreadthFirstFill(int x, int y,
        bool cornered,
        DrawAction<T> shouldQueue,
        DrawAction<T> shouldContinue,
        out Queue<Value2D<T>> endingQueue,
        out Container2D<bool> endingVisited)
    {
        endingQueue = new Queue<Value2D<T>>();
        endingQueue.Enqueue(new Value2D<T>(x, y, this[x, y]));
        endingVisited = Container2D<bool>.CreateArrayFromBounds(this);
        endingVisited[x, y] = true;
        return DrawBreadthFirstFill(endingQueue,
            endingVisited,
            cornered,
            shouldQueue,
            shouldContinue);
    }

    public bool DrawBreadthFirstFill(
        Queue<Value2D<T>> queue,
        Container2D<bool> visited,
        bool cornered,
        DrawAction<T> shouldQueue,
        DrawAction<T> shouldContinue = null)
    {
        #region DEBUG
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps) && BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Breadth First Fill");
            MultiMap<GridType> queueMap = new MultiMap<GridType>();
            foreach (Point p in queue)
                queueMap[p] = GridType.INTERNAL_RESERVED_BLOCKED;
            queueMap.ToLog("Starting queue");
            visited.ToLog("Starting Visited");
        }
        #endregion
        Value2D<T> curPoint = null;
        bool pass = true;
        while (queue.Count > 0)
        {
            curPoint = queue.Dequeue();
            #region DEBUG
            if (BigBoss.Debug.Flag(DebugManager.DebugFlag.SearchSteps) && BigBoss.Debug.Flag(DebugManager.DebugFlag.BFSSteps) && BigBoss.Debug.logging(Logs.LevelGen))
            {
                MultiMap<GridType> tmpMap = new MultiMap<GridType>();
                foreach (Value2D<bool> val in visited)
                {
                    tmpMap[val] = GridType.INTERNAL_RESERVED_BLOCKED;
                }
                foreach (Value2D<T> val in queue)
                {
                    tmpMap[val] = GridType.INTERNAL_RESERVED_CUR;
                }
                tmpMap[curPoint] = GridType.INTERNAL_RESERVED_CUR;
                tmpMap.ToLog("At " + curPoint);
            }
            #endregion
            if (!DrawAround(curPoint.x, curPoint.y, cornered, (arr, x2, y2) =>
            {
                if (!visited[x2, y2])
                {
                    if (shouldQueue(arr, x2, y2))
                    {
                        queue.Enqueue(new Value2D<T>(x2, y2, arr[x2, y2]));
                        #region DEBUG
                        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.SearchSteps) && BigBoss.Debug.Flag(DebugManager.DebugFlag.BFSSteps) && BigBoss.Debug.logging(Logs.LevelGen))
                        {
                            BigBoss.Debug.w(Logs.LevelGen, "Queued " + x2 + " " + y2);
                        }
                        #endregion
                    }
                    else if (shouldContinue != null && !shouldContinue(arr, x2, y2))
                    {
                        #region DEBUG
                        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.SearchSteps) && BigBoss.Debug.Flag(DebugManager.DebugFlag.BFSSteps) && BigBoss.Debug.logging(Logs.LevelGen))
                        {
                            if (x2 == 45 && y2 == 22)
                            {
                                int wer = 23;
                                wer++;
                            }
                            BigBoss.Debug.w(Logs.LevelGen, "Stopping early at " + x2 + " " + y2);
                        }
                        #endregion
                        return false;
                    }
                }
                visited[x2, y2] = true;
                return true;
            }))
            {
                // Stopping early
                pass = false;
                break;
            }
        }
        #region DEBUG
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps) && BigBoss.Debug.logging(Logs.LevelGen))
        {
            MultiMap<GridType> tmpMap = new MultiMap<GridType>();
            foreach (Value2D<bool> val in visited)
            {
                tmpMap[val] = GridType.INTERNAL_RESERVED_BLOCKED;
            }
            foreach (Value2D<T> val in queue)
            {
                tmpMap[val] = GridType.INTERNAL_RESERVED_CUR;
            }
            if (curPoint != null)
            {
                tmpMap[curPoint] = GridType.INTERNAL_RESERVED_CUR;
            }
            tmpMap.ToLog("Ending at " + curPoint);
            BigBoss.Debug.printFooter(Logs.LevelGen, "Breadth First Fill");
        }
        #endregion
        return pass;
    }

    public bool DrawBreadthFirstSearch(
        Queue<Value2D<T>> queue,
        Container2D<bool> visited,
        bool cornered,
        DrawAction<T> shouldQueue,
        DrawAction<T> goal,
        out Value2D<T> found)
    {
        found = new Value2D<T>();
        DrawAction<T> shouldContinue = Draw.Not(goal).IfNotThen(Draw.Set(found).And(Draw.Stop<T>()));
        return !DrawBreadthFirstFill(queue, visited, cornered, shouldQueue, shouldContinue);
    }

    public bool DrawBreadthFirstSearch(
        int x, int y,
        bool cornered,
        DrawAction<T> shouldQueue,
        DrawAction<T> goal,
        out Value2D<T> found,
        out Queue<Value2D<T>> endingQueue,
        out Container2D<bool> endingVisited)
    {
        endingQueue = new Queue<Value2D<T>>();
        endingQueue.Enqueue(new Value2D<T>(x, y, this[x, y]));
        endingVisited = Container2D<bool>.CreateArrayFromBounds(this);
        endingVisited[x, y] = true;
        return DrawBreadthFirstSearch(
            endingQueue,
            endingVisited,
            cornered,
            shouldQueue,
            goal,
            out found);
    }

    public void DrawPerimeter(DrawAction<T> isInsideTest, StrokedAction<T> action, bool cornered = true)
    {
        DrawAction<T> call;
        Container2D<bool> debugArr;
        if (action.UnitAction != null && action.StrokeAction != null)
        {
            call = (arr, x, y) =>
            {
                if (arr.DrawAround(x, y, cornered, isInsideTest))
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
                if (arr.DrawAround(x, y, cornered, isInsideTest))
                    action.UnitAction(arr, x, y);
                return true;
            };
        }
        else
        {
            call = (arr, x, y) =>
            {
                if (!arr.DrawAround(x, y, cornered, isInsideTest))
                    action.StrokeAction(arr, x, y);
                return true;
            };
        }
        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.Perimeter) && BigBoss.Debug.Flag(DebugManager.DebugFlag.SearchSteps))
        {
            debugArr = new Array2D<bool>(this.Bounding);
            call = new DrawAction<T>(call).And((arr, x, y) =>
            {
                debugArr[x, y] = true;
                debugArr.ToLog("Draw Perimeter on " + x + " " + y);
                return true;
            });
        }
        this.DrawAll(call);
    }

    public List<Value2D<T>> DrawJumpTowardsSearch(int x, int y,
        int minJump,
        int maxJump,
        DrawAction<T> allowedSpace,
        DrawAction<T> target,
        System.Random rand,
        Point gravityPt,
        bool hugCorners,
        bool edgeSafe = false)
    {
        JumpTowardsSearcher<T> searcher = new JumpTowardsSearcher<T>(this, x, y,
            minJump, maxJump,
            allowedSpace, target,
            rand, gravityPt, hugCorners, edgeSafe);
        Stack<List<Value2D<T>>> stack = searcher.Find();
        Stack<List<Value2D<T>>> tmp = new Stack<List<Value2D<T>>>(stack.Count);
        int count = 0;
        foreach (List<Value2D<T>> val in stack)
        {
            count += val.Count;
            tmp.Push(val);
        }
        List<Value2D<T>> ret = new List<Value2D<T>>(count);
        foreach (List<Value2D<T>> val in tmp)
        {
            ret.AddRange(val);
        }
        return ret;
    }
    #endregion
    #endregion
}
