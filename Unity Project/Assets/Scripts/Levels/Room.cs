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
        BoxFill(GridType.Floor, width - 1, height - 1);
        BoxStroke(GridType.Wall, width, height);
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            toLog(DebugManager.Logs.LevelGen);
            DebugManager.printFooter(DebugManager.Logs.LevelGen);
        }
        #endregion
    }
	
	public GridMap getWalls() {
		return getTypes(GridType.Wall);
	}
	
	public GridMap getFloors() {
		return getTypes(GridType.Floor);
	}
	
	public GridMap getDoors() {
		return getTypes(GridType.Door);
	}
	
	public override string ToString()
	{
		return "Room " + roomNum;
	}

}
