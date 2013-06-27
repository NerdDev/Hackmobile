using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridArray : Array2D<GridType> {

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
	
	public GridArray(Bounding bounds, bool minimize) : base(bounds, minimize)
	{
	}

    public GridArray(GridType[,] arr)
    {
        this.arr = arr;
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
	
	public void PutAll(LayoutObject obj, Bounding origBound)
	{
		Point shift = obj.GetShift();
		shift.x -= origBound.xMin;
		shift.y -= origBound.yMin;
		base.PutAll (obj.GetArray(), shift);	
	}
	
	public override Bounding GetBounding ()
    {
        GridType[,] array = GetArr();
        Bounding ret = new Bounding();
        for (int y = 0; y < array.GetLength(0); y++)
        {
            for (int x = 0; x < array.GetLength(1); x++)
            {
                if (array[y, x] != GridType.NULL)
                {
                    ret.absorb(x, y);
                }
            }
        }
        return ret;
	}
	
    public Bounding GetBoundingInternal()
    {
        return base.GetBounding();
    }

    public override List<string> ToRowStrings()
    {
        GridType[,] array = GetArr();
		Bounding bounds = GetBounding();
        List<string> ret = new List<string>();
        for (int y = bounds.yMax; y >= bounds.yMin; y -= 1)
        {
            string rowStr = "";
            for (int x = bounds.xMin; x <= bounds.xMax; x += 1)
            {
                rowStr += LayoutObject.getAscii(array[y, x]);
            }
            ret.Add(rowStr);
        }
        return ret;
    }


    // Fills in a row with a desired value
    public override void PutRow(GridType t, int xl, int xr, int y)
    {
        for (; xl <= xr; xl++)
        {
            if (GridType.NULL == arr[y, xl])
            {
                arr[y, xl] = t;
            }
        }
    }

    // Fills in a col with a desired value
    public override void PutCol(GridType t, int y1, int y2, int x)
    {
        for (; y1 <= y2; y1++)
        {
            if (GridType.NULL == arr[y1, x])
            {
                arr[y1, x] = t;
            }
        }
    }
}
