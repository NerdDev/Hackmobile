using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelGenerator
{

    #region GlobalGenVariables
    // Number of Rooms
    public static int minRooms { get { return 8; } }
    public static int maxRooms { get { return 16; } } //Max not inclusive

    // Box Room Size (including walls)
    public static int minRectSize { get { return 5; } }
    public static int maxRectSize { get { return 20; } }

    // Amount to shift rooms
    public static int shiftRange { get { return 10; } } //Max not inclusive

    // Number of doors per room
    public static int doorsMin { get { return 1; } }
    public static int doorsMax { get { return 5; } } //Max not inclusive
    public static int minDoorSpacing { get { return 2; } }

    // Margin space on room layout
    public static int layoutMargin { get { return 8; } }

    // Room modifier probabilies
    public static int maxFlexMod { get { return 5; } } //Max not inclusive
    public static int chanceNoFinalMod { get { return 40; } }
    #endregion

    public static System.Random Rand = new System.Random();
    public static Random UnityRand = new Random();

    public LevelLayout GenerateLayout(int levelDepth)
    {
        return GenerateLayout(levelDepth, Rand.Next(), Rand.Next());
    }

    public LevelLayout GenerateLayout(int levelDepth, int seed, int unitySeed)
    {
        #region DEBUG
        int debugNum = 1;
        float stepTime = 0, startTime = 0;
        if (DebugManager.logging(DebugManager.Logs.LevelGenMain))
        {
            if (DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                DebugManager.printHeader(DebugManager.Logs.LevelGenMain, "Generating Level: " + levelDepth);
                DebugManager.w(DebugManager.Logs.LevelGenMain, "Random/Unity seed ints: " + seed + ", " + unitySeed);
                DebugManager.CreateNewLog(DebugManager.Logs.LevelGen, "Level Depth " + levelDepth + "/" + levelDepth + " " + debugNum++ + " - Generate Rooms");
                DebugManager.printHeader(DebugManager.Logs.LevelGen, "Generating level: " + levelDepth);
            }
            stepTime = Time.realtimeSinceStartup;
            startTime = stepTime;
        }
        #endregion
        Rand = new System.Random(seed);
        Random.seed = unitySeed;
        LevelLayout layout = new LevelLayout();
        List<Room> rooms = generateRooms();
        ModRooms(rooms);
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGenMain))
        {
            DebugManager.w(DebugManager.Logs.LevelGenMain, "Modding Rooms took: " + (Time.realtimeSinceStartup - stepTime));
            stepTime = Time.realtimeSinceStartup;
            if (DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                DebugManager.CreateNewLog(DebugManager.Logs.LevelGen, "Level Depth " + levelDepth + "/" + levelDepth + " " + debugNum++ + " - Place Rooms");
            }
        }
        #endregion
        PlaceRooms(rooms, layout);
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGenMain))
        {
            DebugManager.w(DebugManager.Logs.LevelGenMain, "Place Rooms took: " + (Time.realtimeSinceStartup - stepTime));
            stepTime = Time.realtimeSinceStartup;
            if (DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                DebugManager.CreateNewLog(DebugManager.Logs.LevelGen, "Level Depth " + levelDepth + "/" + levelDepth + " " + debugNum++ + " - Place Doors");
            }
        }
        #endregion
        PlaceDoors(layout);
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGenMain))
        {
            DebugManager.w(DebugManager.Logs.LevelGenMain, "Place Doors took: " + (Time.realtimeSinceStartup - stepTime));
            stepTime = Time.realtimeSinceStartup;
            if (DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                DebugManager.CreateNewLog(DebugManager.Logs.LevelGen, "Level Depth " + levelDepth + "/" + levelDepth + " " + debugNum++ + " - Place Paths");
            }
        }
        #endregion
        PlacePaths(layout);
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGenMain))
        {
            DebugManager.w(DebugManager.Logs.LevelGenMain, "Place paths took: " + (Time.realtimeSinceStartup - stepTime));
            stepTime = Time.realtimeSinceStartup;
            if (DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                DebugManager.CreateNewLog(DebugManager.Logs.LevelGen, "Level Depth " + levelDepth + "/" + levelDepth + " " + debugNum++ + " - Confirm Connections");
            }
        }
        #endregion
        ConfirmConnection(layout);
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGenMain))
        {
            DebugManager.w(DebugManager.Logs.LevelGenMain, "Confirm Connection took: " + (Time.realtimeSinceStartup - stepTime));
            stepTime = Time.realtimeSinceStartup;
        }
        #endregion

        #region DEBUG
        if (DebugManager.logging())
        {
            DebugManager.w(DebugManager.Logs.LevelGenMain, "Generate Level took: " + (Time.realtimeSinceStartup - startTime));
            layout.ToLog(DebugManager.Logs.LevelGenMain);
            DebugManager.printFooter(DebugManager.Logs.LevelGenMain);
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
        int numRooms = Rand.Next(minRooms, maxRooms);
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.w(DebugManager.Logs.LevelGen, "Generating " + numRooms + " rooms.");
        }
        #endregion
        for (int i = 1; i <= numRooms; i++)
        {
            Room room = new Room();
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

    void ModRooms(IEnumerable<Room> rooms)
    {
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printHeader(DebugManager.Logs.LevelGen, "Mod Rooms");
        }
        #endregion
        foreach (Room room in rooms)
        {
            #region DEBUG
            if (DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                DebugManager.printHeader(DebugManager.Logs.LevelGen, "Modding " + room);
            }
            #endregion
            foreach (RoomModifier mod in PickMods())
            {
                #region DEBUG
                if (DebugManager.logging(DebugManager.Logs.LevelGen))
                {
                    DebugManager.w(DebugManager.Logs.LevelGen, "Applying: " + mod);
                }
                #endregion
                mod.Modify(room, Rand);
            }
            #region DEBUG
            if (DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                room.ToLog(DebugManager.Logs.LevelGen);
                DebugManager.printFooter(DebugManager.Logs.LevelGen);
            }
            #endregion
        }
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printFooter(DebugManager.Logs.LevelGen);
        }
        #endregion
    }

    static List<RoomModifier> PickMods()
    {
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printHeader(DebugManager.Logs.LevelGen, "Pick Mods");
        }
        #endregion
        List<RoomModifier> mods = new List<RoomModifier>();
        RoomModifier baseMod = RoomModifier.GetBase();
        mods.Add(baseMod);
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.w(DebugManager.Logs.LevelGen, "Picked Base Mod: " + baseMod);
        }
        #endregion
        int numFlex = Rand.Next(1, maxFlexMod);
        List<RoomModifier> flexMods = RoomModifier.GetFlexible(numFlex);
        mods.AddRange(flexMods);
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.w(DebugManager.Logs.LevelGen, "Picked " + numFlex + " flex modifiers: ");
            foreach (RoomModifier mod in flexMods)
            {
                DebugManager.w(DebugManager.Logs.LevelGen, 1, mod.ToString());
            }
        }
        #endregion
        if (chanceNoFinalMod < Rand.Next(100))
        {
            RoomModifier finalMod = RoomModifier.GetFinal();
            mods.Add(finalMod);
            #region DEBUG
            if (DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                DebugManager.w(DebugManager.Logs.LevelGen, "Picked Base Mod: " + finalMod);
            }
            #endregion
        }
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printFooter(DebugManager.Logs.LevelGen);
        }
        #endregion
        return mods;
    }

    static void PlaceRooms(List<Room> rooms, LevelLayout layout)
    {
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printHeader(DebugManager.Logs.LevelGen, "Place Rooms");
        }
        #endregion
        List<LayoutObject> placedRooms = new List<LayoutObject>();
        Room seedRoom = new Room();
        layout.AddObject(seedRoom);
        placedRooms.Add(seedRoom); // Seed empty center room to start positioning from.
        foreach (Room room in rooms)
        {
            // Find room it will start from
            int roomNum = Rand.Next(placedRooms.Count);
            LayoutObject startRoom = placedRooms[roomNum];
            room.setShift(startRoom);
            // Find where it will shift away
            Point shiftMagn = GenerateShiftMagnitude(shiftRange);
            #region DEBUG
            if (DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                DebugManager.w(DebugManager.Logs.LevelGen, "Placing room: " + room);
                DebugManager.w(DebugManager.Logs.LevelGen, "Picked starting room number: " + roomNum);
                DebugManager.w(DebugManager.Logs.LevelGen, "Shift: " + shiftMagn);
            }
            #endregion
            // Keep moving until room doesn't overlap any other rooms
            LayoutObject intersect;
            while ((intersect = room.intersectObj(placedRooms, 1)) != null)
            {
                #region DEBUG
                if (DebugManager.logging(DebugManager.Logs.LevelGen))
                {
                    DebugManager.w(DebugManager.Logs.LevelGen, "This layout led to an overlap: " + room.GetBounding());
                    layout.ToLog(DebugManager.Logs.LevelGen);
                }
                #endregion
                room.ShiftOutside(intersect, shiftMagn);
            }
            placedRooms.Add(room);
            layout.AddRoom(room);
            layout.RemoveObject(seedRoom);
            #region DEBUG
            if (DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                DebugManager.w(DebugManager.Logs.LevelGen, "Layout after placing room at: " + room.GetBounding());
                layout.ToLog(DebugManager.Logs.LevelGen);
                DebugManager.printBreakers(DebugManager.Logs.LevelGen, 4);
            }
            #endregion
        }
        #region DEBUG
        DebugManager.printFooter(DebugManager.Logs.LevelGen);
        #endregion
    }

    static void PlaceDoors(LevelLayout layout)
    {
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printHeader(DebugManager.Logs.LevelGen, "Place Doors");
        }
        #endregion
        foreach (Room room in layout.GetRooms())
        {
            #region DEBUG
            if (DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                DebugManager.printHeader(DebugManager.Logs.LevelGen, "Place Doors on " + room);
            }
            #endregion
            GridMap potentialDoors = room.GetPotentialExternalDoors();
            int numDoors = Rand.Next(doorsMin, doorsMax);
            #region DEBUG
            if (DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                potentialDoors.ToLog(DebugManager.Logs.LevelGen, "Potential Doors");
                DebugManager.w(DebugManager.Logs.LevelGen, "Number of doors to generate: " + numDoors);
            }
            #endregion
            for (int i = 0; i < numDoors; i++)
            {
                Value2D<GridType> doorSpace = potentialDoors.RandomValue(Rand);
                if (doorSpace != null)
                {
                    room.put(GridType.Door, doorSpace.x, doorSpace.y);
                    potentialDoors.Remove(doorSpace.x, doorSpace.y, minDoorSpacing - 1);
                    #region DEBUG
                    if (DebugManager.logging(DebugManager.Logs.LevelGen))
                    {
                        room.ToLog(DebugManager.Logs.LevelGen, "Generated door at: " + doorSpace);
                        potentialDoors.ToLog(DebugManager.Logs.LevelGen, "Remaining options");
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
                room.ToLog(DebugManager.Logs.LevelGen, "Final Room After placing doors");
                DebugManager.printFooter(DebugManager.Logs.LevelGen);
            }
            #endregion
        }
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            layout.ToLog(DebugManager.Logs.LevelGen);
            DebugManager.printFooter(DebugManager.Logs.LevelGen);
        }
        #endregion
    }

    static void PlacePaths(LevelLayout layout)
    {
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printHeader(DebugManager.Logs.LevelGen, "Place Paths");
        }
        #endregion
        var bounds = layout.GetBounding();
        bounds.expand(layoutMargin);
        bounds.ShiftNonNeg();
        var grids = layout.GetArray(bounds);
        GridMap doors = layout.getTypes(grids, GridType.Door);
        foreach (var door in doors)
        {
            var path = new Path(door, grids);
            #region DEBUG

            if (DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                GridArray tmp = new GridArray(grids);
                tmp.PutAll(path.GetArray());
                tmp.ToLog(DebugManager.Logs.LevelGen, "Map after placing for door: " + door);
            }

            #endregion
            if (path.isValid())
            {
                path.Finalize(layout);
                grids.PutAll(path.GetArray());
                layout.AddPath(path);
            }
            #region DEBUG

            if (DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                grids.ToLog(DebugManager.Logs.LevelGen, "Map after simplifying path for door: " + door);
                layout.ToLog(DebugManager.Logs.LevelGen, "Map after simplifying path for door TEST: " + door);
            }

            #endregion
        }
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printFooter(DebugManager.Logs.LevelGen);
        }
        #endregion
    }

    private static void ConfirmConnection(LevelLayout layout)
    {
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printHeader(DebugManager.Logs.LevelGen, "Confirm Connections");
        }
        #endregion
        var roomsToConnect = new List<LayoutObject>(layout.GetRooms().Cast<LayoutObject>());
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.w(DebugManager.Logs.LevelGen, "Connecting List:");
            foreach (var layoutobj in layout.GetRooms())
            {
                DebugManager.w(DebugManager.Logs.LevelGen, 1, layoutobj.ToString());       
            }
        }
        #endregion
        foreach (var layoutObj in layout.GetRooms())
        {
            #region DEBUG
            if (DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                layoutObj.ToLog(DebugManager.Logs.LevelGen, "Connecting");
            }
            #endregion
            roomsToConnect.Remove(layoutObj);
            LayoutObject fail;
            if (!layoutObj.ConnectedTo(roomsToConnect, out fail))
            {
                #region DEBUG
                if (DebugManager.logging(DebugManager.Logs.LevelGen))
                {
                    fail.ToLog(DebugManager.Logs.LevelGen, layoutObj + " failed to connect to:");
                }
                #endregion
                MakeConnection(layout, layoutObj, fail);
            }
            #region DEBUG
            if (DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                DebugManager.printFooter(DebugManager.Logs.LevelGen);
            }
            #endregion
        }
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printFooter(DebugManager.Logs.LevelGen);
        }
        #endregion
    }

    private static void MakeConnection(LevelLayout layout, LayoutObject obj1, LayoutObject obj2)
    {
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printHeader(DebugManager.Logs.LevelGen, "Make Connection - " + obj1 + " AND " + obj2);
        }
        #endregion
        GridArray smallest;
        GridArray largest;
        GridArray layoutArr = layout.GetArray();
        layoutArr.PutAsBlocked(layoutArr);
        Container2D<GridType>.Smallest(obj1.GetConnectedGrid(), obj2.GetConnectedGrid(), out smallest, out largest);
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            smallest.ToLog(DebugManager.Logs.LevelGen, "Smallest");
            largest.ToLog(DebugManager.Logs.LevelGen, "Largest");
        }
        #endregion
        var startPtStack = DepthFirstSearchFor(new Value2D<GridType>(), smallest, Path.PathTypes());
        if (startPtStack.Count > 0)
        {
            layoutArr.PutAll(largest);
            Value2D<GridType> startPoint = startPtStack.Pop();
            #region DEBUG
            if (DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                layoutArr[startPoint.x, startPoint.y] = GridType.INTERNAL_RESERVED_CUR;
                layoutArr.ToLog(DebugManager.Logs.LevelGen, "Largest after putting blocked");
                DebugManager.w(DebugManager.Logs.LevelGen, "Start Point:" + startPoint);
            }
            #endregion
            var path = new Path(startPoint, layoutArr);
            if (path.isValid())
            {
                #region DEBUG
                if (DebugManager.logging(DebugManager.Logs.LevelGen))
                {
                    largest.PutAll(path);
                    largest.ToLog(DebugManager.Logs.LevelGen, "Connecting Path");
                }
                #endregion
                path.Finalize(layout);
                layout.AddPath(path);
                #region DEBUG
                if (DebugManager.logging(DebugManager.Logs.LevelGen))
                {
                    layout.ToLog(DebugManager.Logs.LevelGen, "Final Connection");
                }
                #endregion
            }
        }
        #region DEBUG
        else if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.w(DebugManager.Logs.LevelGen, "Could not make an initial start point connection.");
        }
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printFooter(DebugManager.Logs.LevelGen);
        }
        #endregion
    }

    public static Stack<Value2D<GridType>> DepthFirstSearchFor(Value2D<GridType> startPoint, GridArray grids, params GridType[] targets)
    {
        return DepthFirstSearchFor(startPoint, grids, new HashSet<GridType>(targets));
    }

    public static Stack<Value2D<GridType>> DepthFirstSearchFor(Value2D<GridType> startPoint, GridArray grids, HashSet<GridType> targets)
    {
        #region DEBUG
        if (DebugManager.Flag(DebugManager.DebugFlag.SearchSteps) && DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printHeader(DebugManager.Logs.LevelGen, "Depth First Search");
            GridArray tmp = new GridArray(grids);
            tmp[startPoint.x, startPoint.y] = GridType.INTERNAL_RESERVED_CUR;
            tmp.ToLog(DebugManager.Logs.LevelGen, "Starting Map:");
        }
        #endregion
        // Init
        GridType[,] arr = grids.GetArr();
        Stack<Value2D<GridType>> pathTaken = new Stack<Value2D<GridType>>();
        Array2D<bool> blockedPoints = new Array2D<bool>(grids.GetBoundingInternal(), false);
        FilterDFS filter = new FilterDFS(blockedPoints, targets);
        #region DEBUG
        GridArray debugGrid = new GridArray(0, 0); // Will be reassigned later
        #endregion

        // Push start point onto path
        pathTaken.Push(startPoint);
        while (pathTaken.Count > 0)
        {
            startPoint = pathTaken.Peek();
            // Don't want to visit the same point on a different route later
            blockedPoints[startPoint.x, startPoint.y] = true;
            #region DEBUG
            if (DebugManager.Flag(DebugManager.DebugFlag.SearchSteps) && DebugManager.logging(DebugManager.Logs.LevelGen))
            { // Set up new print array
                debugGrid = new GridArray(grids);
                // Fill in blocked points
                foreach (Value2D<bool> blockedPt in blockedPoints)
                {
                    if (blockedPt.val)
                    {
                        debugGrid[blockedPt.x, blockedPt.y] = GridType.INTERNAL_RESERVED_BLOCKED;
                    }
                }
                Path tmpPath = new Path(pathTaken);
                debugGrid.PutAll(tmpPath.GetArray());
            }
            #endregion

            // Get surrounding points
            Surrounding<GridType> options = Surrounding<GridType>.Get(arr, startPoint.x, startPoint.y, filter);
            #region DEBUG
            if (DebugManager.Flag(DebugManager.DebugFlag.SearchSteps) && DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                debugGrid.ToLog(DebugManager.Logs.LevelGen, "Current Map with " + options.Count() + " options.");
            }
            #endregion

            // If found target, return path we took
            Value2D<GridType> targetDir = options.GetDirWithVal(targets);
            if (targetDir != null)
            {
                #region DEBUG
                if (DebugManager.Flag(DebugManager.DebugFlag.SearchSteps) && DebugManager.logging(DebugManager.Logs.LevelGen))
                {
                    DebugManager.w(DebugManager.Logs.LevelGen, "===== FOUND TARGET: " + startPoint);
                    DebugManager.printFooter(DebugManager.Logs.LevelGen);
                }
                #endregion
                pathTaken.Push(targetDir);
                return pathTaken;
            }

            // Didn't find target, pick random direction
            targetDir = options.GetRandom(Rand);
            if (targetDir == null)
            { // If all directions are bad, back up
                pathTaken.Pop();
            }
            else
            {
                #region DEBUG
                if (DebugManager.Flag(DebugManager.DebugFlag.SearchSteps) && DebugManager.logging(DebugManager.Logs.LevelGen))
                {
                    DebugManager.w(DebugManager.Logs.LevelGen, "Chose Direction: " + targetDir);
                }
                #endregion
                startPoint = targetDir;
                pathTaken.Push(startPoint);
            }
        }
        #region DEBUG
        if (DebugManager.Flag(DebugManager.DebugFlag.SearchSteps) && DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printFooter(DebugManager.Logs.LevelGen);
        }
        #endregion
        return pathTaken;
    }

    public static Array2D<bool> BreadthFirstFill(Value2D<GridType> startPoint, GridArray grids, params GridType[] targets)
    {
        return BreadthFirstFill(startPoint, grids, null, targets);
    }

    public static Array2D<bool> BreadthFirstFill(Value2D<GridType> startPoint, GridArray grids, PassFilter<Value2D<GridType>> pass, params GridType[] targets)
    {
        return BreadthFirstFill(startPoint, grids, pass, new HashSet<GridType>(targets));
    }

    public static Array2D<bool> BreadthFirstFill(Value2D<GridType> startPoint, GridArray grids, HashSet<GridType> targets)
    {
        return BreadthFirstFill(startPoint, grids, null, targets);
    }

    public static Array2D<bool> BreadthFirstFill(Value2D<GridType> startPoint, GridArray grids, PassFilter<Value2D<GridType>> pass, HashSet<GridType> targets)
    {
        #region DEBUG
        if (DebugManager.Flag(DebugManager.DebugFlag.SearchSteps) && DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printHeader(DebugManager.Logs.LevelGen, "Breadth First Search Fill");
            GridArray tmp = new GridArray(grids);
            tmp[startPoint.x, startPoint.y] = GridType.INTERNAL_RESERVED_CUR;
            tmp.ToLog(DebugManager.Logs.LevelGen, "Starting Map:");
        }
        #endregion
        Queue<Value2D<GridType>> queue = new Queue<Value2D<GridType>>();
        queue.Enqueue(startPoint);
        GridType[,] targetArr = grids.GetArr();
        Array2D<bool> outGridArr = new Array2D<bool>(grids.GetBoundingInternal(), false);
        outGridArr[startPoint.x, startPoint.y] = true;
        Value2D<GridType> curPoint;
        while (queue.Count > 0)
        {
            curPoint = queue.Dequeue();
            Surrounding<GridType> options = Surrounding<GridType>.Get(targetArr, curPoint.x, curPoint.y, pass);
            #region DEBUG
            if (DebugManager.Flag(DebugManager.DebugFlag.SearchSteps) && DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                outGridArr.ToLog(DebugManager.Logs.LevelGen, "Current Map with " + options.Count() + " options. Evaluating " + curPoint);
            }
            #endregion
            foreach (Value2D<GridType> option in options)
            {
                if (targets.Contains(option.val) && !outGridArr[option.x, option.y])
                {
                    queue.Enqueue(option);
                    outGridArr[option.x, option.y] = true;
                }
            }
        }
        #region DEBUG
        if (DebugManager.Flag(DebugManager.DebugFlag.SearchSteps) && DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printFooter(DebugManager.Logs.LevelGen);
        }
        #endregion
        return outGridArr;
    }

    static Point GenerateShiftMagnitude(int mag)
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
