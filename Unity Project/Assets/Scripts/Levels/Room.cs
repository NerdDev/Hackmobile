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

    public void generate()
    {
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printHeader(DebugManager.Logs.LevelGen, "Room -> Generate");
        }
        #endregion
        int height = LevelGenerator.rand.Next(LevelGenerator.minRectSize, LevelGenerator.maxRectSize);
        int width = LevelGenerator.rand.Next(LevelGenerator.minRectSize, LevelGenerator.maxRectSize);
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.w(DebugManager.Logs.LevelGen, "Height: " + height + ", Width: " + width);
        }
        #endregion
        BoxStrokeAndFill(GridType.Wall, GridType.Floor, width, height);
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            toLog(DebugManager.Logs.LevelGen);
            DebugManager.printFooter(DebugManager.Logs.LevelGen);
        }
        #endregion
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

    public GridMap GetPotentialDoors()
    {
        GridMap potentialDoors = getWalls();
        GridMap corneredAreas = getCorneredBy(GridType.Wall, GridType.Wall);
        potentialDoors.RemoveAll(corneredAreas);
        return potentialDoors;
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
