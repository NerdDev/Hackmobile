using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelGenerator {

    static int minRooms = 10;
    static int maxRooms = 25;
    LevelLayered lev = new LevelLayered();
    public static System.Random rand = new System.Random();


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

        for (int i = 0; i < numRooms; i++)
        {
            Room room = new Room();
            room.generate();
            ret.Add(room);
        }

        logRooms(ret);
        DebugManager.printFooter(DebugManager.Logs.LevelGen);
        return ret;
    }

    void logRooms(List<Room> rooms)
    {
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            int i = 1;
            foreach (Room r in rooms)
            {
                DebugManager.w(DebugManager.Logs.LevelGen, i + ": ");
                r.toLog(DebugManager.Logs.LevelGen);
            }
        }
    }

    void placeRooms()
    {
        DebugManager.printHeader(DebugManager.Logs.LevelGen, "Place Rooms");
        


        DebugManager.printFooter(DebugManager.Logs.LevelGen);
    }
}
