using System;
using System.Collections;
using System.Collections.Generic;

public class Surrounding<T> : IEnumerable<Value2D<T>>
{
    private static readonly int arrLen = Enum.GetNames(typeof (GridLocation)).Length;
    private readonly Value2D<T>[] dirs = new Value2D<T>[arrLen];

    public int Count
    {
        get { return CountInternal(); }
    }

    public PassFilter<Value2D<T>> Filter { get; set; }
    protected T[,] Arr;
    protected int CurX;
    protected int CurY;
    protected int Up;
    protected int Down;
    protected int Left;
    protected int Right;
    public bool Cornered { get; set; }

    public Surrounding(T[,] srcArr)
        : this(srcArr, false)
    {
    }

    public Surrounding(T[,] srcArr, bool corners)
    {
        Arr = srcArr;
        Cornered = corners;
    }

    public Value2D<T> this[GridLocation loc]
    {
        get
        {
            if (dirs[(int) loc] == null)
            {
                Get(loc);
            }
            return dirs[(int) loc];
        }
        set
        {
            dirs[(int) loc] = value;
            Arr[value.y, value.x] = value.val;
        }
    }

    public void Clear()
    {
        for (int i = 0; i < dirs.Length; i++)
        {
            dirs[i] = null;
        }
    }

    protected void FilterExec(PassFilter<Value2D<T>> filter)
    {
        if (filter != null)
        {
            for (int i = 0; i < dirs.Length; i++)
            {
                if (!filter.pass(dirs[i]))
                {
                    dirs[i] = null;
                }
            }
        }
    }

    private Value2D<T> Get(GridLocation loc)
    {
        // Assume we are not on the edge
        switch (loc)
        {
            case GridLocation.DOWN:
                return new Value2D<T>(CurX, Down, Arr[CurX, Down]);
            case GridLocation.LEFT:
                return new Value2D<T>(Left, CurY, Arr[Left, CurY]);
            case GridLocation.UP:
                return new Value2D<T>(CurX, Up, Arr[CurX, Up]);
            case GridLocation.RIGHT:
                return new Value2D<T>(Right, CurY, Arr[Right, CurY]);
            case GridLocation.BOTTOMLEFT:
                return Cornered ? new Value2D<T>(Left, Down, Arr[Left, Down]) : null;
            case GridLocation.BOTTOMRIGHT:
                return Cornered ? new Value2D<T>(Right, Down, Arr[Right, Down]) : null;
            case GridLocation.TOPRIGHT:
                return Cornered ? new Value2D<T>(Right, Up, Arr[Right, Up]) : null;
            case GridLocation.TOPLEFT:
                return Cornered ? new Value2D<T>(Left, Up, Arr[Left, Up]) : null;
        }
        return null;
    }

    public void Load(Value2D<T> val)
    {
        Load(val.x, val.y);
    }

    public void Load(int x, int y)
    {
        Clear();

        CurX = x;
        CurY = y;
        Left = x - 1;
        Right = x + 1;
        Up = y + 1;
        Down = y - 1;
        if (x <= 1
            || y <= 1
            || Right >= Arr.GetLength(1)
            || Up >= Arr.GetLength(0))
        {
            // On edge of array.  Handle all outputs now, so normal
            // non-edge queries don't have to worry and check

            if (Left >= 0)
            {
                this[GridLocation.LEFT] = new Value2D<T>(Left, y, Arr[y, Left]);
            }
            if (Right < Arr.GetLength(1))
            {
                this[GridLocation.RIGHT] = new Value2D<T>(Right, y, Arr[y, Right]);
            }
            if (Down >= 0)
            {
                this[GridLocation.DOWN] = new Value2D<T>(x, Down, Arr[Down, x]);
            }
            if (Up < Arr.GetLength(0))
            {
                this[GridLocation.UP] = new Value2D<T>(x, Up, Arr[Up, x]);
            }
            if (Cornered)
            {
                if (Left >= 0 && Down >= 0)
                {
                    this[GridLocation.BOTTOMLEFT] = new Value2D<T>(Left, Down, Arr[Down, Left]);
                }
                if (Left >= 0 && Up < Arr.GetLength(0))
                {
                    this[GridLocation.TOPLEFT] = new Value2D<T>(Left, Up, Arr[Up, Left]);
                }
                if (Right < Arr.GetLength(1) && Down >= 0)
                {
                    this[GridLocation.BOTTOMRIGHT] = new Value2D<T>(Right, Down, Arr[Down, Right]);
                }
                if (Right < Arr.GetLength(1) && Up < Arr.GetLength(0))
                {
                    this[GridLocation.TOPRIGHT] = new Value2D<T>(Right, Up, Arr[Up, Right]);
                }
            }
            FilterExec(Filter);
        }
    }

