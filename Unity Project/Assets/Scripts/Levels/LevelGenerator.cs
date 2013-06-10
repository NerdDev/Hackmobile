using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelGenerator {

    static int minRooms = 10;
    static int maxRooms = 25;
    LevelLayered lev = new LevelLayered();
    System.Random rand = new System.Random();


    public Level generate(int levelDepth)
    {
        DebugManager.printHeader(DebugManager.Logs.LevelGen, "Generating level: " + levelDepth);

        List<Room> rooms = generateRooms();
        LevelFlat flat = lev.getFlatLevel();

        DebugManager.printFooter(DebugManager.Logs.LevelGen);
        return flat;
    }

    List<Room> generateRooms()
    {
        DebugManager.printHeader(DebugManager.Logs.LevelGen, "Generate Rooms");

        List<Room> ret = new List<Room>();
        int numRooms = rand.Next(minRooms, maxRooms);
        DebugManager.w(DebugManager.Logs.LevelGen, "Generating " + numRooms + " rooms.");

        DebugManager.printFooter(DebugManager.Logs.LevelGen);
        return ret;
    }

    void placeRooms()
    {
        DebugManager.printHeader(DebugManager.Logs.LevelGen, "Place Rooms");
        


        DebugManager.printFooter(DebugManager.Logs.LevelGen);
    }
}
