using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelGenerator {

    static int minRooms = 10;
    static int maxRooms = 25;
    LevelLayout lev = new LevelLayout();
    public static System.Random rand = new System.Random();

    public static char getAscii(GridType type) {
        switch (type)
        {
            case GridType.Floor:
                return '.';
            case GridType.TrapDoor:
                return 'T';
            case GridType.Door:
                return '|';
            case GridType.Wall:
                return '#';
            case GridType.NULL:
                return ' ';
            default:
                return '?';
        }
    }

    public LevelLayout generateLayout(int levelDepth)
    {
        DebugManager.printHeader(DebugManager.Logs.LevelGen, "Generating level: " + levelDepth);

        LevelLayout layout = new LevelLayout();
        List<Room> rooms = generateRooms();

        DebugManager.printFooter(DebugManager.Logs.LevelGen);
        return layout;
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

        DebugManager.printFooter(DebugManager.Logs.LevelGen);
        return ret;
    }

    void placeRooms()
    {
        DebugManager.printHeader(DebugManager.Logs.LevelGen, "Place Rooms");
        


        DebugManager.printFooter(DebugManager.Logs.LevelGen);
    }
}
