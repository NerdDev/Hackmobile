using System.Collections;
using System;
using System.Collections.Generic;

public class Room : LayoutObjectLeaf {

    public GridType[,] Array { get { return grids.GetArr(); } }

    public Room()
        : base(LevelGenerator.maxRectSize * 2, LevelGenerator.maxRectSize * 2)
    {
    }
    
	public GridMap GetWalls() 
    {
		return getType(GridType.Wall);
	}
	
	public GridMap GetFloors() 
    {
		return getType(GridType.Floor);
	}
	
	public GridMap GetSmallLoots() 
    {
		return getType(GridType.SmallLoot);
	}

    public GridMap GetSecrets()
    {
        return getType(GridType.Secret);
    }

    public GridMap GetChests()
    {
        return getType(GridType.Chest);
    }

    public GridMap GetTraps()
    {
        return getType(GridType.Trap);
    }

    public GridMap GetTrapDoors()
    {
        return getType(GridType.TrapDoor);
    }

    public GridMap GetDoors()
    {
        return getType(GridType.Door);
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
        return getTypes(type).Count();
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

    public override string GetTypeString()
    {
        return "Room";
    }

    protected override Bounding GetBoundingUnshifted()
    {
		return grids.GetBounding();
	}
}
