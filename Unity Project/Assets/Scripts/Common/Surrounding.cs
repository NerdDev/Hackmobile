using System;
using System.Collections.Generic;

public class Surrounding<T> : IEnumerable<Value2D<T>>
{
	public Value2D<T> up { get; set; }
    public Value2D<T> down { get; set; }
    public Value2D<T> left { get; set; }
    public Value2D<T> right { get; set; }
	
    protected Surrounding ()
    {

    }

    public static Surrounding<T> Get(T[,] arr, Value2D<T> val)
    {
        return Get(arr, val.x, val.y);
    }

	public static Surrounding<T> Get(T[,] arr, int x, int y)
	{
        Surrounding <T> ret = new Surrounding<T>();
		x -= 1;
		if (x >= 0 && x < arr.GetLength(1))
		{
			ret.left = new Value2D<T>(x, y, arr[y,x]);	
		}
		x += 2;
		if (x >= 0 && x < arr.GetLength(1))
		{
            ret.right = new Value2D<T>(x, y, arr[y, x]);	
		}
		x -= 1;
		y -= 1;
		if (y >= 0 && y < arr.GetLength(0))
		{
            ret.down = new Value2D<T>(x, y, arr[y, x]);	
		}
		y += 2;
		if (y >= 0 && y < arr.GetLength(0))
		{
            ret.up = new Value2D<T>(x, y, arr[y, x]);	
		}
		return ret;
	}
	
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

    public virtual Value2D<T> GetDirWithVal(HashSet<T> t, PassFilter<Value2D<T>> pass)
    {
        foreach (Value2D<T> val in this)
        {
            if (val != null && t.Contains(val.val) && pass.pass(val))
            {
                return val;
            }
        }
        return null;
    }
	
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

    public virtual bool IsCorneredBy(HashSet<T> by)
    {
        return GetNeighbor(GridDir.HORIZ, by) != null
            && GetNeighbor(GridDir.VERT, by) != null;
    }

    public bool IsCorneredBy(params T[] by)
    {
        return IsCorneredBy(new HashSet<T>(by));
    }

    public Value2D<T> GetNeighbor(GridDir d, params T[] ofType)
    {
        return GetNeighbor(d, new HashSet<T>(ofType));
    }

    public Value2D<T> GetNeighbor(GridDir d, HashSet<T> ofType)
    {
        List<Value2D<T>> neighbors = GetNeighbors(d, ofType);
        if (neighbors.Count > 0)
        {
            return neighbors[0];
        }
        return null;
    }

    public Value2D<T> GetNeighbor(GridDir d, T t)
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

    public List<Value2D<T>> GetNeighbors(GridDir d, T t)
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

    public List<Value2D<T>> GetNeighbors(GridDir d, params T[] ofType)
    {
        return GetNeighbors(d, new HashSet<T>(ofType));
    }

    public List<Value2D<T>> GetNeighbors(GridDir d, HashSet<T> ofType)
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

    public Value2D<T> GetRandom(Random rand, PassFilter<Value2D<T>> pass)
    {
        List<Value2D<T>> options = new List<Value2D<T>>();
        foreach (Value2D<T> val in this)
        {
            if (pass == null || pass.pass(val))
                options.Add(val);
        }
        if (options.Count != 0)
        {
            return options[rand.Next(options.Count)];
        }
        return null;
    }

    public int Count()
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

    public IEnumerator<Value2D<T>> GetEnumerator(GridDir d)
    {
        if (up != null && d == GridDir.UP || d == GridDir.VERT)
        {
            yield return up;
        }
        if (right != null && d == GridDir.RIGHT || d == GridDir.HORIZ)
        {
            yield return right;
        }
        if (down != null && d == GridDir.DOWN || d == GridDir.VERT)
        {
            yield return down;
        }
        if (left != null && d == GridDir.LEFT || d == GridDir.HORIZ)
        {
            yield return left;
        }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}
