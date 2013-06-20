using UnityEngine;
using System.Collections;

public class GridMap : MultiMap<GridType> {

    #region Ctors
    public GridMap()
    {
    }

    public GridMap(GridMap rhs) : base(rhs)
    {
    }

    public GridMap(GridMap rhs, Point shift) : base(rhs, shift)
    {
    }

    public GridMap(GridMap rhs, int xShift, int yShift) : base(rhs, xShift, yShift)
    {
    }
    #endregion

    protected override void setComparator()
    {
        comparator = GridTypeComparator.get();
    }
}
