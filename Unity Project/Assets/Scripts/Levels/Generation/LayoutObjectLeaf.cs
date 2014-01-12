using System.Collections;
using System.Collections.Generic;
using System;

public class LayoutObjectLeaf : LayoutObject 
{
    public override Container2D<GridType> Grids { get; protected set; }

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
