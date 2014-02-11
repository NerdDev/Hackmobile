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

    // Doors
    public const int desiredWallToDoorRatio = 10;
    #endregion

    public Theme Theme;
    public System.Random Rand;
    public int Depth;
    protected LevelLayout Layout;
    protected List<ILayoutObject> Objects;
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
        Log("Place Clusters", true, ClusterRooms);
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
        List<ILayoutObject> rooms = new List<ILayoutObject>();
        int numRooms = Rand.Next(minRooms, maxRooms);
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.w(Logs.LevelGen, "Generating " + numRooms + " rooms.");
        }
        #endregion
        for (int i = 1; i <= numRooms; i++)
        {
            LayoutObject room = new LayoutObject("Room");
            rooms.Add(room);
            Layout.Rooms.Add(room);
        }
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printFooter(Logs.LevelGen, "Generate Rooms");
        }
        #endregion
        Objects = rooms;
    }

    void ModRooms()
    {
        foreach (LayoutObject room in Objects)
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
            foreach (LayoutObject room in Objects)
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

    #region Clustering
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
        if (numClusters > Objects.Count / 2)
            numClusters = Objects.Count / 2;
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
            LayoutObject obj1 = (LayoutObject)Objects.Take();
            LayoutObject obj2 = (LayoutObject)Objects.Take();
            cluster.Objects.Add(obj1);
            cluster.Objects.Add(obj2);
            ClusterAround(obj1, obj2);
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
            BigBoss.Debug.w(Logs.LevelGen, "Rooms left: " + Objects.Count);
        }
        #endregion
        // For remaining rooms, put into random clusters
        foreach (LayoutObject r in new List<ILayoutObject>(Objects))
        {
            if (Rand.Percent(clusterProbability))
            {
                LayoutObjectContainer cluster = clusters.Random(Rand);
                ClusterAround(cluster, r);
                cluster.Objects.Add(r);
                Objects.Remove(r);
                #region DEBUG
                if (BigBoss.Debug.logging(Logs.LevelGen))
                {
                    cluster.ToLog(Logs.LevelGen);
                }
                #endregion
            }
        }
        // Add Clusters to rooms list
        foreach (ILayoutObject cluster in clusters)
        {
            Objects.Add(cluster);
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

    protected class ClusterInfo
    {
        public Point Shift;
        public List<Point> Intersects;
        public override string ToString()
        {
            return Shift.ToString();
        }
    }

    protected void ClusterAround(ILayoutObject cluster, LayoutObject obj)
    {
        #region Debug
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader("Cluster Around");
        }
        #endregion
        obj.ShiftOutside(cluster, new Point(1, 0), null, false, false);
        obj.Shift(-1, 0); // Shift to overlapping slightly
        MultiMap<bool> visited = new MultiMap<bool>();
        visited[0, 0] = true;
        ProbabilityList<ClusterInfo> shiftOptions = new ProbabilityList<ClusterInfo>(Rand);
        Queue<Point> shiftQueue = new Queue<Point>();
        shiftQueue.Enqueue(new Point());
        Container2D<GridType> clusterGrid = cluster.GetGrid();
        Container2D<GridType> objGrid = obj.GetGrid();
        #region Debug
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            MultiMap<GridType> tmp = new MultiMap<GridType>();
            tmp.PutAll(obj.GetGrid());
            tmp.PutAll(cluster.GetGrid());
            tmp.ToLog(Logs.LevelGen, "Starting placement");
        }
        #endregion
        while (shiftQueue.Count > 0)
        {
            Point curShift = shiftQueue.Dequeue();
            #region Debug
            if (BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps) && BigBoss.Debug.logging(Logs.LevelGen))
            {
                MultiMap<GridType> tmpMap = new MultiMap<GridType>();
                tmpMap.PutAll(clusterGrid);
                tmpMap.PutAll(objGrid, curShift);
                tmpMap.ToLog(Logs.LevelGen, "Analyzing at shift " + curShift);
            }
            #endregion
            // Test if pass
            List<Point> intersectPoints = new List<Point>();
            if (objGrid.DrawAll((arr, x, y) =>
            {
                if (GridTypeEnum.EdgeType(arr[x, y]))
                {
                    GridType clusterType = clusterGrid[x + curShift.x, y + curShift.y];
                    if (clusterType == GridType.NULL) return true;
                    intersectPoints.Add(new Point(x, y));
                    return GridTypeEnum.EdgeType(clusterType);
                }
                else
                {
                    return !clusterGrid.Contains(x + curShift.x, y + curShift.y);
                }
            })
                && intersectPoints.Count > 0)
            { // Passed test
                // queue surrounding points
                visited.DrawAround(curShift.x, curShift.y, true, Draw.Not(Draw.EqualTo(true)).IfThen(Draw.AddTo<bool>(shiftQueue).And(Draw.SetTo(true))));

                #region Debug
                if (BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps) && BigBoss.Debug.logging(Logs.LevelGen))
                {
                    BigBoss.Debug.w(Logs.LevelGen, "passed with " + intersectPoints.Count);
                }
                #endregion
                shiftOptions.Add(new ClusterInfo() { Shift = curShift, Intersects = intersectPoints }, Math.Pow(intersectPoints.Count, 3));
            }
        }
        ClusterInfo info = shiftOptions.Get();
        obj.Shift(info.Shift.x, info.Shift.y);
        obj.PlaceSomeDoors(info.Intersects, Rand, info.Shift);
        #region Debug
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            shiftOptions.ToLog(Logs.LevelGen, "Shift options");
            BigBoss.Debug.w(Logs.LevelGen, "picked" + info.Shift);
            MultiMap<GridType> tmpMap = new MultiMap<GridType>();
            tmpMap.PutAll(clusterGrid);
            tmpMap.PutAll(obj.GetGrid());
            tmpMap.ToLog(Logs.LevelGen, "Final setup " + info.Shift);
            BigBoss.Debug.printFooter("Cluster Around");
        }
        #endregion
    }
    #endregion

    protected void PlaceRooms()
    {
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Place Rooms");
        }
        #endregion
        List<ILayoutObject> unplacedRooms = new List<ILayoutObject>(Objects);
        List<ILayoutObject> placedRooms = new List<ILayoutObject>();
        if (unplacedRooms.Count > 0)
        { // Add seed
            ILayoutObject seed = unplacedRooms.Take();
            Layout.Objects.Add(seed);
            placedRooms.Add(seed);
        }
        foreach (ILayoutObject room in unplacedRooms)
        {
            // Find room it will start from
            int roomNum = Rand.Next(placedRooms.Count);
            ILayoutObject startRoom = placedRooms[roomNum];
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
            Layout.Objects.Add(room);
            #region DEBUG
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                Layout.ToLog(Logs.LevelGen, "Layout after placing room at: " + room.Bounding);
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
        foreach (LayoutObject room in Objects)
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
        Container2D<GridType> layoutCopy = Layout.GetGrid().Array;
        List<LayoutObject> rooms = new List<LayoutObject>(Layout.Rooms.Cast<LayoutObject>());
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
                LayoutObject leaf1, leaf2;
                LayoutObject pathObj = path.Bake();
                Layout.ConnectTo(first, pathObj, first, out leaf1, out pathObj);
                Layout.ConnectTo(second, pathObj, second, out leaf2, out pathObj);
                if (leaf1[first] == GridType.Wall)
                {
                    leaf1[first] = GridType.Door;
                }
                if (leaf2[second] == GridType.Wall)
                {
                    leaf2[second] = GridType.Door;
                }
                foreach (var v in pathObj)
                {
                    layoutCopy[v] = v.val;
                    runningConnected.Put(v);
                    if (!visited[v])
                    {
                        queue.Enqueue(v);
                    }
                    visited[v] = true;
                }
                Layout.Objects.Add(pathObj);
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
        if (!Layout.GetObjAt(endPoint, out hit))
        {
            startPoint = null;
            return false;
        }
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
        LayoutObject edgeObject = new LayoutObject("Edges");
        Container2D<GridType> grids = Layout.GetGrid();
        grids.DrawAll(Draw.Not(Draw.EqualTo(GridType.NULL).Or(Draw.EqualTo(GridType.Wall))).IfThen((arr, x, y) =>
            {
                grids.DrawAround(x, y, true, Draw.EqualTo(GridType.NULL).IfThen(Draw.SetTo(edgeObject.Grids, GridType.Wall)));
                return true;
            }));
        Layout.Objects.Add(edgeObject);
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
        if (!PlaceMissingStair(true, null, out Layout.UpStart)
            || !PlaceMissingStair(false, Layout.UpStart, out Layout.DownStart))
        {
            throw new ArgumentException("Could not place stairs");
        }
        #region Debug
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printFooter("Placing Missing Stairs");
        }
        #endregion
    }

    protected bool PlaceMissingStair(bool up, Point otherStair, out Point placed)
    {
        foreach (LayoutObject obj in Layout.Flatten().Randomize(Rand))
        {
            MultiMap<GridType> options = new MultiMap<GridType>();
            DrawAction<GridType> test = Draw.CanDrawStair();
            if (otherStair != null)
            {
                double farthest;
                double closest;
                obj.Grids.Bounding.DistanceTo(otherStair, out closest, out farthest);
                if (farthest < MinStairDist)
                { // Inside or way too close
                    continue;
                }
                else if (closest < MinStairDist)
                { // On the edge.. could have a potential
                    test = test.And(Draw.Not(Draw.WithinTo<GridType>(MinStairDist, otherStair)));
                }
            }
            obj.Grids.DrawAll(test.IfThen(Draw.AddTo(options)));

            // Place stair
            Value2D<GridType> picked;
            if (!options.Random(Rand, out picked)) continue;
            obj.Grids[picked.x, picked.y] = up ? GridType.StairUp : GridType.StairDown;

            // Place startpoint
            MultiMap<GridType> startOptions = new MultiMap<GridType>();
            obj.Grids.DrawAround(picked.x, picked.y, false, Draw.EqualTo(GridType.Floor).IfThen(Draw.AddTo(startOptions)));
            Value2D<GridType> start;
            startOptions.Random(Rand, out start);
            obj.Grids[start] = GridType.StairPlace;

            placed = new Point(picked);
            placed.Shift(obj.ShiftP);
            #region Debug
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                options.ToLog(Logs.LevelGen, "Stair Options");
                obj.ToLog(Logs.LevelGen, "Placed stairs");
                Layout.ToLog(Logs.LevelGen, "Layout");
            }
            #endregion
            return true;
        }
        placed = null;
        return false;
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
