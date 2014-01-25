using System.Collections;
using System.Collections.Generic;
using System;

public class LayoutObjectLeaf : LayoutObject 
{
    public override Container2D<GridType> Grids { get; protected set; }

    public GridType this[int x, int y]
    {
        get
        {
            return Grids[x - ShiftP.x, y - ShiftP.y];
        }
        set
        {
            Grids[x - ShiftP.x, y - ShiftP.y] = value;
        }
    }

    public GridType this[Point p]
    {
        get { return this[p.x, p.y]; }
        set { this[p.x, p.y] = value; }
    }

    public LayoutObjectLeaf() 
        : base()
    {
        Grids = new MultiMap<GridType>();
    }

    public LayoutObjectLeaf(Container2D<GridType> arr)
    {
        Grids = arr;
    }

    public override string GetTypeString()
    {
        return "Layout Object Leaf";
    }
}
