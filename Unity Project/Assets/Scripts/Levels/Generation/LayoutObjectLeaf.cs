using System.Collections;
using System.Collections.Generic;
using System;

public class LayoutObjectLeaf : LayoutObject {

    protected GridArray grids;
    public int Width { get { return grids.GetArr().GetLength(1); } }
    public int Height { get { return grids.GetArr().GetLength(0); } }

    protected LayoutObjectLeaf() : base()
    {
    }

    public LayoutObjectLeaf(int width, int height)
        : this(new GridArray(width, height))
    {
    }

    public LayoutObjectLeaf(GridArray arr) : this()
    {
        grids = arr;
    }

    #region GetSet
    public GridType get(int x, int y)
    {
        return grids[x, y];
    }

    public void put(GridType t, int x, int y)
    {
        grids[x,y] = t;
    }

    public void putAll(GridMap map)
    {
        foreach (Point<GridType> vals in map)
        {
            put(vals.val, vals.x, vals.y);
        }
    }

    public override GridArray GetArray()
    {
        return grids;
    }

    protected override Bounding GetBoundingUnshifted()
    {
        if (finalized)
        {
            return bakedBounds;
        }
		return grids.GetBounding();
	}
    #endregion GetSet

    public override void Bake(bool shiftCompensate)
    {
        Point minimizeShift = grids.Minimize(1);
        if (shiftCompensate)
        {
            ShiftP.Shift(minimizeShift);
        }
        bakedBounds = grids.GetBoundingInternal();
        finalized = true;
    }

    public override bool ContainsPoint(Point<GridType> val)
    {
        return grids.ContainsPoint(val);
    }

    public override string GetTypeString()
    {
        return "Layout Object Leaf";
    }
}
