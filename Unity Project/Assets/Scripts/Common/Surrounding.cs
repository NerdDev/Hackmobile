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
            Arr[value.x, value.y] = value.val;
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

    private Value2D<GridType> Get(GridLocation loc)
    {
        // Assume we are not on the edge
        switch (loc)
        {
            case GridLocation.DOWN:
                return new Value2D<GridType>(CurX - 1, CurY, Arr[C]);
                break;
        }
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
        if (x <= 1
            || y <= 1
            || x + 1 >= Arr.GetLength(1)
            || y + 1 >= Arr.GetLength(0)) ;
        {
            // On edge of array.  Handle all outputs now, so normal
            // non-edge queries don't have to worry and check

            int left = x - 1;
            int right = x + 1;
            int up = y + 1;
            int down = y - 1;
            if (left >= 0)
            {
                this[GridLocation.LEFT] = new Value2D<T>(left, y, Arr[y, left]);
            }
            if (right < Arr.GetLength(1))
            {
                this[GridLocation.RIGHT] = new Value2D<T>(right, y, Arr[y, right]);
            }
            if (down >= 0)
            {
                this[GridLocation.DOWN] = new Value2D<T>(x, down, Arr[down, x]);
            }
            if (up < Arr.GetLength(0))
            {
                this[GridLocation.UP] = new Value2D<T>(x, up, Arr[up, x]);
            }
            if (Cornered)
            {
                if (left >= 0 && down >= 0)
                {
                    this[GridLocation.BOTTOMLEFT] = new Value2D<T>(left, down, Arr[down, left]);
                }
                if (left >= 0 && up < Arr.GetLength(0))
                {
                    this[GridLocation.TOPLEFT] = new Value2D<T>(left, up, Arr[up, left]);
                }
                if (right < Arr.GetLength(1) && down >= 0)
                {
                    this[GridLocation.BOTTOMRIGHT] = new Value2D<T>(right, down, Arr[down, right]);
                }
                if (right < Arr.GetLength(1) && up < Arr.GetLength(0))
                {
                    this[GridLocation.TOPRIGHT] = new Value2D<T>(right, up, Arr[up, right]);
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
