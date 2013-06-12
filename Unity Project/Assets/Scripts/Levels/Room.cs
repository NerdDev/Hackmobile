using System.Collections;
using System;
using System.Collections.Generic;

public class Room : LayoutObjectLeaf {

    public int roomNum {
    	get;
    	private set;
	}

    public Room(int num)
    {
        roomNum = num;
    }

    protected override List<string> ToRowStrings() 
    {
        List<string> ret = base.ToRowStrings();
        int vert = ret.Count / 2;
        if (vert > 1)
        {
            string str = ret[vert];
			string roomNumStr = roomNum.ToString();
            int horiz = str.Length / 2;
            if (horiz > roomNumStr.Length)
            {
                ret[vert] = str.Substring(0, horiz) + roomNumStr + str.Substring(horiz + roomNumStr.Length);
            }
        }
        return ret;
    }

    public void generate()
    {
        DebugManager.printHeader(DebugManager.Logs.LevelGen, "Room -> Generate");

        int height = LevelGenerator.rand.Next(LevelGenerator.minRectSize, LevelGenerator.maxRectSize);
        int width = LevelGenerator.rand.Next(LevelGenerator.minRectSize, LevelGenerator.maxRectSize);
        DebugManager.w(DebugManager.Logs.LevelGen, "Height: " + height + ", Width: " + width);
        BoxStrokeAndFill(GridType.Wall, GridType.Floor, width, height);
		
        toLog(DebugManager.Logs.LevelGen);
        DebugManager.printFooter(DebugManager.Logs.LevelGen);
    }
	
	public override string ToString()
	{
		return "Room " + roomNum;
	}

}
