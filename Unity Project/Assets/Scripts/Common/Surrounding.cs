using System;
using System.Collections.Generic;

public class Surrounding<T> : IEnumerable<Value2D<T>>
{
	public Value2D<T> up { get; set; }
    public Value2D<T> down { get; set; }
    public Value2D<T> left { get; set; }
    public Value2D<T> right { get; set; }
	
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

    public IEnumerator<Value2D<T>> GetEnumerator()
    {
        yield return up;
        yield return right;
        yield return down;
        yield return left;
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}
