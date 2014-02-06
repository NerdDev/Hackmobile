using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

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
    public static int minDoorSpacing { get { return 1; } }

    // Room modifier probabilies
    public static int maxFlexMod { get { return 5; } } //Max not inclusive
    public static int chanceNoFinalMod { get { return 40; } }

    // Cluster probabilities
    public static int minRoomClusters { get { return 2; } }
    public static int maxRoomClusters { get { return 5; } }
    public static int clusterProbability { get { return 90; } }

    // Stairs
    public const float MinStairDist = 30;
    #endregion

    public Theme Theme;
    public System.Random Rand;
    public int Depth;
    protected LevelLayout Layout;
    protected List<LayoutObject> Rooms;
    private int _debugNum = 0;

    public LevelGenerator()
    {
    }

    public LevelLayout Generate()
    {
        #region DEBUG
        float startTime = 0;
        if (BigBoss.Debug.logging(Logs.LevelGenMain))
        {
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                BigBoss.Debug.printHeader(Logs.LevelGenMain, "Generating Level: " + Depth);
            }
            startTime = Time.realtimeSinceStartup;
        }
        #endregion
        Layout = new LevelLayout();
        Log("Mod Rooms", false, GenerateRoomShells, ModRooms);
        //Log("Place Clusters", true, ClusterRooms);
        Log("Place Rooms", true, PlaceRooms);
        Log("Confirm Connection", true, ConfirmConnection);
        Log("Confirm Edges", true, ConfirmEdges);
        Log("Place Stairs", true, PlaceStairs);
        #region DEBUG
        if (BigBoss.Debug.logging())
        {
            BigBoss.Debug.w(Logs.LevelGenMain, "Generate Level took: " + (Time.realtimeSinceStartup - startTime));
            Layout.ToLog(Logs.LevelGenMain);
            BigBoss.Debug.printFooter(Logs.LevelGenMain, "Generating Level: " + Depth);
        }
        #endregion
        return Layout;
    }

    protected void Log(string name, bool newLog, params Action[] a)
    {
        float time = 0;
        if (BigBoss.Debug.logging(Logs.LevelGenMain) ||
            BigBoss.Debug.logging(Logs.LevelGen))
        {
            time = Time.realtimeSinceStartup;
        }
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            if (newLog)
                BigBoss.Debug.CreateNewLog(Logs.LevelGen, "Level Depth " + Depth + "/" + Depth + " " + _debugNum++ + " - " + name);
            BigBoss.Debug.w(Logs.LevelGen, "Start time: " + time);
        }
        foreach (Action action in a)
            action();
        if (BigBoss.Debug.logging(Logs.LevelGenMain))
        {
            BigBoss.Debug.w(Logs.LevelGen, "End time: " + Time.realtimeSinceStartup + ", Total time: " + (Time.realtimeSinceStartup - time));
            BigBoss.Debug.w(Logs.LevelGenMain, name + " took: " + (Time.realtimeSinceStartup - time));
        }
    }
    protected void GenerateRoomShells()
    {
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Generate Rooms");
        }
        #endregion
        List<LayoutObject> rooms = new List<LayoutObject>();
        int numRooms = Rand.Next(minRooms, maxRooms);
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.w(Logs.LevelGen, "Generating " + numRooms + " rooms.");
        }
        #endregion
        for (int i = 1; i <= numRooms; i++)
        {
            LayoutObject room = new LayoutObject();
            rooms.Add(room);
        }
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printFooter(Logs.LevelGen, "Generate Rooms");
        }
        #endregion
        Rooms = rooms;
    }

    void ModRooms()
    {
        foreach (LayoutObject room in Rooms)
        {
            #region DEBUG
            double stepTime = 0, time = 0;
            if (BigBoss.Debug.logging(Logs.LevelGenMain))
            {
                BigBoss.Debug.w(Logs.LevelGenMain, "Mods for " + room);
            }
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                BigBoss.Debug.CreateNewLog(Logs.LevelGen, "Level Depth " + Depth + "/" + Depth + " " + 0 + " - Generate Room " + room.Id);
                BigBoss.Debug.printHeader(Logs.LevelGen, "Modding " + room);
                time = Time.realtimeSinceStartup;
            }
            #endregion
            RoomSpec spec = new RoomSpec(room, Depth, Theme, Rand);
            foreach (RoomModifier mod in PickMods())
            {
                #region DEBUG
                if (BigBoss.Debug.logging(Logs.LevelGenMain))
                {
                    BigBoss.Debug.w(Logs.LevelGenMain, "   Applying: " + mod);
                }
                if (BigBoss.Debug.logging(Logs.LevelGen))
                {
                    stepTime = Time.realtimeSinceStartup;
                    BigBoss.Debug.w(Logs.LevelGen, "Applying: " + mod);
                }
                #endregion
                mod.Modify(spec);
                #region DEBUG
                if (BigBoss.Debug.logging(Logs.LevelGen))
                {
                    spec.Room.ToLog(Logs.LevelGen);
                    BigBoss.Debug.w(Logs.LevelGen, "Applying " + mod + " took " + (Time.realtimeSinceStartup - stepTime) + " seconds.  Total time: " + (Time.realtimeSinceStartup - time));
                }
                #endregion
            }
            room.Bake();
            #region DEBUG
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                room.ToLog(Logs.LevelGen);
                BigBoss.Debug.w(Logs.LevelGen, "Modding " + room + " took " + (Time.realtimeSinceStartup - time) + " seconds.");
                BigBoss.Debug.printFooter(Logs.LevelGen, "Modding " + room);
            }
            #endregion
        }
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGenMain))
        {
            foreach (LayoutObject room in Rooms)
                room.ToLog(Logs.LevelGenMain);
        }
        #endregion
    }

    protected List<RoomModifier> PickMods()
    {
        List<RoomModifier> mods = new List<RoomModifier>();
        RoomModifier baseMod = RoomModifier.GetBase(Rand);
        mods.Add(baseMod);
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.w(Logs.LevelGen, "Picked Base Mod: " + baseMod);
        }
        #endregion
        int numFlex = Rand.Next(1, maxFlexMod);
        List<RoomModifier> flexMods = RoomModifier.GetFlexible(numFlex, Rand);
        mods.AddRange(flexMods);
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.w(Logs.LevelGen, "Picked " + numFlex + " flex modifiers: ");
            foreach (RoomModifier mod in flexMods)
            {
                BigBoss.Debug.w(Logs.LevelGen, 1, mod.ToString());
            }
        }
        #endregion
        if (chanceNoFinalMod < Rand.Next(100))
        {
            RoomModifier finalMod = RoomModifier.GetFinal(Rand);
            mods.Add(finalMod);
            #region DEBUG
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                BigBoss.Debug.w(Logs.LevelGen, "Picked Final Mod: " + finalMod);
            }
            #endregion
        }
        return mods;
    }

    protected void ClusterRooms()
    {
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Cluster Rooms");
        }
        #endregion
        List<LayoutObject> ret = new List<LayoutObject>();
        int numClusters = Rand.Next(maxRoomClusters - minRoomClusters) + minRoomClusters;
        // Num clusters cannot be more than half num rooms
        if (numClusters > Rooms.Count / 2)
            numClusters = Rooms.Count / 2;
        List<LayoutCluster> clusters = new List<LayoutCluster>();
        for (int i = 0; i < numClusters; i++)
            clusters.Add(new LayoutCluster(Rand));
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.w(Logs.LevelGen, "Number of clusters: " + numClusters);
        }
        #endregion
        // Add two rooms to each
        foreach (LayoutCluster cluster in clusters)
        {
            cluster.AddObject(Rooms.Take());
            cluster.AddObject(Rooms.Take());
            #region DEBUG
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                cluster.ToLog(Logs.LevelGen);
            }
            #endregion
        }
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.w(Logs.LevelGen, "Rooms left: " + Rooms.Count);
        }
        #endregion
        // For remaining rooms, put into random clusters
        foreach (LayoutObject r in Rooms)
        {
            if (Rand.Percent(clusterProbability))
            {
                LayoutCluster cluster = clusters.Random(Rand);
                cluster.AddObject(r);
                #region DEBUG
                if (BigBoss.Debug.logging(Logs.LevelGen))
                {
                    cluster.ToLog(Logs.LevelGen);
                }
                #endregion
            }
        }
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            foreach (LayoutCluster cluster in clusters)
            {
                cluster.ToLog(Logs.LevelGen);
            }
            BigBoss.Debug.printFooter(Logs.LevelGen, "Cluster Rooms");
        }
        #endregion
    }

    protected void PlaceRooms()
    {
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Place Rooms");
        }
        #endregion
        List<LayoutObject> unplacedRooms = new List<LayoutObject>(Rooms.Cast<LayoutObject>());
        List<LayoutObject> placedRooms = new List<LayoutObject>();
        LayoutObject seedRoom = new LayoutObject();
        Layout.AddObject(seedRoom);
        placedRooms.Add(seedRoom); // Seed empty center room to start positioning from.
        foreach (LayoutObject room in unplacedRooms)
        {
            // Find room it will start from
            int roomNum = Rand.Next(placedRooms.Count);
            LayoutObject startRoom = placedRooms[roomNum];
            room.CenterOn(startRoom);
            // Find where it will shift away
            Point shiftMagn = GenerateShiftMagnitude(shiftRange, Rand);
            #region DEBUG
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                BigBoss.Debug.w(Logs.LevelGen, "Placing room: " + room);
                BigBoss.Debug.w(Logs.LevelGen, "Picked starting room number: " + roomNum);
                BigBoss.Debug.w(Logs.LevelGen, "Shift: " + shiftMagn);
            }
            #endregion
            room.ShiftOutside(placedRooms, shiftMagn);
            placedRooms.Add(room);
            Layout.AddRoom(room);
            Layout.RemoveObject(seedRoom);
            #region DEBUG
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                Layout.ToLog(Logs.LevelGen, "Layout after placing room at: " + room.GetBounding(true));
                BigBoss.Debug.printBreakers(Logs.LevelGen, 4);
            }
            #endregion
        }
        #region DEBUG
        BigBoss.Debug.printFooter(Logs.LevelGen, "Place Rooms");
        #endregion
    }

    protected void PlaceDoors()
    {
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Place Doors");
        }
        #endregion
        foreach (LayoutObject room in Rooms)
        {
            #region DEBUG
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                BigBoss.Debug.printHeader(Logs.LevelGen, "Place Doors on " + room);
            }
            #endregion
            var potentialDoors = new MultiMap<GridType>();
            room.Grids.DrawPotentialExternalDoors(Draw.AddTo(potentialDoors));
            int numDoors = Rand.Next(doorsMin, doorsMax);
            #region DEBUG
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                potentialDoors.ToLog(Logs.LevelGen, "Potential Doors");
                BigBoss.Debug.w(Logs.LevelGen, "Number of doors to generate: " + numDoors);
            }
            #endregion
            foreach (Value2D<GridType> doorSpace in potentialDoors.Random(Rand, numDoors, minDoorSpacing))
            {
                room.Grids[doorSpace] = GridType.Door;
            }
            #region DEBUG
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                room.ToLog(Logs.LevelGen, "Room After placing doors");
                BigBoss.Debug.printFooter(Logs.LevelGen, "Place Doors on " + room);
            }
            #endregion
        }
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printFooter(Logs.LevelGen, "Place Doors");
        }
        #endregion
    }

    protected void PlacePaths()
    {
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Place Paths");
        }
        #endregion
        Container2D<GridType> grids = Layout.Bake();
        MultiMap<GridType> doors = new MultiMap<GridType>();
        // If a door and near a null
        grids.DrawAll(Draw.EqualTo(GridType.Door).And(Draw.HasAround(false, Draw.EqualTo(GridType.NULL))).IfThen(Draw.AddTo(doors)));
        foreach (var door in doors)
        {
            // Block nearby walkable
            grids.DrawAround(door.x, door.y, false, Draw.If<GridType>(GridTypeEnum.Walkable).IfThen(Draw.SetTo(GridType.INTERNAL_RESERVED_BLOCKED)));
        }
        Point shift;
        Array2DRaw<GridType> rawArr = grids.RawArray(out shift);
        rawArr.Expand(20);
        shift.Shift(-20, -20);
        #region DEBUG
        Container2D<GridType> debugArr = null;
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            grids.ToLog("Initial Map");
            doors.ToLog(Logs.LevelGen, "Doors to connect");
            debugArr = new MultiMap<GridType>(grids, -shift);
        }
        #endregion
        foreach (var door in doors)
        {
            door.Shift(-shift);
            Stack<Value2D<GridType>> stack = rawArr.DrawDepthFirstSearch(
                door.x,
                door.y,
                Draw.EqualTo(GridType.NULL),
                Draw.ContainedIn(Path.PathTypes),
                Rand, true);
            var path = new Path(stack);
            if (path.isValid())
            {
                #region DEBUG
                if (BigBoss.Debug.logging(Logs.LevelGen))
                {
                    MultiMap<GridType> messyPathArr = new MultiMap<GridType>(debugArr);
                    messyPathArr.PutAll(path.Grids);
                    messyPathArr.ToLog(Logs.LevelGen, "Map after placing for door: " + door);
                }
                #endregion
                path.Simplify();
                path.ConnectEnds(Layout, shift);
                path.Bake();
                rawArr.PutAll(path.Grids);
                path.Shift(shift);
                Layout.AddPath(path);
                #region DEBUG
                if (BigBoss.Debug.logging(Logs.LevelGen))
                {
                    debugArr.PutAll(path.Grids);
                    debugArr.ToLog(Logs.LevelGen, "Map after simplifying path for door: " + door);
                    List<LayoutObject> list = path.ConnectedToAll();
                    BigBoss.Debug.w(Logs.LevelGen, path + " connected to:");
                    foreach (LayoutObject obj in list)
                        BigBoss.Debug.w(Logs.LevelGen, "   " + obj);
                }
                #endregion
            }
        }
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            Layout.ToLog(Logs.LevelGen, "Final Layout");
            BigBoss.Debug.printFooter(Logs.LevelGen, "Place Paths");
        }
        #endregion
    }

    protected void ConfirmConnection()
    {
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Confirm Connections");
        }
        #endregion
        DrawAction<GridType> passTest = Draw.ContainedIn(Path.PathTypes).Or(Draw.CanDrawDoor());
        Container2D<GridType> layoutCopy = Layout.Bake().Array;
        List<LayoutObject> rooms = new List<LayoutObject>(Layout.GetRooms().Cast<LayoutObject>());
        LayoutObject startingRoom = rooms.Take();
        startingRoom.Grids.DrawAll(Draw.SetTo(layoutCopy, GridType.INTERNAL_RESERVED_BLOCKED));
        LayoutObject fail;
        while (!startingRoom.ConnectedTo(rooms, out fail))
        {
            // Find start points
            #region DEBUG
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                runningConnected.ToLog(Logs.LevelGen, "Source Setup");
                fail.ToLog(Logs.LevelGen, "Failed to connect to this");
            }
            #endregion
            Value2D<GridType> startPoint;
            if (!FindClosestPoint(layoutCopy, fail, out startPoint))
            {
                throw new ArgumentException("Cannot find path to fail room");
            }
            
            // Connect
            #region DEBUG
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                layoutCopy[startPoint.x, startPoint.y] = GridType.INTERNAL_RESERVED_CUR;
                layoutCopy.ToLog(Logs.LevelGen, "Largest after putting blocked");
                BigBoss.Debug.w(Logs.LevelGen, "Start Point:" + startPoint);
            }
            #endregion
            List<Value2D<GridType>> stack = layoutCopy.DrawJumpTowardsSearch(
            startPoint.x,
            startPoint.y,
            3,
            5,
            Draw.EqualTo(GridType.NULL).And(Draw.Inside<GridType>(layoutCopy.Bounding.Expand(5))),
            passTest,
            Rand,
            fail.Center,
            true);
            var path = new Path(stack);
            if (path.isValid())
            {
                #region DEBUG
                if (BigBoss.Debug.logging(Logs.LevelGen))
                {
                    path.ToLog(Logs.LevelGen, "Connecting Path");
                }
                #endregion
                path.Simplify();
                path.ConnectEnds(Layout, new Point(0, 0));
                Point first = path.FirstEnd;
                Point second = path.SecondEnd;
                LayoutObject leaf1 = Layout.GetObjAt(first);
                LayoutObject leaf2 = Layout.GetObjAt(second);
                if (leaf1[first] == GridType.Wall)
                {
                    leaf1[first] = GridType.Door;
                }
                if (leaf2[second] == GridType.Wall)
                {
                    leaf2[second] = GridType.Door;
                }
                path.Bake();
                Layout.AddPath(path);
                runningConnected.PutAll(path.Grids);
                #region DEBUG
                if (BigBoss.Debug.logging(Logs.LevelGen))
                {
                    layoutCopy[startPoint.x, startPoint.y] = GridType.INTERNAL_RESERVED_BLOCKED;
                    Layout.ToLog(Logs.LevelGen, "Final Connection");
                }
                #endregion
            }
            #region DEBUG
            else if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                layoutCopy[startPoint.x, startPoint.y] = GridType.INTERNAL_RESERVED_BLOCKED;
                BigBoss.Debug.w(Logs.LevelGen, "Could not make an initial start point connection.");
            }
            #endregion
        }
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printFooter(Logs.LevelGen, "Confirm Connections");
        }
        #endregion
    }

    protected bool FindClosestPoint(Container2D<GridType> map, LayoutObject smaller, out Value2D<GridType> found)
    {
        MultiMap<GridType> inside = new MultiMap<GridType>();
        MultiMap<GridType> perim = new MultiMap<GridType>();
        smaller.DrawPerimeter(Draw.Not(Draw.EqualTo(GridType.NULL)), new StrokedAction<GridType>()
        {
            UnitAction = Draw.AddTo(inside, smaller.ShiftP),
            StrokeAction = Draw.AddTo(perim, smaller.ShiftP)
        }, false);
        return map.DrawBreadthFirstSearch(
            perim.Cast<Point>(), inside.Cast<Point>(), false,
            Draw.EqualTo(GridType.NULL),
            Draw.Not(Draw.EqualTo(GridType.NULL)),
            out found);
    }

    private void ConfirmEdges()
    {
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Confirm Edges");
            Layout.ToLog(Logs.LevelGen, "Pre Confirm Edges");
        }
        #endregion
        LayoutObject edgeObject = new LayoutObject();
        Container2D<GridType> grids = Layout.Bake();
        grids.DrawAll(Draw.Not(Draw.EqualTo(GridType.NULL).Or(Draw.EqualTo(GridType.Wall))).IfThen((arr, x, y) =>
            {
                grids.DrawAround(x, y, true, Draw.EqualTo(GridType.NULL).IfThen(Draw.SetTo(edgeObject.Grids, GridType.Wall)));
                return true;
            }));
        Layout.AddObject(edgeObject);
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            edgeObject.ToLog(Logs.LevelGen, "Edge Object");
            Layout.ToLog(Logs.LevelGen, "Post Confirm Edges");
            BigBoss.Debug.printFooter(Logs.LevelGen, "Confirm Edges");
        }
        #endregion
    }

    protected void PlaceStairs()
    {
        #region Debug
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Placing missing stairs");
        }
        #endregion
        Layout.UpStart = PlaceMissingStair(true, null);
        Layout.DownStart = PlaceMissingStair(false, Layout.UpStart);
        #region Debug
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printFooter("Placing Missing Stairs");
        }
        #endregion
    }

    protected Point PlaceMissingStair(bool up, Point otherStair)
    {
        foreach (LayoutObject room in Rooms.Randomize(Rand))
        {
            MultiMap<GridType> options = new MultiMap<GridType>();
            DrawAction<GridType> test = Draw.CanDrawStair();
            if (otherStair != null)
            {
                double farthest;
                double closest;
                room.Grids.Bounding.DistanceTo(otherStair, out closest, out farthest);
                if (farthest < MinStairDist)
                { // Inside or way too close
                    continue;
                }
                else if (closest < MinStairDist)
                { // On the edge.. could have a potential
                    test = test.And(Draw.Not(Draw.WithinTo<GridType>(MinStairDist, otherStair)));
                }
            }
            room.Grids.DrawAll(test.IfThen(Draw.AddTo(options)));

            // Place stair
            Value2D<GridType> picked;
            if (!options.Random(Rand, out picked)) continue;
            room.Grids[picked.x, picked.y] = up ? GridType.StairUp : GridType.StairDown;

            // Place startpoint
            MultiMap<GridType> startOptions = new MultiMap<GridType>();
            room.Grids.DrawAround(picked.x, picked.y, false, Draw.EqualTo(GridType.Floor).IfThen(Draw.AddTo(startOptions)));
            Value2D<GridType> start;
            startOptions.Random(Rand, out start);
            room.Grids[start] = GridType.StairPlace;

            Point p = new Point(picked);
            p.Shift(room.ShiftP);
            #region Debug
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                options.ToLog(Logs.LevelGen, "Stair Options");
                room.ToLog(Logs.LevelGen, "Placed stairs");
            }
            #endregion
            return p;
        }
        return null;
    }

    protected void MakeConnection(LevelLayout layout, LayoutObject obj1, LayoutObject obj2)
    {
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Make Connection - " + obj1);
        }
        #endregion
        Container2D<GridType> smallest;
        Container2D<GridType> largest;
        Container2D<GridType>.Smallest(obj1.GetConnectedGrid(), obj2.GetConnectedGrid(), out smallest, out largest);
        Container2D<GridType> layoutArr = layout.Bake();
        MultiMap<GridType> startPoints = new MultiMap<GridType>();
        DrawAction<GridType> passTest = Draw.ContainedIn(Path.PathTypes).Or(Draw.EqualTo(GridType.Wall));
        smallest.DrawPerimeter(Draw.Not(Draw.EqualTo(GridType.NULL)), new StrokedAction<GridType>()
        {
            StrokeAction = passTest.IfThen(Draw.AddTo(startPoints))
        }, false);
        passTest = passTest.And(Draw.ContainedIn(largest));
        Value2D<GridType> startPoint;
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            smallest.ToLog(Logs.LevelGen, "Smallest");
            largest.ToLog(Logs.LevelGen, "Largest");
            startPoints.ToLog(Logs.LevelGen, "Start options");
        }
        #endregion
        if (startPoints.Random(Rand, out startPoint))
        {
            #region DEBUG
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                layoutArr[startPoint.x, startPoint.y] = GridType.INTERNAL_RESERVED_CUR;
                layoutArr.ToLog(Logs.LevelGen, "Largest after putting blocked");
                BigBoss.Debug.w(Logs.LevelGen, "Start Point:" + startPoint);
            }
            #endregion
            Stack<Value2D<GridType>> stack = layoutArr.DrawDepthFirstSearch(
            startPoint.x,
            startPoint.y,
            Draw.EqualTo(GridType.NULL).And(Draw.Inside<GridType>(layoutArr.Bounding.Expand(5))),
            passTest,
            Rand);
            var path = new Path(stack);
            if (path.isValid())
            {
                #region DEBUG
                if (BigBoss.Debug.logging(Logs.LevelGen))
                {
                    path.ToLog(Logs.LevelGen, "Connecting Path");
                }
                #endregion
                path.Simplify();
                path.ConnectEnds(Layout, new Point(0, 0));
                Point first = path.FirstEnd;
                Point second = path.SecondEnd;
                LayoutObject leaf1 = Layout.GetObjAt(first);
                LayoutObject leaf2 = Layout.GetObjAt(second);
                if (leaf1[first] == GridType.Wall)
                {
                    leaf1[first] = GridType.Door;
                }
                if (leaf2[second] == GridType.Wall)
                {
                    leaf2[second] = GridType.Door;
                }
                path.ConnectEnds(Layout, new Point());
                path.Bake();
                layout.AddPath(path);
                #region DEBUG
                if (BigBoss.Debug.logging(Logs.LevelGen))
                {
                    layout.ToLog(Logs.LevelGen, "Final Connection");
                }
                #endregion
            }
        }
        #region DEBUG
        else if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.w(Logs.LevelGen, "Could not make an initial start point connection.");
        }
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printFooter(Logs.LevelGen, "Make Connection - " + obj1);
        }
        #endregion
    }

    public static Point GenerateShiftMagnitude(int mag, System.Random rand)
    {
        UnityEngine.Random.seed = rand.Next();
        Vector2 vect = UnityEngine.Random.insideUnitCircle * mag;
        Point p = new Point(vect);
        while (p.isZero())
        {
            vect = UnityEngine.Random.insideUnitCircle * mag;
            p = new Point(vect);
        }
        return p;
    }
}
