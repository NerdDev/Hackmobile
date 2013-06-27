using System.Collections;
using System;
using System.Collections.Generic;

public class Room : LayoutObjectLeaf {

    public int roomNum { get; private set; }
	
    public Room(int num)
        : base(LevelGenerator.maxRectSize * 2, LevelGenerator.maxRectSize * 2)
    {
        roomNum = num;
    }
	
	public GridMap getWalls() 
    {
		return getTypes(GridType.Wall);
	}
	
	public GridMap getFloors() 
    {
		return getTypes(GridType.Floor);
	}
	
	public GridMap getDoors() 
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
        GridMap potentialDoors = getWalls();
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
                    ret.Put(grids.Get(val.x, val.y), val.x, val.y);
                }
            }
        }
        return ret;
    }

	public override string ToString()
	{
		return "Room " + roomNum;
	}

    protected override Bounding GetBoundingInternal()
    {
		return grids.GetBounding();
	}
}
