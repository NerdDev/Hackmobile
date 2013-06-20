using System.Collections;
using System;
using System.Collections.Generic;

public class Room : LayoutObjectContainer {

    // Testings

    public LayoutObjectLeaf floors { get; set; }
    public LayoutObjectLeaf walls { get; set; }
    public LayoutObjectLeaf doors { get; set; }
    public int roomNum { get; private set; }

    public Room(int num)
    {
        roomNum = num;
        floors = new LayoutObjectLeaf();
        walls = new LayoutObjectLeaf();
        doors = new LayoutObjectLeaf();
    }

    #region UNUSED
    //protected override List<string> ToRowStrings() 
    //{
    //    List<string> ret = base.ToRowStrings();
    //    int vert = ret.Count / 2;
    //    if (vert > 1)
    //    {
    //        string str = ret[vert];
    //        string roomNumStr = roomNum.ToString();
    //        int horiz = str.Length / 2;
    //        if (horiz > roomNumStr.Length)
    //        {
    //            ret[vert] = str.Substring(0, horiz) + roomNumStr + str.Substring(horiz + roomNumStr.Length);
    //        }
    //    }
    //    return ret;
    //}
    #endregion

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
