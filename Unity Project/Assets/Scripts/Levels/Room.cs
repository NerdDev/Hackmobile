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
        GridMap corneredAreas = getCorneredBy(GridType.Wall, GridType.Wall);
        potentialDoors.RemoveAll(corneredAreas);
    }

    public GridMap GetPerimeter()
    {
        GridMap ret = new GridMap();
        // Get null spaces surrounding room
        Array2D<bool> bfs = LevelGenerator.BreadthFirstFill(new Value2D<GridType>(), grids, GridType.NULL);
        // Invert to be room
        Array2D<bool>.invert(bfs);
        foreach (Value2D<bool> val in bfs)
        {
            // If space part of room
            if (val.val)
            {
                Surrounding<bool> surround = Surrounding<bool>.Get(bfs.GetArr(), val);
                // If space is an edge (next to a false)
                if (surround.GetDirWithVal(false) != null)
                {
                    ret[val.x, val.y] = grids[val.x, val.y];
                }
            }
        }
        return ret;
    }

    public override String GetTypeString()
    {
        return "Room";
    }

    protected override Bounding GetBoundingInternal()
    {
		return grids.GetBounding();
	}
}
