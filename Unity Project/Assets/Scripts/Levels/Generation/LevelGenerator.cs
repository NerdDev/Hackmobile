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
        List<LayoutObjectContainer> clusters = new List<LayoutObjectContainer>();
        for (int i = 0; i < numClusters; i++)
            clusters.Add(new LayoutObjectContainer());
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.w(Logs.LevelGen, "Number of clusters: " + numClusters);
        }
        #endregion
        // Add two rooms to each
        foreach (LayoutObjectContainer cluster in clusters)
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
                LayoutObjectContainer cluster = clusters.Random(Rand);
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
            foreach (LayoutObjectContainer cluster in clusters)
            {
                cluster.ToLog(Logs.LevelGen);
            }
            BigBoss.Debug.printFooter(Logs.LevelGen, "Cluster Rooms");
        }
        #endregion
    }

    protected void ClusterAround(ILayoutObject cluster, LayoutObject obj2)
    {

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

    #region Confirm Connection / Pathing
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
        Container2D<GridType> runningConnected = Container2D<GridType>.CreateArrayFromBounds(layoutCopy);
        // Create initial queue and visited
        LayoutObject startingRoom = rooms.Take();
        startingRoom.DrawAll(Draw.AddTo(runningConnected, startingRoom.ShiftP));
        Container2D<bool> visited;
        Queue<Value2D<GridType>> queue;
        ConstructBFS(startingRoom, out queue, out visited);
        visited = visited.Array;
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
            Value2D<GridType> endPoint;
            LayoutObject hit;
            if (!FindNextPathPoints(layoutCopy, runningConnected, out hit, passTest, queue, visited, out startPoint, out endPoint))
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
            passTest.And(Draw.ContainedIn(hit)),
            Rand,
            endPoint,
            true);
            var path = new Path(stack);
            if (path.Valid)
            {
                #region DEBUG
                if (BigBoss.Debug.logging(Logs.LevelGen))
                {
                    path.Bake().ToLog(Logs.LevelGen, "Connecting Path");
                }
                #endregion
                path.Simplify();
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
                leaf1.Connect(leaf2);
                LayoutObject pathObj = path.Bake();
                foreach (var v in path)
                {
                    runningConnected.Put(v);
                    if (!visited[v])
                    {
                        queue.Enqueue(v);
                    }
                    visited[v] = true;
                }
                Layout.AddObject(pathObj);
                hit.DrawAll(Draw.AddTo(runningConnected, hit.ShiftP));
                #region DEBUG
                if (BigBoss.Debug.logging(Logs.LevelGen))
                {
                    Layout.ToLog(Logs.LevelGen, "Final Connection");
                }
                #endregion
            }
            else
            {
                throw new ArgumentException("Cannot create path to hit room");
            }
        }
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printFooter(Logs.LevelGen, "Confirm Connections");
        }
        #endregion
    }

    protected void ConstructBFS(LayoutObject obj,
        out Queue<Value2D<GridType>> queue,
        out Container2D<bool> visited)
    {
        visited = new MultiMap<bool>();
        queue = new Queue<Value2D<GridType>>();
        obj.DrawPerimeter(Draw.Not(Draw.EqualTo(GridType.NULL)), new StrokedAction<GridType>()
        {
            UnitAction = Draw.SetTo<GridType, bool>(visited, true, obj.ShiftP),
            StrokeAction = Draw.AddTo(queue, obj.ShiftP).And(Draw.SetTo<GridType, bool>(visited, true, obj.ShiftP))
        }, false);
    }
    
    protected bool FindNextPathPoints(Container2D<GridType> map, 
        Container2D<GridType> runningConnected,
        out LayoutObject hit, 
        DrawAction<GridType> pass,
        Queue<Value2D<GridType>> curQueue, 
        Container2D<bool> curVisited, 
        out Value2D<GridType> startPoint, 
        out Value2D<GridType> endPoint)
    {
        if (!map.DrawBreadthFirstSearch(
            curQueue, curVisited, false,
            Draw.EqualTo(GridType.NULL),
            pass,
            out endPoint))
        {
            hit = null;
            startPoint = null;
            return false;
        }
        hit = Layout.GetObjAt(endPoint);
        Container2D<bool> hitVisited;
        Queue<Value2D<GridType>> hitQueue;
        ConstructBFS(hit, out hitQueue, out hitVisited);
        curQueue.Enqueue(hitQueue);
        curVisited.PutAll(hitVisited);
        return map.DrawBreadthFirstSearch(
            hitQueue, hitVisited, false,
            Draw.EqualTo(GridType.NULL),
            pass.And(Draw.ContainedIn(runningConnected)),
            out startPoint);
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
    #endregion

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
