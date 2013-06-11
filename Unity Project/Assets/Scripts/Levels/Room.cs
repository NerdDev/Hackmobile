using UnityEngine;
using System.Collections;
using System;

public class Room : MapObject {

    static int maxHeight = 40;
    static int maxWidth = 40;
    int relativeX = 0;
    int relativeY = 0;

    public override GridInstance get(int x, int y)
    {
        throw new NotImplementedException();
    }

    public override MultiMap<GridInstance> getFlat()
    {
        throw new NotImplementedException();
    }

    public void generate()
    {
        DebugManager.printHeader(DebugManager.Logs.LevelGen, "Room -> Generate");

        int height = LevelGenerator.rand.Next(maxHeight);
        int width = LevelGenerator.rand.Next(maxWidth);
        DebugManager.w(DebugManager.Logs.LevelGen, "Height: " + height + ", Width: " + width);
        generateBox(width, height);

        DebugManager.printFooter(DebugManager.Logs.LevelGen);
    }

    void generateBox(int width, int height)
    {
        throw new NotImplementedException();
    }
}
