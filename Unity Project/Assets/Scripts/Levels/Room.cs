using System.Collections;
using System;
using System.Collections.Generic;

public class Room : LayoutObjectLeaf {

    static int max = 20;
    static int min = 5;
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

        int height = LevelGenerator.rand.Next(min, max);
        int width = LevelGenerator.rand.Next(min, max);
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
