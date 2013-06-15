using System.Collections;
using System;
using System.Collections.Generic;

public class Room : LayoutObjectContainer {

    public LayoutObjectLeaf floors { get; set; }
    public LayoutObjectLeaf walls { get; set; }
    public LayoutObjectLeaf doors { get; set; }
    public int roomNum { get; private set; }
	
    public Room(int num)
    {
        int size = LevelGenerator.maxRectSize;
        roomNum = num;
        floors = new LayoutObjectLeaf(size, size);
        walls = new LayoutObjectLeaf(size, size);
        doors = new LayoutObjectLeaf(size, size);
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
        walls.BoxStroke(GridType.Wall, width, height);
        floors.BoxFill(GridType.Floor, width - 1, height - 1);
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            toLog(DebugManager.Logs.LevelGen);
            DebugManager.printFooter(DebugManager.Logs.LevelGen);
        }
        #endregion
    }
	
	public MultiMap<GridType> getWalls() {
		return walls.getTypes(GridType.Wall);
	}
	
	public MultiMap<GridType> getFloors() {
		return floors.getTypes(GridType.Floor);
	}
	
	public MultiMap<GridType> getDoors() {
		return doors.getTypes(GridType.Door);
	}
	
	public override string ToString()
	{
		return "Room " + roomNum;
	}


    protected override Bounding getBoundsInternal()
    {
        return walls.getBounds();
    }

    public override IEnumerator<LayoutObject> GetEnumerator()
    {
        yield return floors;
        yield return walls;
        yield return doors;
    }

}
