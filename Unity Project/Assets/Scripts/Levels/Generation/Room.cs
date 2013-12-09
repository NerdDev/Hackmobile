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

    public int CountGridType(GridType type)
    {
        return getTypes(type).Count;
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