    public virtual List<Value2D<T>> GetDirsWithVal(bool with, T t)
    {
        var ret = new List<Value2D<T>>();
        foreach (var val in this)
        {
            if (val != null && val.val.Equals(t) == with)
            {
                ret.Add(val);
            }
        }
        return ret;
    }

    public virtual List<Value2D<T>> GetDirsWithVal(bool with, HashSet<T> set)
    {
        var ret = new List<Value2D<T>>();
        foreach (var val in this)
        {
            if (val != null && set.Contains(val.val) == with)
            {
                ret.Add(val);
            }
        }
        return ret;
    }

    public virtual List<Value2D<T>> GetDirsWithVal(bool with, params T[] types)
    {
        var ret = new List<Value2D<T>>();
        foreach (var val in this)
        {
            if (val != null && types.Contains(val.val) == with)
            {
                ret.Add(val);
            }
        }
        return ret;
    }

    public virtual Value2D<T> GetDirWithVal(bool with, T t)
    {
        List<Value2D<T>> list = GetDirsWithVal(with, t);
        if (list.Count > 0)
            return list[0];
        return null;
    }

    public virtual Value2D<T> GetDirWithVal(bool with, HashSet<T> t)
    {
        List<Value2D<T>> list = GetDirsWithVal(with, t);
        if (list.Count > 0)
            return list[0];
        return null;
    }

    public virtual Value2D<T> GetDirWithVal(bool with, params T[] t)
    {
        List<Value2D<T>> list = GetDirsWithVal(with, t);
        if (list.Count > 0)
            return list[0];
        return null;
    }

    // True if current space is cornered by values given.
    // Cornered means a block is present vertically and horizontally.
    public virtual bool IsCorneredBy(HashSet<T> by)
    {
        return GetNeighbor(GridDirection.HORIZ, by) != null
               && GetNeighbor(GridDirection.VERT, by) != null;
    }

    // True if current space is cornered by values given.
    // Cornered means a block is present vertically and horizontally.
    public bool IsCorneredBy(params T[] by)
    {
        return IsCorneredBy(new HashSet<T>(by));
    }

    public Value2D<T> GetNeighbor(GridDirection d, params T[] ofType)
    {
        return GetNeighbor(d, new HashSet<T>(ofType));
    }

    public Value2D<T> GetNeighbor(GridDirection d, HashSet<T> ofType)
    {
        List<Value2D<T>> neighbors = GetNeighbors(d, ofType);
        if (neighbors.Count > 0)
        {
            return neighbors[0];
        }
        return null;
    }

    public Value2D<T> GetNeighbor(GridDirection d, T t)
    {
        IEnumerator<Value2D<T>> en = GetEnumerator(d);
        while (en.MoveNext())
        {
            if (t.Equals(en.Current.val))
            {
                return en.Current;
            }
        }
        return null;
    }

    public List<Value2D<T>> GetNeighbors(GridDirection d, T t)
    {
        var ret = new List<Value2D<T>>();
        IEnumerator<Value2D<T>> en = GetEnumerator(d);
        while (en.MoveNext())
        {
            if (t.Equals(en.Current.val))
            {
                ret.Add(en.Current);
            }
        }
        return ret;
    }

    public List<Value2D<T>> GetNeighbors(GridDirection d, params T[] ofType)
    {
        return GetNeighbors(d, new HashSet<T>(ofType));
    }

    public List<Value2D<T>> GetNeighbors(GridDirection d, HashSet<T> ofType)
    {
        var ret = new List<Value2D<T>>();
        IEnumerator<Value2D<T>> en = GetEnumerator(d);
        while (en.MoveNext())
        {
            if (ofType.Contains(en.Current.val))
            {
                ret.Add(en.Current);
            }
        }
        return ret;
    }

    public Value2D<T> GetRandom(Random rand)
    {
        var options = new List<Value2D<T>>();
        foreach (var val in this)
        {
            options.Add(val);
        }
        if (options.Count != 0)
        {
            return options[rand.Next(options.Count)];
        }
        return null;
    }

    private int CountInternal()
    {
        int ret = 0;
        IEnumerator<Value2D<T>> e = GetEnumerator();
        while (e.MoveNext())
        {
            ret++;
        }
        return ret;
    }

    public IEnumerator<Value2D<T>> GetEnumerator()
    {
        foreach (var val in dirs)
        {
            if (val != null)
            {
                yield return val;
            }
        }
    }

    public IEnumerator<Value2D<T>> GetEnumerator(GridDirection d)
    {
        foreach (GridLocation loc in Enum.GetValues(typeof (GridLocation)))
        {
            if (loc.PartOf(d) && this[loc] != null)
            {
                yield return this[loc];
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
