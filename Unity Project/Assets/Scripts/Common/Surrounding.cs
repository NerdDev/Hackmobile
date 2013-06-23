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

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}
