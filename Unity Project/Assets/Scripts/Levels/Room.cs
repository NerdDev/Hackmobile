using UnityEngine;
using System.Collections;
using System;

public class Room : LayoutObject {

    static int max = 20;
    static int min = 5;
    int relativeX = 0;
    int relativeY = 0;

    public void generate()
    {
        DebugManager.printHeader(DebugManager.Logs.LevelGen, "Room -> Generate");

        int height = LevelGenerator.rand.Next(min, max);
        int width = LevelGenerator.rand.Next(min, max);
        DebugManager.w(DebugManager.Logs.LevelGen, "Height: " + height + ", Width: " + width);
        generateBox(GridType.Wall, width, height);
		
        toLog(DebugManager.Logs.LevelGen);
        DebugManager.printFooter(DebugManager.Logs.LevelGen);
    }
}
