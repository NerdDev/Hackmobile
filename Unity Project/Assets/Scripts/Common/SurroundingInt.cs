using UnityEngine;
using System.Collections;

public class SurroundingInt : Surrounding<int> {
	
    public SurroundingInt(int[,] arr)
        : base(arr)
    {
        
    }

	public Value2D<int> GetDirWithVal(int t)
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
	
	public Value2D<int> GetDirWithoutVal(int t)
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
        Value2D<int> ret = null;
        int lastDiff = 0;
		foreach (Value2D<int> val in this)
        {
            if (val != null) // Exists
            {
                int valDiff = Mathf.Abs(initVal - val.val);
                if (val.val != 0 // Not zero (regarded as null value)
                    && valDiff > diff // Diff meets requirements
                    && (ret == null || lastDiff < valDiff) // Larger than last found diff
                    )
                {
                    lastDiff = valDiff;
                    ret = val;
                }
            }
		}
		return ret;
	}
}
