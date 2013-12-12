using System.Collections;
using System;
using System.Collections.Generic;

public class Room : LayoutObjectLeaf {

    public GridType[,] Array { get { return grids.GetArr(); } }

    public Room()
        : base(LevelGenerator.maxRectSize * 2, LevelGenerator.maxRectSize * 2)
    {
    }

    public override string GetTypeString()
    {
        return "Room";
    }

    protected override Bounding GetBoundingUnshifted()
    {
		return grids.GetBounding();
	}
}
