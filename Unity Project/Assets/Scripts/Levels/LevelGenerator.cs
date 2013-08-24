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
    public static int minRectSize { get { return 8; } }
    public static int maxRectSize { get { return 20; } }

    // Circular Room Size (including walls)
    public static int minRadiusSize { get { return 6; } }
    public static int maxRadiusSize { get { return 15; } }

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

    // Cluster probabilities
    public static int minRoomClusters { get { return 2; } }
    public static int maxRoomClusters { get { return 5; } }
    public static int clusterProbability { get { return 90; } }
    #endregion

    public LevelLayout GenerateLayout(int levelDepth)
    {
        return GenerateLayout(levelDepth, Probability.LevelRand.Next());
    }

    public LevelLayout GenerateLayout(int levelDepth, int seed)
    {
        if (seed == -1)
        {
            return GenerateLayout(levelDepth);
        }
        #region DEBUG
        int debugNum = 1;
        float stepTime = 0, startTime = 0;
        if (DebugManager.logging(DebugManager.Logs.LevelGenMain))
        {
            if (DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                DebugManager.printHeader(DebugManager.Logs.LevelGenMain, "Generating Level: " + levelDepth);
                DebugManager.w(DebugManager.Logs.LevelGenMain, "Random seed int: " + seed);
                DebugManager.CreateNewLog(DebugManager.Logs.LevelGen, "Level Depth " + levelDepth + "/" + levelDepth + " " + debugNum++ + " - Generate Rooms");
                DebugManager.printHeader(DebugManager.Logs.LevelGen, "Generating level: " + levelDepth);
            }
            stepTime = Time.realtimeSinceStartup;
            startTime = stepTime;
        }
        #endregion
        Probability.LevelRand.SetSeed(seed);
        LevelLayout layout = new LevelLayout();
        List<Room> rooms = GenerateRoomShells();
        ModRooms(rooms);
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGenMain))
        {
            DebugManager.w(DebugManager.Logs.LevelGenMain, "Modding Rooms took: " + (Time.realtimeSinceStartup - stepTime));
            stepTime = Time.realtimeSinceStartup;
            if (DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                DebugManager.CreateNewLog(DebugManager.Logs.LevelGen, "Level Depth " + levelDepth + "/" + levelDepth + " " + debugNum++ + " - Cluster Rooms");
            }
        }
        #endregion
        ClusterRooms(rooms);
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGenMain))
        {
            DebugManager.w(DebugManager.Logs.LevelGenMain, "Cluster Rooms took: " + (Time.realtimeSinceStartup - stepTime));
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
            if (DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                DebugManager.CreateNewLog(DebugManager.Logs.LevelGen, "Level Depth " + levelDepth + "/" + levelDepth + " " + debugNum++ + " - Confirm Edges");
            }
        }
        #endregion
        ConfirmEdges(layout);
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGenMain))
        {
            DebugManager.w(DebugManager.Logs.LevelGenMain, "Confirm Edges took: " + (Time.realtimeSinceStartup - stepTime));
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

    List<Room> GenerateRoomShells()
    {
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printHeader(DebugManager.Logs.LevelGen, "Generate Rooms");
        }
        #endregion
        List<Room> rooms = new List<Room>();
        int numRooms = Probability.LevelRand.Next(minRooms, maxRooms);
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
                mod.Modify(room, Probability.LevelRand);
            }
            room.Bake(false);
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
        int numFlex = Probability.LevelRand.Next(1, maxFlexMod);
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
        if (chanceNoFinalMod < Probability.LevelRand.Next(100))
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

    static List<LayoutObject> ClusterRooms(List<Room> rooms)
    {
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printHeader(DebugManager.Logs.LevelGen, "Custer Rooms");
        }
        #endregion
        List<LayoutObject> ret = new List<LayoutObject>();
        int numClusters = Probability.LevelRand.Next(maxRoomClusters - minRoomClusters) + minRoomClusters;
        // Num clusters cannot be more than half num rooms
        if (numClusters > rooms.Count / 2)
            numClusters = rooms.Count / 2;
        List<LayoutCluster> clusters = new List<LayoutCluster>().Populate(numClusters);
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.w(DebugManager.Logs.LevelGen, "Number of clusters: " + numClusters);
        }
        #endregion
        // Add two rooms to each
        foreach (LayoutCluster cluster in clusters)
        {
            cluster.AddObject(rooms.Take());
            cluster.AddObject(rooms.Take());
            #region DEBUG
            if (DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                cluster.ToLog(DebugManager.Logs.LevelGen);
            }
            #endregion
        }
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.w(DebugManager.Logs.LevelGen, "Rooms left: " + rooms.Count);
        }
        #endregion
        // For remaining rooms, put into random clusters
        foreach (Room r in rooms)
        {
            if (Probability.LevelRand.Percent(clusterProbability))
            {
                LayoutCluster cluster = clusters.Random(Probability.LevelRand);
                cluster.AddObject(r);
                #region DEBUG
                if (DebugManager.logging(DebugManager.Logs.LevelGen))
                {
                    cluster.ToLog(DebugManager.Logs.LevelGen);
                }
                #endregion
            }
        }
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            foreach (LayoutCluster cluster in clusters)
            {
                cluster.ToLog(DebugManager.Logs.LevelGen);
            }
            DebugManager.printFooter(DebugManager.Logs.LevelGen);
        }
        #endregion
        return ret;
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
            int roomNum = Probability.LevelRand.Next(placedRooms.Count);
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
            while ((intersect = room.IntersectObjBounds(placedRooms, 1)) != null)
            {
                #region DEBUG
                if (DebugManager.logging(DebugManager.Logs.LevelGen))
                {
                    DebugManager.w(DebugManager.Logs.LevelGen, "This layout led to an overlap: " + room.GetBounding());
                    layout.ToLog(DebugManager.Logs.LevelGen);
                }
                #endregion
                room.ShiftOutside(intersect, shiftMagn, true, true);
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
            int numDoors = Probability.LevelRand.Next(doorsMin, doorsMax);
            #region DEBUG
            if (DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                potentialDoors.ToLog(DebugManager.Logs.LevelGen, "Potential Doors");
                DebugManager.w(DebugManager.Logs.LevelGen, "Number of doors to generate: " + numDoors);
            }
            #endregion
            for (int i = 0; i < numDoors; i++)
            {
                Value2D<GridType> doorSpace = potentialDoors.RandomValue(Probability.LevelRand);
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
            layout.ToLog(DebugManager.Logs.LevelGen, "Pre Path Layout");
        }
        #endregion
        var grids = layout.GetArray(layoutMargin);
        GridMap doors = layout.getTypes(grids, GridType.Door);
        Surrounding<GridType> surround = new Surrounding<GridType>(grids);
        #region DEBUG
        GridArray debugArr = null;
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            debugArr = new GridArray(grids);
        }
        #endregion
        foreach (var door in doors)
        {
            // Block nearby floor from being found
            surround.Load(door);
            Value2D<GridType> floor = surround.GetDirWithVal(true, GridType.Floor);
            if (floor != null)
            {
                grids[floor] = GridType.INTERNAL_RESERVED_BLOCKED;
            }

            var path = new Path(door, grids);
            #region DEBUG
            if (DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                GridArray messyPathArr = new GridArray(debugArr);
                messyPathArr.PutAll(path.GetArray());
                messyPathArr.ToLog(DebugManager.Logs.LevelGen, "Map after placing for door: " + door);
            }
            #endregion
            if (path.isValid())
            {
                path.Simplify();
                path.ConnectEnds(layout);
                #region DEBUG
                if (DebugManager.logging(DebugManager.Logs.LevelGen))
                {
                    debugArr.PutAll(path);
                }
                #endregion
                path.Bake(true);
                grids.PutAll(path);
                layout.AddPath(path);
            }
            #region DEBUG
            if (DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                debugArr.ToLog(DebugManager.Logs.LevelGen, "Map after simplifying path for door: " + door);
            }
            #endregion
        }
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            layout.ToLog(DebugManager.Logs.LevelGen, "Final Layout");
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

    private static void ConfirmEdges(LevelLayout layout)
    {
        layout.ShiftAll(1,1);
        GridArray arr = layout.GetArray(1);
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printHeader(DebugManager.Logs.LevelGen, "Confirm Edges");
            layout.ToLog(DebugManager.Logs.LevelGen, "Pre Confirm Edges");
        }
        #endregion
        Surrounding<GridType> surround = new Surrounding<GridType>(arr, true);
        LayoutObjectLeaf leaf = new LayoutObjectLeaf(arr.getWidth(), arr.getHeight());
        layout.AddObject(leaf);
        foreach (Value2D<GridType> val in arr)
        {
            if (val.val == GridType.Floor)
            {
                surround.Load(val);
                foreach (Value2D<GridType> neighbor in surround)
                {
                    if (neighbor.val == GridType.NULL)
                    {
                        leaf.put(GridType.Wall, neighbor.x, neighbor.y);
                    }
                }
            }
        }
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            layout.ToLog(DebugManager.Logs.LevelGen, "Post Confirm Edges");
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
        layoutArr.PutAs(layoutArr, GridType.INTERNAL_RESERVED_BLOCKED);
        Container2D<GridType>.Smallest(obj1.GetConnectedGrid(), obj2.GetConnectedGrid(), out smallest, out largest);
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            smallest.ToLog(DebugManager.Logs.LevelGen, "Smallest");
            largest.ToLog(DebugManager.Logs.LevelGen, "Largest");
        }
        #endregion
        DFSSearcher searcher = new DFSSearcher(Probability.LevelRand);
        var startPtStack = searcher.Search(new Value2D<GridType>(), smallest, GridType.NULL, Path.PathTypes());
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

    public static Point GenerateShiftMagnitude(int mag)
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
