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
        putAll(rhs);
    }

    public GridMap(GridMap rhs, Point shift)
        : this(rhs)
    {
        putAll(rhs, shift);
    }

    public GridMap(GridMap rhs, int xShift, int yShift)
        : this(rhs)
    {
        putAll(rhs, xShift, yShift);
    }
    #endregion

}
