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
        float stepTime = 0, startTime = 0;
        if (DebugManager.logging(DebugManager.Logs.LevelGenMain))
        {
            if (DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                DebugManager.printHeader(DebugManager.Logs.LevelGenMain, "Generating Level: " + levelDepth);
                DebugManager.CreateNewLog(DebugManager.Logs.LevelGen, "Level Depth " + levelDepth + "/" + levelDepth + " " + debugNum++ + " - Generate Rooms");
                DebugManager.printHeader(DebugManager.Logs.LevelGen, "Generating level: " + levelDepth);
                DebugManager.w(DebugManager.Logs.LevelGen, "Random's seed int: " + seed);
                DebugManager.w(DebugManager.Logs.LevelGen, "Unity Random's seed int: " + unitySeed);
            }
            stepTime = Time.realtimeSinceStartup;
            startTime = stepTime;
        }
        #endregion
        rand = new System.Random(seed);
        Random.seed = unitySeed;
        LevelLayout layout = new LevelLayout();
        List<Room> rooms = generateRooms();
        modRooms(rooms);
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
        placeRooms(rooms, layout);
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
        placeDoors(layout);
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
        placePaths(layout);
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGenMain))
        {
            DebugManager.w(DebugManager.Logs.LevelGenMain, "Place Paths took: " + (Time.realtimeSinceStartup - stepTime));
            stepTime = Time.realtimeSinceStartup;
            if (DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                DebugManager.printFooter(DebugManager.Logs.LevelGen);
            }
        }
        #endregion

        #region DEBUG
        if (DebugManager.logging())
        {
            DebugManager.w(DebugManager.Logs.LevelGenMain, "Generate Level took: " + (Time.realtimeSinceStartup - startTime));
            layout.toLog(DebugManager.Logs.LevelGenMain);
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

    void modRooms(List<Room> rooms)
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
            foreach (RoomModifier mod in pickMods())
            {
                #region DEBUG
                if (DebugManager.logging(DebugManager.Logs.LevelGen))
                {
                    DebugManager.w(DebugManager.Logs.LevelGen, "Applying: " + mod);
                }
                #endregion
                mod.Modify(room, rand);
            }
            #region DEBUG
            if (DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                room.toLog(DebugManager.Logs.LevelGen);
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

    List<RoomModifier> pickMods()
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
        int numFlex = rand.Next(1, maxFlexMod);
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
        if (chanceNoFinalMod < rand.Next(100))
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
            while ((intersect = room.intersectObj(placedRooms, 1)) != null)
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
            GridMap potentialDoors = room.GetPotentialExternalDoors();
            int numDoors = rand.Next(doorsMin, doorsMax);
            #region DEBUG
            if (DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                potentialDoors.toLog(DebugManager.Logs.LevelGen, "Potential Doors");
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
        Bounding bounds = layout.GetBounding();
        bounds.expand(layoutMargin);
        GridArray grids = layout.GetArray(bounds);
        GridMap doors = layout.getTypes(grids, GridType.Door);
        foreach (Value2D<GridType> door in doors)
        {

            Path path = new Path(depthFirstSearchFor(door, grids, GridType.Door,
                GridType.Path_Horiz,
                GridType.Path_Vert,
                GridType.Path_LB,
                GridType.Path_LT,
                GridType.Path_RB,
                GridType.Path_RT));
            #region DEBUG
            if (DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                GridArray tmp = new GridArray(grids);
                tmp.PutAll(path.GetArray());
                tmp.toLog(DebugManager.Logs.LevelGen, "Map after placing for door: " + door);
            }
            #endregion
            path.simplify();
            if (path.isValid())
            {
                grids.PutAll(path.GetArray());
                path.shift(bounds.xMin, bounds.yMin);
                layout.addPath(path);
            }
            #region DEBUG
            if (DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                grids.toLog(DebugManager.Logs.LevelGen, "Map after simplifying path for door: " + door);
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

    public static Stack<Value2D<GridType>> depthFirstSearchFor(Value2D<GridType> startPoint, GridArray grids, params GridType[] targets)
    {
        return depthFirstSearchFor(startPoint, grids, new HashSet<GridType>(targets));
    }

    public static Stack<Value2D<GridType>> depthFirstSearchFor(Value2D<GridType> startPoint, GridArray grids, HashSet<GridType> targets)
    {
        #region DEBUG
        if (DebugManager.Flag(DebugManager.DebugFlag.SearchSteps) && DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printHeader(DebugManager.Logs.LevelGen, "Depth First Search");
            GridArray tmp = new GridArray(grids);
            tmp.Put(GridType.INTERNAL_RESERVED_CUR, startPoint.x, startPoint.y);
            tmp.toLog(DebugManager.Logs.LevelGen, "Starting Map:");
        }
        #endregion
        // Init
        GridType[,] arr = grids.GetArr();
        Stack<Value2D<GridType>> pathTaken = new Stack<Value2D<GridType>>();
        Array2D<bool> blockedPoints = new Array2D<bool>(grids.GetBounding(), false);
        DFSFilter filter = new DFSFilter(blockedPoints, targets);
        #region DEBUG
        GridArray debugGrid = new GridArray(0, 0);
        #endregion

        // Push start point onto path
        pathTaken.Push(startPoint);
        while (pathTaken.Count > 0)
        {
            startPoint = pathTaken.Peek();
            // Don't want to visit the same point on a different route later
            blockedPoints.Put(true, startPoint.x, startPoint.y);
            #region DEBUG
            if (DebugManager.Flag(DebugManager.DebugFlag.SearchSteps) && DebugManager.logging(DebugManager.Logs.LevelGen))
            { // Set up new print array
                debugGrid = new GridArray(grids);
                // Fill in blocked points
                foreach (Value2D<bool> blockedPt in blockedPoints)
                {
                    if (blockedPt.val)
                    {
                        debugGrid.Put(GridType.INTERNAL_RESERVED_BLOCKED, blockedPt.x, blockedPt.y);
                    }
                }
                Path tmpPath = new Path(pathTaken);
                debugGrid.PutAll(tmpPath.GetArray());
            }
            #endregion

            // Get surrounding points
            Surrounding<GridType> options = Surrounding<GridType>.Get(arr, startPoint.x, startPoint.y);
            #region DEBUG
            if (DebugManager.Flag(DebugManager.DebugFlag.SearchSteps) && DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                debugGrid.toLog(DebugManager.Logs.LevelGen, "Current Map with " + options.Count() + " options.");
            }
            #endregion

            // If found target, return path we took
            Value2D<GridType> targetDir = options.GetDirWithVal(targets, filter);
            if (targetDir != null)
            {
                #region DEBUG
                if (DebugManager.Flag(DebugManager.DebugFlag.SearchSteps) && DebugManager.logging(DebugManager.Logs.LevelGen))
                {
                    DebugManager.w(DebugManager.Logs.LevelGen, "===== FOUND TARGET: " + startPoint);
                }
                #endregion
                pathTaken.Push(targetDir);
                return pathTaken;
            }

            // Didn't find target, pick random direction
            targetDir = options.GetRandom(rand, filter);
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
        return pathTaken;
    }

    public static Array2D<bool> BreadthFirstFill(Value2D<GridType> startPoint, GridArray grids, params GridType[] targets)
    {
        return BreadthFirstFill(startPoint, grids, new HashSet<GridType>(targets));
    }

    // Not tested
    public static Array2D<bool> BreadthFirstFill(Value2D<GridType> startPoint, GridArray grids, HashSet<GridType> targets)
    {
        #region DEBUG
        if (DebugManager.Flag(DebugManager.DebugFlag.SearchSteps) && DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printHeader(DebugManager.Logs.LevelGen, "Breadth First Search Fill");
            GridArray tmp = new GridArray(grids);
            tmp.Put(GridType.INTERNAL_RESERVED_CUR, startPoint.x, startPoint.y);
            tmp.toLog(DebugManager.Logs.LevelGen, "Starting Map:");
        }
        #endregion
        Queue<Value2D<GridType>> queue = new Queue<Value2D<GridType>>();
        queue.Enqueue(startPoint);
        GridType[,] targetArr = grids.GetArr();
        Array2D<bool> outGridArr = new Array2D<bool>(grids.GetBoundingInternal(), false);
        outGridArr.Put(true, startPoint.x, startPoint.y);
        Value2D<GridType> curPoint;
        while (queue.Count > 0)
        {
            curPoint = queue.Dequeue();
            Surrounding<GridType> options = Surrounding<GridType>.Get(targetArr, curPoint.x, curPoint.y);
            #region DEBUG
            if (DebugManager.Flag(DebugManager.DebugFlag.SearchSteps) && DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                outGridArr.toLog(DebugManager.Logs.LevelGen, "Current Map with " + options.Count() + " options. Evaluating " + curPoint);
            }
            #endregion
            foreach (Value2D<GridType> option in options)
            {
                if (targets.Contains(option.val) && !outGridArr.Get(option.x, option.y))
                {
                    queue.Enqueue(option);
                    outGridArr.Put(true, option.x, option.y);
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
