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
	
	public GridMap GetSmallLoots() 
    {
		return getTypes(GridType.SmallLoot);
	}

    public GridMap GetSecrets()
    {
        return getTypes(GridType.Secret);
    }

    public GridMap GetChests()
    {
        return getTypes(GridType.Chest);
    }

    public GridMap GetTraps()
    {
        return getTypes(GridType.Trap);
    }

    public GridMap GetTrapDoors()
    {
        return getTypes(GridType.TrapDoor);
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

    public int CountGridType(GridType type)
    {
        int ctr = 0;
        GridMap grid;

        switch (type)
        {
            case GridType.Floor: grid = GetFloors(); break;
            case GridType.Wall: grid = GetWalls(); break;
            case GridType.Chest: grid = GetChests(); break;
            case GridType.SmallLoot: grid = GetSmallLoots(); break;
            case GridType.Secret: grid = GetSecrets(); break;
            case GridType.Trap: grid = GetTraps(); break;
            case GridType.TrapDoor: grid = GetTrapDoors(); break;
            case GridType.Door: grid = GetDoors(); break;
            default: grid = null; break;
        }

        if (grid != null) ctr = grid.Count();

        return ctr;
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
