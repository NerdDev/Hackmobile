using UnityEngine;
using System.Collections;

public class SurroundingInt : Surrounding<int> {
	
    public static SurroundingInt Get(Array2D<int> arr, int x, int y)
    {
        return Get(arr.GetArr(), x, y);
    }

	public static new SurroundingInt Get(int[,] arr, int x, int y)
	{
        SurroundingInt ret = new SurroundingInt();
		x -= 1;
		if (x >= 0 && x < arr.GetLength(1))
		{
			ret.left = new Value2D<int>(x, y, arr[y,x]);	
		}
		x += 2;
		if (x >= 0 && x < arr.GetLength(1))
		{
            ret.right = new Value2D<int>(x, y, arr[y, x]);	
		}
		x -= 1;
		y -= 1;
		if (y >= 0 && y < arr.GetLength(0))
		{
            ret.down = new Value2D<int>(x, y, arr[y, x]);	
		}
		y += 2;
		if (y >= 0 && y < arr.GetLength(0))
		{
            ret.up = new Value2D<int>(x, y, arr[y, x]);	
		}
		return ret;
	}
	
	public override Value2D<int> GetDirWithVal(int t)
	{
		foreach (Value2D<int> val in this)
		{
			if (val != null && val.val == t)
			{
				return val;	
			}
		}
		return null;
	}
	
	public override Value2D<int> GetDirWithoutVal(int t)
	{
		foreach (Value2D<int> val in this)
		{
			if (val != null && val.val != t)
			{
				return val;	
			}
		}
		return null;
	}
	
	public Value2D<int> GetDirWithValDiff(int initVal, int diff)
	{
		foreach (Value2D<int> val in this)
		{
			if (val != null && (val.val - initVal == diff || initVal - val.val == diff))
			{
				return val;	
			}
		}
		return null;
	}
	
	public Value2D<int> GetDirWithValDiffLarger(int initVal, int diff)
	{
		foreach (Value2D<int> val in this)
		{
			if (val != null && val.val != 0 && Mathf.Abs(val.val - initVal) > diff)
			{
				return val;	
			}
		}
		return null;
	}
}
