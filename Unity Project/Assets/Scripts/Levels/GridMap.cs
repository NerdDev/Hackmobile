using UnityEngine;
using System.Collections;

public class GridMap : Array2Dcoord<GridType> {

    #region Ctors
    public GridMap(int width, int height) : base(width, height)
    {
    }

    public GridMap(GridMap rhs) 
        : base(rhs.arr.GetLength(1), rhs.arr.GetLength(0))
    {
        PutAll(rhs);
    }

    public GridMap(GridMap rhs, Point shift)
        : base(rhs.arr.GetLength(1), rhs.arr.GetLength(0))
    {
        PutAll(rhs, shift);
    }

    public GridMap(GridMap rhs, int xShift, int yShift)
        : base(rhs.arr.GetLength(1), rhs.arr.GetLength(0))
    {
        PutAll(rhs, xShift, yShift);
    }
	
	public GridMap(Bounding bounds) : base(bounds)
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
}
