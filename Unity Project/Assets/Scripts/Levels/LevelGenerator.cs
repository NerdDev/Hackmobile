using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelGenerator
{

    #region GlobalGenVariables
    // Number of Rooms
	public static int minRooms { get { return 8; }}
	public static int maxRooms { get { return 16; }} //Max not inclusive
	
	// Box Room Size (including walls)
	public static int minRectSize { get { return 5; }}
	public static int maxRectSize { get { return 20; }}

    // Amount to shift rooms
	public static int shiftRange { get { return 10; }} //Max not inclusive

    // Number of doors per room
    public static int doorsMin { get { return 1; } }
    public static int doorsMax { get { return 4; } } //Max not inclusive
    #endregion

    public static System.Random rand = new System.Random();
    public static Random unityRand = new Random();

    public LevelLayout generateLayout(int levelDepth)
    {
        #region DEBUG
        DebugManager.printHeader(DebugManager.Logs.LevelGen, "Generating level: " + levelDepth);
        #endregion
        LevelLayout layout = new LevelLayout();
        List<Room> rooms = generateRooms();
        placeRooms(rooms, layout);
        placeDoors(layout);
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
            room.setShift(startRoom);
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
            // Keep moving until room doesn't overlap any other rooms
			LayoutObject intersect;
            while ((intersect = room.intersectObj(placedRooms)) != null)
            {
                #region DEBUG
                if (DebugManager.logging(DebugManager.Logs.LevelGen))
                {
                    DebugManager.w(DebugManager.Logs.LevelGen, "This layout led to an overlap: " + room.GetBounding());
                    layout.toLog(DebugManager.Logs.LevelGen);
                }
                #endregion
                room.ShiftOutside(intersect, shiftMagn);
            }
            placedRooms.Add(room);
            #region DEBUG
            if (DebugManager.logging(DebugManager.Logs.LevelGen))
			{
                DebugManager.w(DebugManager.Logs.LevelGen, "Layout after placing room at: " + room.GetBounding());
                layout.toLog(DebugManager.Logs.LevelGen);
                DebugManager.printBreakers(DebugManager.Logs.LevelGen, 4);
            }
            #endregion
        }
        #region DEBUG
        DebugManager.printFooter(DebugManager.Logs.LevelGen);
        #endregion
    }

    void placeDoors(LevelLayout layout)
    {
        DebugManager.printHeader(DebugManager.Logs.LevelGen, "Place Doors");
        foreach (Room room in layout.getRooms())
        { 
            MultiMap<GridType> potentialDoors = room.getWalls();
			#region DEBUG
			if (DebugManager.logging(DebugManager.Logs.LevelGen))
			{
				potentialDoors.toLog(DebugManager.Logs.LevelGen);
			}
			#endregion
            potentialDoors.RemoveAll(room.getCorneredBy(GridType.Wall, GridType.Wall));
			#region DEBUG
			if (DebugManager.logging(DebugManager.Logs.LevelGen))
			{
				potentialDoors.toLog(DebugManager.Logs.LevelGen);
			}
			#endregion
			potentialDoors.toLog(DebugManager.Logs.LevelGen);
        }
        DebugManager.printFooter(DebugManager.Logs.LevelGen);
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
