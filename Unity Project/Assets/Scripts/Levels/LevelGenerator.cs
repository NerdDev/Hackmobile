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
    public static int doorsMax { get { return 5; } } //Max not inclusive
    public static int minDoorSpacing { get { return 2; } }
    #endregion

    public static System.Random rand = new System.Random();
    public static Random unityRand = new Random();

    public LevelLayout generateLayout(int levelDepth)
    {
        return generateLayout(levelDepth, rand.Next(), rand.Next());
    }

    public LevelLayout generateLayout(int levelDepth, int seed, int unitySeed)
    {
        #region DEBUG
        int debugNum = 1;
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.CreateNewLog(DebugManager.Logs.LevelGen, "Level Depth " + levelDepth + "/" + levelDepth + " " + debugNum++ + " - Generate Rooms");
            DebugManager.printHeader(DebugManager.Logs.LevelGen, "Generating level: " + levelDepth);
            DebugManager.w(DebugManager.Logs.LevelGen, "Random's seed int: " + seed);
            DebugManager.w(DebugManager.Logs.LevelGen, "Unity Random's seed int: " + unitySeed);
        }
        #endregion
        rand = new System.Random(seed);
        Random.seed = unitySeed;
        LevelLayout layout = new LevelLayout();
        List<Room> rooms = generateRooms();
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.CreateNewLog(DebugManager.Logs.LevelGen, "Level Depth " + levelDepth + "/" + levelDepth + " " + debugNum++ + " - Place Rooms");
        }
        #endregion
        placeRooms(rooms, layout);
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.CreateNewLog(DebugManager.Logs.LevelGen, "Level Depth " + levelDepth + "/" + levelDepth + " " + debugNum++ + " - Place Doors");
        }
        #endregion
        placeDoors(layout);
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.CreateNewLog(DebugManager.Logs.LevelGen, "Level Depth " + levelDepth + "/" + levelDepth + " " + debugNum++ + " - Place Paths");
        }
        #endregion
        placePaths(layout);
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printFooter(DebugManager.Logs.LevelGen);
        }
        #endregion
        return layout;
    }

    List<Room> generateRooms()
    {
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printHeader(DebugManager.Logs.LevelGen, "Generate Rooms");
        }
        #endregion
        List<Room> rooms = new List<Room>();
        int numRooms = rand.Next(minRooms, maxRooms);
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.w(DebugManager.Logs.LevelGen, "Generating " + numRooms + " rooms.");
        }
        #endregion
        for (int i = 1; i <= numRooms; i++)
        {
            Room room = new Room(i);
            room.generate();
			rooms.Add(room);
        }
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printFooter(DebugManager.Logs.LevelGen);
        }
        #endregion
        return rooms;
    }

    void placeRooms(List<Room> rooms, LevelLayout layout)
    {
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printHeader(DebugManager.Logs.LevelGen, "Place Rooms");
        }
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
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printHeader(DebugManager.Logs.LevelGen, "Place Doors");
        }
        #endregion
        foreach (Room room in layout.getRooms())
        {
            #region DEBUG
            if (DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                DebugManager.printHeader(DebugManager.Logs.LevelGen, "Place Doors on " + room);
            }
            #endregion
            GridMap potentialDoors = room.getWalls();
            GridMap corneredAreas = room.getCorneredBy(GridType.Wall, GridType.Wall);
			#region DEBUG
			if (DebugManager.logging(DebugManager.Logs.LevelGen))
			{
                potentialDoors.toLog(DebugManager.Logs.LevelGen, "Original Room");
                corneredAreas.toLog(DebugManager.Logs.LevelGen, "Cornered Areas");
			}
			#endregion
            potentialDoors.RemoveAll(corneredAreas);
            int numDoors = rand.Next(doorsMin, doorsMax);
			#region DEBUG
			if (DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                potentialDoors.toLog(DebugManager.Logs.LevelGen, "After Removing Invalid Locations");
                DebugManager.w(DebugManager.Logs.LevelGen, "Number of doors to generate: " + numDoors);
			}
            #endregion
            for (int i = 0; i < numDoors; i++)
            {
                Value2D<GridType> doorSpace = potentialDoors.RandomValue(rand);
                if (doorSpace != null)
                {
                    room.put(GridType.Door, doorSpace.x, doorSpace.y);
                    potentialDoors.Remove(doorSpace.x, doorSpace.y, minDoorSpacing - 1);
                    #region DEBUG
                    if (DebugManager.logging(DebugManager.Logs.LevelGen))
                    {
                        room.toLog(DebugManager.Logs.LevelGen, "Generated door at: " + doorSpace);
                        potentialDoors.toLog(DebugManager.Logs.LevelGen, "Remaining options");
                    }
                    #endregion
                }
                #region DEBUG
                else if (DebugManager.logging(DebugManager.Logs.LevelGen))
                {
                    DebugManager.w(DebugManager.Logs.LevelGen, "No door options remain.");
                }
                #endregion
            }
            #region DEBUG
            if (DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                room.toLog(DebugManager.Logs.LevelGen, "Final Room After placing doors");
                DebugManager.printFooter(DebugManager.Logs.LevelGen);
            }
            #endregion
        }
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            layout.toLog(DebugManager.Logs.LevelGen);
            DebugManager.printFooter(DebugManager.Logs.LevelGen);
        }
		#endregion
    }

    void placePaths(LevelLayout layout)
    {
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printHeader(DebugManager.Logs.LevelGen, "Place Paths");
        }
        #endregion
        GridArray grids = layout.GetArray();
        GridMap doors = layout.getTypes(grids, GridType.Door);
        foreach (Value2D<GridType> door in doors)
        {

        }
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printFooter(DebugManager.Logs.LevelGen);
        }
        #endregion
    }

    static Stack<Value2D<GridType>> depthFirstSearchFor(Value2D<GridType> startPoint, GridArray grids, params GridType[] targets)
    {
        Stack<Value2D<GridType>> nodesToVisit = new Stack<Value2D<GridType>>();
        Stack<Value2D<GridType>> pathTaken = new Stack<Value2D<GridType>>();
        Array2Dcoord<bool> blockedPoints = new Array2Dcoord<bool>(grids.GetBounding());
        pathTaken.Push(startPoint);
        GridType val;
        while (pathTaken.Count > 0)
        {
            // Don't want to visit the same point on a different route later
            blockedPoints.Put(true, startPoint.x, startPoint.y);
            if (grids.TryGet(
        }

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
