using System;
using System.Collections.Generic;

public class Surrounding<T> : IEnumerable<Value2D<T>>
{
	public Value2D<T> up { get; set; }
    public Value2D<T> down { get; set; }
    public Value2D<T> left { get; set; }
    public Value2D<T> right { get; set; }
    public int Count { get; private set; }
	
    protected Surrounding ()
    {
    }

    public static Surrounding<T> Get(T[,] arr, Value2D<T> val, PassFilter<Value2D<T>> pass)
    {
        return Get(arr, val.x, val.y, pass);
    }

    public static Surrounding<T> Get(T[,] arr, Value2D<T> val)
    {
        return Get(arr, val.x, val.y, null);
    }

    public static Surrounding<T> Get(T[,] arr, int x, int y)
    {
        return Get(arr, x, y, null);
    }

    public static Surrounding<T> Get(T[,] arr, int x, int y, PassFilter<Value2D<T>> pass)
    {
        Position xPos = GetPos(x, arr.GetLength(1));
        Position yPos = GetPos(y, arr.GetLength(0));

        Surrounding<T> ret = new Surrounding<T>();

        if (xPos == Position.Out || yPos == Position.Out)
        { // Bad Query
            return ret;
        }

        // Create Values
        if (xPos != Position.BottomEdge) {
			ret.left = new Value2D<T>(x - 1, y, arr[y, x - 1]);
		}
		if (xPos != Position.TopEdge)
		{
            ret.right = new Value2D<T>(x + 1, y, arr[y, x + 1]);
		}
		if (yPos != Position.BottomEdge)
		{
            ret.down = new Value2D<T>(x, y - 1, arr[y - 1, x]);
		}
		if (yPos != Position.TopEdge)
		{
            ret.up = new Value2D<T>(x, y + 1, arr[y + 1, x]);
		}

        // Handle pass filter
        if (pass != null)
        {
            if (!pass.pass(ret.left))
            {
                ret.left = null;
            }
            if (!pass.pass(ret.right))
            {
                ret.right = null;
            }
            if (!pass.pass(ret.up))
            {
                ret.up = null;
            }
            if (!pass.pass(ret.down))
            {
                ret.down = null;
            }
        }

        ret.Count = ret.CountInternal();
		return ret;
	}

    static Position GetPos(int val, int arrLim)
    {
        arrLim -= 1;
        if (val > 0 && val < arrLim)
        {
            return Position.In;
        }
        if (val == 0)
        {
            return Position.BottomEdge;
        }
        if (val == arrLim)
        {
            return Position.TopEdge;
        }
        return Position.Out;
    }

    private enum Position
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

    // Returns a direction containing one of the given values that also passes the filter.
    // Null if none found.
    public virtual Value2D<T> GetDirWithVal(params T[] types)
    {
        return GetDirWithVal(new HashSet<T>(types));
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

    // Returns a direction not contained in the given set, that also passes the filter.
    // Null if none found.
    public virtual Value2D<T> GetDirWithoutVal(params T[] types)
    {
        return GetDirWithoutVal(new HashSet<T>(types));
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
        if (up != null)
            ret++;
        if (right != null)
            ret++;
        if (down != null)
            ret++;
        if (left != null)
            ret++;
        return ret;
    }

    public IEnumerator<Value2D<T>> GetEnumerator()
    {
        if (up != null)
            yield return up;
        if (right != null)
            yield return right;
        if (down != null)
            yield return down;
        if (left != null)
            yield return left;
    }

    public IEnumerator<Value2D<T>> GetEnumerator(GridDirection d)
    {
        if (up != null && d == GridDirection.UP || d == GridDirection.VERT)
        {
            yield return up;
        }
        if (right != null && d == GridDirection.RIGHT || d == GridDirection.HORIZ)
        {
            yield return right;
        }
        if (down != null && d == GridDirection.DOWN || d == GridDirection.VERT)
        {
            yield return down;
        }
        if (left != null && d == GridDirection.LEFT || d == GridDirection.HORIZ)
        {
            yield return left;
        }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}
