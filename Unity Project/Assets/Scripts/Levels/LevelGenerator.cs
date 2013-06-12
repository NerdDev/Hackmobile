using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelGenerator {

    //// ===== Static generation variables ===== ////
    // Number of Rooms
    static int minRooms = 8;
    static int maxRooms = 16;  //Max not inclusive

    // Amount to shift rooms
    static int shiftRange = 10;   //Max not inclusive
    //////////////////////////////////////////////////

    LevelLayout lev = new LevelLayout();
    public static System.Random rand = new System.Random();
    public static Random unityRand = new Random();

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
        #region DEBUG
        DebugManager.printHeader(DebugManager.Logs.LevelGen, "Generating level: " + levelDepth);
        #endregion
        LevelLayout layout = new LevelLayout();
        List<Room> rooms = generateRooms();
        placeRooms(rooms, layout);
        #region DEBUG
        DebugManager.printFooter(DebugManager.Logs.LevelGen);
        #endregion
        return layout;
    }

    List<Room> generateRooms()
    {
        #region DEBUG
        DebugManager.printHeader(DebugManager.Logs.LevelGen, "Generate Rooms");
        #endregion
        List<Room> rooms = new List<Room>();
        int numRooms = rand.Next(minRooms, maxRooms);
        #region DEBUG
        DebugManager.w(DebugManager.Logs.LevelGen, "Generating " + numRooms + " rooms.");
        #endregion
        for (int i = 1; i <= numRooms; i++)
        {
            Room room = new Room(i);
            room.generate();
			rooms.Add(room);
        }
        #region DEBUG
        DebugManager.printFooter(DebugManager.Logs.LevelGen);
        #endregion
        return rooms;
    }

    void placeRooms(List<Room> rooms, LevelLayout layout)
    {
        #region DEBUG
        DebugManager.printHeader(DebugManager.Logs.LevelGen, "Place Rooms");
        #endregion
        List<LayoutObject> placedRooms = new List<LayoutObject>();
        placedRooms.Add(new Room(0)); // Seed empty center room to start positioning from.
        foreach (Room room in rooms)
        {
			layout.addRoom(room);
            // Find room it will start from
            int roomNum = rand.Next(placedRooms.Count);
            LayoutObject startRoom = placedRooms[roomNum];
            // Find where it will shift away
            Point shiftMagn = generateShiftMagnitude(shiftRange);
            #region DEBUG
            if (DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                DebugManager.w(DebugManager.Logs.LevelGen, "Placing room: " + room.roomNum);
                DebugManager.w(DebugManager.Logs.LevelGen, "Picked starting room number: " + roomNum);
                DebugManager.w(DebugManager.Logs.LevelGen, "Shift: " + shiftMagn);
            }
            #endregion
            // Move to initial start room
        	room.setShift(startRoom);
            // Keep moving until room doesn't overlap any other rooms
			LayoutObject intersect;
            while ((intersect = room.intersectObj(placedRooms)) != null)
            {
                #region DEBUG
                if (DebugManager.logging(DebugManager.Logs.LevelGen))
                {
                    DebugManager.w(DebugManager.Logs.LevelGen, "This layout led to an overlap: " + room.getBounds());
                    layout.toLog(DebugManager.Logs.LevelGen);
                }
                #endregion
                room.ShiftOutside(intersect, shiftMagn);
            }
            placedRooms.Add(room);
            #region DEBUG
            if (DebugManager.logging(DebugManager.Logs.LevelGen))
			{
                DebugManager.w(DebugManager.Logs.LevelGen, "Layout after placing room at: " + room.getBounds());
                layout.toLog(DebugManager.Logs.LevelGen);
                DebugManager.printBreakers(DebugManager.Logs.LevelGen, 4);
            }
            #endregion
        }
        #region DEBUG
        DebugManager.printFooter(DebugManager.Logs.LevelGen);
        #endregion
    }

    Point generateShiftMagnitude(int mag)
    {
        Vector2 vect = Random.insideUnitCircle * mag;
        Point p = new Point(vect);
		while (p.isZero())
		{
			vect = Random.insideUnitCircle * mag;
			p = new Point(vect);
		}
        return p;
    }
}
