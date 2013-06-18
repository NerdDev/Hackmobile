using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridArray : Array2Dcoord<GridType> {

    #region Ctors
    public GridArray(int width, int height) : base(width, height)
    {
    }

    public GridArray(GridArray rhs) 
        : base(rhs.arr.GetLength(1), rhs.arr.GetLength(0))
    {
        PutAll(rhs);
    }

    public GridArray(GridArray rhs, Point shift)
        : base(rhs.arr.GetLength(1), rhs.arr.GetLength(0))
    {
        PutAll(rhs, shift);
    }

    public GridArray(GridArray rhs, int xShift, int yShift)
        : base(rhs.arr.GetLength(1), rhs.arr.GetLength(0))
    {
        PutAll(rhs, xShift, yShift);
    }
	
	public GridArray(Bounding bounds) : base(bounds)
	{
		
	}
	
	protected override Comparator<GridType> getDefaultComparator ()
	{
		return GridTypeComparator.get();
	}
    #endregion
	
    public override void Put(GridType val, int x, int y)
    {
		if (val != GridType.NULL)
		{
			base.Put(val, x, y);
		}
    }

    public override List<string> ToRowStrings()
    {
        GridType[,] array = GetArr();
        List<string> ret = new List<string>();
        for (int y = array.GetLength(0) - 1; y >= 0; y -= 1)
        {
            string rowStr = "";
            for (int x = 0; x < array.GetLength(1); x += 1)
            {
                rowStr += LayoutObject.getAscii(array[y, x]);
            }
            ret.Add(rowStr);
        }
        return ret;
    }
}
