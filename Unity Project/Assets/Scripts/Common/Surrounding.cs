using System;
using System.Collections.Generic;

public class Surrounding<T> : IEnumerable<Value2D<T>>
{
    Value2D<T>[] dirs = new Value2D<T>[Enum.GetNames(typeof(GridLocation)).Length];
    public int Count {
        get { return CountInternal(); }
    }
    public PassFilter<Value2D<T>> Filter { get; set; } 
    public T[,] arr;
	
    public Surrounding(T[,] srcArr)
    {
        arr = srcArr;
    }

    public Value2D<T> this[GridLocation loc]
    {
        get { return dirs[(int) loc]; }
        set { dirs[(int) loc] = value; }
    }

    public void Clear()
    {
        for (int i = 0; i < dirs.Length; i++)
        {
            dirs[i] = null;
        }
    }

    public void FilterExec(PassFilter<Value2D<T>> filter)
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

    public void Load(Value2D<T> val)
    {
        Load(val.x, val.y);
    }

    public void Load(int x, int y)
    {
        EdgePosition xPos = GetPos(x, arr.GetLength(1));
        EdgePosition yPos = GetPos(y, arr.GetLength(0));

        Clear();
        if (xPos == EdgePosition.Out || yPos == EdgePosition.Out)
        { // Bad Query
            return;
        }

        // Create Values
        if (xPos != EdgePosition.BottomEdge)
        {
            this[GridLocation.LEFT] = new Value2D<T>(x - 1, y, arr[y, x - 1]);
        }
        if (xPos != EdgePosition.TopEdge)
        {
            this[GridLocation.RIGHT] = new Value2D<T>(x + 1, y, arr[y, x + 1]);
        }
        if (yPos != EdgePosition.BottomEdge)
        {
            this[GridLocation.DOWN] = new Value2D<T>(x, y - 1, arr[y - 1, x]);
        }
        if (yPos != EdgePosition.TopEdge)
        {
            this[GridLocation.UP] = new Value2D<T>(x, y + 1, arr[y + 1, x]);
        }

        FilterExec(Filter);
    }

    static EdgePosition GetPos(int val, int arrLim)
    {
        arrLim -= 1;
        if (val > 0 && val < arrLim)
        {
            return EdgePosition.In;
        }
        if (val == 0)
        {
            return EdgePosition.BottomEdge;
        }
        if (val == arrLim)
        {
            return EdgePosition.TopEdge;
        }
        return EdgePosition.Out;
    }

    private enum EdgePosition
    {
        Out,
        BottomEdge,
        In,
        TopEdge
    }

    // Returns a direction containing the given value.
    // Null if none found.
    public virtual Value2D<T> GetDirWithVal(T t)
	{
		foreach (Value2D<T> val in this)
		{
            if (val != null && val.val.Equals(t))
			{
				return val;	
			}
		}
		return null;
	}

    // Returns a direction containing one of the given values.
    // Null if none found.
    public virtual Value2D<T> GetDirWithVal(HashSet<T> set)
    {
        foreach (Value2D<T> val in this)
        {
            if (val != null && set.Contains(val.val))
            {
                return val;
            }
        }
        return null;
    }

    public virtual List<Value2D<T>> GetDirsWithVal(HashSet<T> set)
    {
        List<Value2D<T>> ret = new List<Value2D<T>>();
        foreach (Value2D<T> val in this)
        {
            if (val != null && set.Contains(val.val))
            {
                ret.Add(val);
            }
        }
        return ret;
    }

    // Returns a direction containing one of the given values that also passes the filter.
    // Null if none found.
    public virtual Value2D<T> GetDirWithVal(params T[] types)
    {
        return GetDirWithVal(new HashSet<T>(types));
    }

    public virtual List<Value2D<T>> GetDirsWithVal(params T[] types)
    {
        return GetDirsWithVal(new HashSet<T>(types));
    }
	
    // Returns a direction that does not contain given value.
    // Null if none found.
	public virtual Value2D<T> GetDirWithoutVal(T t)
	{
		foreach (Value2D<T> val in this)
		{
			if (val != null && !val.val.Equals(t))
			{
				return val;	
			}
		}
		return null;
	}

    public virtual List<Value2D<T>> GetDirsWithoutVal(T t)
    {
        List<Value2D<T>> ret = new List<Value2D<T>>();
        foreach (Value2D<T> val in this)
        {
            if (val != null && !val.val.Equals(t))
            {
                ret.Add(val);
            }
        }
        return ret;
    }


    // Returns a direction not contained in the given set, that also passes the filter.
    // Null if none found.
    public virtual Value2D<T> GetDirWithoutVal(params T[] types)
    {
        return GetDirWithoutVal(new HashSet<T>(types));
    }

    public virtual List<Value2D<T>> GetDirsWithoutVal(params T[] types)
    {
        return GetDirsWithoutVal(new HashSet<T>(types));
    }

    // Returns a direction not contained in the given set, that also passes the filter.
    // Null if none found.
    public virtual Value2D<T> GetDirWithoutVal(HashSet<T> set)
    {
        foreach (Value2D<T> val in this)
        {
            if (val != null && !set.Contains(val.val))
            {
                return val;
            }
        }
        return null;
    }

    public virtual List<Value2D<T>> GetDirsWithoutVal(HashSet<T> set)
    {
        List<Value2D<T>> ret = new List<Value2D<T>>();
        foreach (Value2D<T> val in this)
        {
            if (val != null && !set.Contains(val.val))
            {
                ret.Add(val);
            }
        }
        return ret;
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
        List<Value2D<T>> ret = new List<Value2D<T>>();
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
        List<Value2D<T>> ret = new List<Value2D<T>>();
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
        List<Value2D<T>> options = new List<Value2D<T>>();
        foreach (Value2D<T> val in this)
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
        foreach (Value2D<T> val in dirs)
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

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}
