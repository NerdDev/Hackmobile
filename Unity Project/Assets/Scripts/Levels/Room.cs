using System.Collections;
using System;
using System.Collections.Generic;

public class Room : LayoutObjectLeaf {

    public Room()
        : base(LevelGenerator.maxRectSize * 2, LevelGenerator.maxRectSize * 2)
    {
    }
	
	public GridMap GetWalls() 
    {
		return getTypes(GridType.Wall);
	}
	
	public GridMap GetFloors() 
    {
		return getTypes(GridType.Floor);
	}
	
	public GridMap GetDoors() 
    {
		return getTypes(GridType.Door);
	}

    public GridMap GetPotentialExternalDoors()
    {
        GridMap potentialDoors = GetPerimeter();
        RemoveBadDoorWalls(potentialDoors);
        potentialDoors.RemoveAllBut(GridType.Wall);
        return potentialDoors;
    }

    public GridMap GetPotentialDoors()
    {
        GridMap potentialDoors = GetWalls();
        RemoveBadDoorWalls(potentialDoors);
        return potentialDoors;
    }

    void RemoveBadDoorWalls(GridMap potentialDoors)
    {
        GridMap corneredAreas = GetCorneredBy(GridType.Wall, GridType.Wall);
        potentialDoors.RemoveAll(corneredAreas);
    }

    public GridMap GetPerimeter()
    {
        return GetBfsPerimeter();
    }

    public override String GetTypeString()
    {
        return "Room";
    }

    protected override Bounding GetBoundingUnshifted()
    {
		return grids.GetBounding();
	}
}
