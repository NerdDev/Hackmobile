using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using LevelGen;

public class LevelGenerator
{

    #region GlobalGenVariables
    // Number of Rooms
    public static int minRooms { get { return 8; } }
    public static int maxRooms { get { return 16; } } //Max not inclusive

    // Amount to shift rooms
    public static int shiftRange { get { return 10; } } //Max not inclusive

    // Number of doors per room
    public static int doorsMin { get { return 1; } }
    public static int doorsMax { get { return 5; } } //Max not inclusive
    public static int minDoorSpacing { get { return 1; } }

    // Cluster probabilities
    public static int minRoomClusters { get { return 2; } }
    public static int maxRoomClusters { get { return 5; } }
    public static int clusterProbability { get { return 90; } }

    // Stairs
    public const float MinStairDist = 30;

    // Doors
    public const int desiredWallToDoorRatio = 10;
    #endregion

    public Theme InitialTheme;
    public ProbabilityPool<ThemeSet> ThemeSetOptions;
    public System.Random Rand;
    public int Depth;
    protected LevelLayout Layout;
    protected LayoutObjectContainer<GenSpace> Container;
    protected List<Area> Areas = new List<Area>();
    protected List<ILayoutObject<GenSpace>> Objects;
    private int _debugNum = 0;

    private const double AREA_RADIUS_SHRINK = 5;

    public LevelGenerator()
    {
    }

    public LevelLayout Generate()
    {
        #region DEBUG
        float startTime = 0;
        if (BigBoss.Debug.logging(Logs.LevelGenMain))
        {
            BigBoss.Debug.printHeader(Logs.LevelGenMain, "Generating Level: " + Depth);
            startTime = Time.realtimeSinceStartup;
        }
        #endregion
        Layout = new LevelLayout() { Random = Rand };
        Container = new LayoutObjectContainer<GenSpace>();
        Log("Areas", true, GenerateAreas);
        Log("Mod Rooms", false, GenerateRoomShells, ModRooms);
        Log("Cluster", true, ClusterRooms);
        Log("Place Rooms", true, PlaceRooms);
        Log("Confirm Connection", true, ConfirmConnection);
        Log("Place Stairs", true, PlaceStairs);
        #region DEBUG
        if (BigBoss.Debug.logging())
        {
            BigBoss.Debug.w(Logs.LevelGenMain, "Generate Level took: " + (Time.realtimeSinceStartup - startTime));
            Container.ToLog(Logs.LevelGenMain);
            BigBoss.Debug.printFooter(Logs.LevelGenMain, "Generating Level: " + Depth);
        }
        #endregion
        Layout.Grids.PutAll(Container.GetGrid());
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
                BigBoss.Debug.CreateNewLog(Logs.LevelGen, "Level Depth " + Depth + "/" + Depth + " " + ++_debugNum + " - " + name);
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

    protected void GenerateAreas()
    {
        int numAreas = Rand.NextNormalDist(3, 10);
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Generate Areas");
            BigBoss.Debug.w(Logs.LevelGen, "Number of areas: " + numAreas);
        }
        #endregion

        for (int i = 0; i < numAreas; i++)
        {
            ThemeSet set = ThemeSetOptions.Get(Rand);

            Point center = new Point(0, 0);
            Vector2 dir = Rand.NextOnUnitCircle();
            bool intersects = true;
            while (intersects)
            {
                intersects = false;
                foreach (Area existingArea in Areas)
                {
                    double distance = existingArea.Center.Distance(center);
                    double existingRadius = existingArea.Set.AvgRadius / AREA_RADIUS_SHRINK;
                    double radius = set.AvgRadius / AREA_RADIUS_SHRINK;
                    double diff = existingRadius + radius - distance;
                    if (diff > 1)
                    {
                        intersects = true;
                        center.x += (int)Math.Round(diff * dir.x);
                        center.y += (int)Math.Round(diff * dir.y);
                        break;
                    }
                }
            }

            Area area = new Area()
            {
                Set = set,
                NumRooms = Rand.NextNormalDist(set.MinRooms, set.MaxRooms),
                Center = center
            };
            Areas.Add(area);

            #region DEBUG
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                BigBoss.Debug.w(Logs.LevelGen, "Area center at " + area.Center + ", using set " + set + ", with " + area.NumRooms + " rooms.");
            }
            #endregion
        }

        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            MultiMap<GridType> tmp = new MultiMap<GridType>();
            foreach (Area area in Areas)
            {
                tmp.DrawCircle(area.Center.x, area.Center.y, (int)Math.Round(area.Set.AvgRadius / AREA_RADIUS_SHRINK), new StrokedAction<GridType>()
                {
                    UnitAction = Draw.SetTo(GridType.Floor),
                    StrokeAction = Draw.SetTo(GridType.Wall)
                });
                tmp[area.Center] = GridType.INTERNAL_MARKER_1;
            }
            tmp.ToLog(BigBoss.Debug.Get(Logs.LevelGen));
            BigBoss.Debug.printFooter(Logs.LevelGen, "Generate Areas");
        }
        #endregion
    }

    protected void GenerateRoomShells()
    {
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Generate Rooms");
        }
        #endregion
        List<ILayoutObject<GenSpace>> rooms = new List<ILayoutObject<GenSpace>>();
        int numRooms = Rand.Next(minRooms, maxRooms);
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.w(Logs.LevelGen, "Generating " + numRooms + " rooms.");
        }
        #endregion
        for (int i = 1; i <= numRooms; i++)
        {
            LayoutObject<GenSpace> room = new LayoutObject<GenSpace>("Room");
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
        foreach (LayoutObject<GenSpace> room in Objects)
        {
            #region DEBUG
            double time = 0;
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
            InitialTheme.ChooseAllSmartObjects(Rand);
            RoomSpec spec = new RoomSpec(room, Depth, InitialTheme, Rand);
            // Base Mod
            if (!ApplyMod(spec, spec.RoomModifiers.BaseMods))
            {
                throw new ArgumentException("Could not apply base mod");
            }
            // Definining Mod
            if (spec.RoomModifiers.AllowDefiningMod)
            {
                ApplyMod(spec, spec.RoomModifiers.DefiningMods);
            }
            // Flex Mods
            int numFlex = Rand.Next(spec.RoomModifiers.MinFlexMods, spec.RoomModifiers.MaxFlexMods);
            int numHeavy = (int)Math.Round((numFlex / 3d) + (numFlex / 3d * Rand.NextDouble()));
            int numFill = numFlex - numHeavy;
            // Heavy Mods
            for (int i = 0; i < numHeavy; i++)
            {
                if (!ApplyMod(spec, spec.RoomModifiers.HeavyMods))
                {
                    break;
                }
            }
            // Fill Mods
            for (int i = 0; i < numFill; i++)
            {
                if (!ApplyMod(spec, spec.RoomModifiers.FillMods))
                {
                    break;
                }
            }
            // Final Mods
            ApplyMod(spec, spec.RoomModifiers.FinalMods);
            #region DEBUG
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                room.ToLog(Logs.LevelGen);
                BigBoss.Debug.w(Logs.LevelGen, "Modding " + room + " took " + (Time.realtimeSinceStartup - time) + " seconds.");
                BigBoss.Debug.printFooter(Logs.LevelGen, "Modding " + room);
            }
            #endregion
            if (!ValidateRoom(room))
            {
                throw new ArgumentException(room + " is not valid.");
            }
        }
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGenMain))
        {
            foreach (LayoutObject<GenSpace> room in Objects)
                room.ToLog(Logs.LevelGenMain);
        }
        #endregion
    }

    protected bool ApplyMod<T>(RoomSpec spec, ProbabilityPool<T> mods)
        where T : RoomModifier
    {
        mods.BeginTaking();
        T mod;
        while (mods.Take(spec.Random, out mod))
        {
            #region DEBUG
            float stepTime = 0;
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
            Container2D<GenSpace> backupGrid = new MultiMap<GenSpace>(spec.Grids);
            if (mod.Modify(spec))
            {
                #region DEBUG
                if (BigBoss.Debug.logging(Logs.LevelGen))
                {
                    spec.Grids.ToLog(Logs.LevelGen, "Applying " + mod + " took " + (Time.realtimeSinceStartup - stepTime) + " seconds.");
                }
                #endregion
                mods.EndTaking();
                return true;
            }
            else
            {
                spec.Grids = backupGrid;
                #region DEBUG
                if (BigBoss.Debug.logging(Logs.LevelGen))
                {
                    spec.Grids.ToLog(Logs.LevelGen, "Couldn't apply mod.  Processing " + mod + " took " + (Time.realtimeSinceStartup - stepTime) + " seconds.");
                }
                #endregion
            }
        }
        mods.EndTaking();
        return false;
    }

    protected bool ValidateRoom(LayoutObject<GenSpace> room)
    {
        return room.Bounding.IsValid();
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
        List<LayoutObject<GenSpace>> ret = new List<LayoutObject<GenSpace>>();
        int numClusters = Rand.Next(maxRoomClusters - minRoomClusters) + minRoomClusters;
        // Num clusters cannot be more than half num rooms
        if (numClusters > Objects.Count / 2)
        {
            numClusters = Objects.Count / 2;
        }
        List<LayoutObjectContainer<GenSpace>> clusters = new List<LayoutObjectContainer<GenSpace>>();
        for (int i = 0; i < numClusters; i++)
        {
            clusters.Add(new LayoutObjectContainer<GenSpace>());
        }
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.w(Logs.LevelGen, "Number of clusters: " + numClusters);
        }
        #endregion
        // Add two rooms to each
        foreach (LayoutObjectContainer<GenSpace> cluster in clusters)
        {
            LayoutObject<GenSpace> obj1 = (LayoutObject<GenSpace>)Objects.Take();
            LayoutObject<GenSpace> obj2 = (LayoutObject<GenSpace>)Objects.Take();
            cluster.Objects.Add(obj1);
            ClusterAround(cluster, obj2);
            cluster.Objects.Add(obj2);
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
        foreach (LayoutObject<GenSpace> r in new List<ILayoutObject<GenSpace>>(Objects))
        {
            if (Rand.Percent(clusterProbability))
            {
                LayoutObjectContainer<GenSpace> cluster = clusters.Random(Rand);
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
        foreach (ILayoutObject<GenSpace> cluster in clusters)
        {
            Objects.Add(cluster);
        }
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            foreach (LayoutObjectContainer<GenSpace> cluster in clusters)
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

    protected ProbabilityPool<ClusterInfo> GenerateClusterOptions<T>(LayoutObjectContainer<T> cluster, LayoutObject<T> obj)
        where T : IGridSpace
    {
        #region Debug
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader("Generate Cluster Options");
        }
        #endregion
        obj.ShiftOutside(cluster, new Point(1, 0), null, false, false);
        obj.Shift(-1, 0); // Shift to overlapping slightly
        MultiMap<bool> visited = new MultiMap<bool>();
        visited[0, 0] = true;
        ProbabilityList<ClusterInfo> shiftOptions = new ProbabilityList<ClusterInfo>();
        Queue<Point> shiftQueue = new Queue<Point>();
        shiftQueue.Enqueue(new Point());
        Container2D<T> clusterGrid = cluster.GetGrid();
        Container2D<T> objGrid = obj.GetGrid();
        #region Debug
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            var tmp = new MultiMap<T>();
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
                var tmpMap = new MultiMap<T>();
                tmpMap.PutAll(clusterGrid);
                tmpMap.PutAll(objGrid, curShift);
                tmpMap.ToLog(Logs.LevelGen, "Analyzing at shift " + curShift);
            }
            #endregion
            // Test if pass
            List<Point> intersectPoints = new List<Point>();
            if (objGrid.DrawAll((arr, x, y) =>
                {
                    if (GridTypeEnum.EdgeType(arr[x, y].GetGridType()))
                    {
                        GridType clusterType = clusterGrid[x + curShift.x, y + curShift.y].GetGridType();
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
        #region Debug
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            shiftOptions.ToLog(BigBoss.Debug.Get(Logs.LevelGen), "Shift options");
            BigBoss.Debug.printFooter("Generate Cluster Options");
        }
        #endregion
        return shiftOptions;
    }

    protected void ClusterAround(LayoutObjectContainer<GenSpace> cluster, LayoutObject<GenSpace> obj)
    {
        #region Debug
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader("Cluster Around");
        }
        #endregion
        ProbabilityPool<ClusterInfo> shiftOptions = GenerateClusterOptions(cluster, obj);
        List<Point> clusterDoorOptions = new List<Point>();
        ClusterInfo info;
        Container2D<GenSpace> clusterGrid = cluster.GetGrid();
        var placed = new List<Value2D<GenSpace>>(0);
        while (shiftOptions.Take(Rand, out info))
        {
            clusterGrid.DrawPoints(info.Intersects, Draw.CanDrawDoor().IfThen(Draw.AddTo<GenSpace>(clusterDoorOptions)).Shift(info.Shift));
            #region Debug
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                BigBoss.Debug.w(Logs.LevelGen, "selected " + info.Shift);
                var tmpMap = new MultiMap<GenSpace>();
                clusterGrid.DrawAll(Draw.CopyTo(tmpMap));
                obj.GetGrid().DrawAll(Draw.CopyTo(tmpMap, info.Shift));
                tmpMap.DrawPoints(info.Intersects, Draw.SetTo(GridType.INTERNAL_RESERVED_CUR, InitialTheme).Shift(info.Shift));
                tmpMap.ToLog(Logs.LevelGen, "Intersect Points");
                tmpMap = new MultiMap<GenSpace>();
                clusterGrid.DrawAll(Draw.CopyTo(tmpMap));
                obj.GetGrid().DrawAll(Draw.CopyTo(tmpMap, info.Shift));
                tmpMap.DrawPoints(clusterDoorOptions, Draw.SetTo(GridType.Door, InitialTheme));
                tmpMap.ToLog(Logs.LevelGen, "Cluster door options");
            }
            #endregion
            if (clusterDoorOptions.Count > 0)
            { // Cluster side has door options
                obj.Shift(info.Shift.x, info.Shift.y);
                placed = InitialTheme.PlaceSomeDoors(obj, clusterDoorOptions, Rand);
                if (placed.Count != 0)
                { // Placed a door
                    foreach (Point p in placed)
                    {
                        LayoutObject<GenSpace> clusterObj;
                        cluster.GetObjAt(p, out clusterObj);
                        obj.Connect(clusterObj);
                    }
                    break;
                }
                else
                {
                    #region Debug
                    if (BigBoss.Debug.logging(Logs.LevelGen))
                    {
                        BigBoss.Debug.w(Logs.LevelGen, "selected point failed to match " + info.Shift + ". Backing up");
                    }
                    #endregion
                    obj.Shift(-info.Shift.x, -info.Shift.y);
                }
            }
        }
        if (placed.Count == 0)
        {
            throw new ArgumentException("Could not cluster rooms");
        }
        #region Debug
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            var tmpMap = new MultiMap<GenSpace>();
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
        List<ILayoutObject<GenSpace>> unplacedRooms = new List<ILayoutObject<GenSpace>>(Objects);
        List<ILayoutObject<GenSpace>> placedRooms = new List<ILayoutObject<GenSpace>>();
        if (unplacedRooms.Count > 0)
        { // Add seed
            ILayoutObject<GenSpace> seed = unplacedRooms.Take();
            Container.Objects.Add(seed);
            placedRooms.Add(seed);
        }
        foreach (ILayoutObject<GenSpace> room in unplacedRooms)
        {
            // Find room it will start from
            int roomNum = Rand.Next(placedRooms.Count);
            ILayoutObject<GenSpace> startRoom = placedRooms[roomNum];
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
            Container.Objects.Add(room);
            #region DEBUG
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                Container.ToLog(Logs.LevelGen, "Layout after placing room at: " + room.Bounding);
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
        foreach (LayoutObject<GenSpace> room in Objects)
        {
            #region DEBUG
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                BigBoss.Debug.printHeader(Logs.LevelGen, "Place Doors on " + room);
            }
            #endregion
            var potentialDoors = new MultiMap<GenSpace>();
            room.Grids.DrawPotentialExternalDoors(Draw.AddTo<GenSpace>(potentialDoors));
            int numDoors = Rand.Next(doorsMin, doorsMax);
            #region DEBUG
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                potentialDoors.ToLog(Logs.LevelGen, "Potential Doors");
                BigBoss.Debug.w(Logs.LevelGen, "Number of doors to generate: " + numDoors);
            }
            #endregion
            foreach (Value2D<GenSpace> doorSpace in potentialDoors.GetRandom(Rand, numDoors, minDoorSpacing))
            {
                room.Grids.SetTo(doorSpace, GridType.Door, InitialTheme);
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
        DrawAction<GenSpace> passTest = Draw.ContainedIn<GenSpace>(Path.PathTypes).Or(Draw.CanDrawDoor());
        var layoutCopy = Container.GetGrid().Array;
        List<LayoutObject<GenSpace>> rooms = new List<LayoutObject<GenSpace>>(Layout.Rooms.Cast<LayoutObject<GenSpace>>());
        var runningConnected = Container2D<GenSpace>.CreateArrayFromBounds(layoutCopy);
        // Create initial queue and visited
        var startingRoom = rooms.Take();
        startingRoom.GetConnectedGrid().DrawAll(Draw.AddTo(runningConnected));
        Container2D<bool> visited;
        Queue<Value2D<GenSpace>> queue;
        ConstructBFS(startingRoom, out queue, out visited);
        visited = visited.Array;
        LayoutObject<GenSpace> fail;
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
            Value2D<GenSpace> startPoint;
            Value2D<GenSpace> endPoint;
            LayoutObject<GenSpace> hit;
            if (!FindNextPathPoints(layoutCopy, runningConnected, out hit, passTest, queue, visited, out startPoint, out endPoint))
            {
                throw new ArgumentException("Cannot find path to fail room");
            }
            // Connect
            #region DEBUG
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                layoutCopy.SetTo(startPoint, GridType.INTERNAL_RESERVED_CUR, InitialTheme);
                layoutCopy.ToLog(Logs.LevelGen, "Largest after putting blocked");
                BigBoss.Debug.w(Logs.LevelGen, "Start Point:" + startPoint);
            }
            #endregion
            var hitConnected = hit.GetConnectedGrid();
            var stack = layoutCopy.DrawJumpTowardsSearch(
                startPoint.x,
                startPoint.y,
                3,
                5,
                Draw.IsType<GenSpace>(GridType.NULL).And(Draw.Inside<GenSpace>(layoutCopy.Bounding.Expand(5))),
                passTest.And(Draw.PointContainedIn(hitConnected)),
                Rand,
                endPoint,
                true);
            var path = new Path(stack);
            if (path.Valid)
            {
                #region DEBUG
                if (BigBoss.Debug.logging(Logs.LevelGen))
                {
                    path.Bake(null).ToLog(Logs.LevelGen, "Connecting Path");
                }
                #endregion
                path.Simplify();
                Point first = path.FirstEnd;
                Point second = path.SecondEnd;
                LayoutObject<GenSpace> leaf1, leaf2;
                LayoutObject<GenSpace> pathObj = path.Bake(InitialTheme);
                Container.ConnectTo(first, pathObj, first, out leaf1, out pathObj);
                Container.ConnectTo(second, pathObj, second, out leaf2, out pathObj);
                if (leaf1[first].Type == GridType.Wall)
                {
                    InitialTheme.PlaceDoor(leaf1, first.x, first.y, Rand);
                }
                if (leaf2[second].Type == GridType.Wall)
                {
                    InitialTheme.PlaceDoor(leaf2, second.x, second.y, Rand);
                }
                // Expand path
                foreach (var p in path)
                {
                    layoutCopy.DrawAround(p.x, p.y, false, Draw.IsType<GenSpace>(GridType.NULL).IfThen(Draw.SetTo(pathObj, GridType.Floor, InitialTheme).And(Draw.SetTo(GridType.Floor, InitialTheme))));
                    layoutCopy.DrawCorners(p.x, p.y, new DrawAction<GenSpace>((arr, x, y) =>
                    {
                        if (!arr.IsType(x, y, GridType.NULL)) return false;
                        return arr.Cornered(x, y, Draw.IsType<GenSpace>(GridType.Floor));
                    }).IfThen(Draw.SetTo(pathObj, GridType.Floor, InitialTheme)));
                }
                // Mark path on layout object
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
                Container.Objects.Add(pathObj);
                hitConnected.DrawAll(Draw.AddTo(runningConnected));
                #region DEBUG
                if (BigBoss.Debug.logging(Logs.LevelGen))
                {
                    Container.ToLog(Logs.LevelGen, "Final Connection");
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

    protected void ConstructBFS(LayoutObject<GenSpace> obj,
        out Queue<Value2D<GenSpace>> queue,
        out Container2D<bool> visited)
    {
        visited = new MultiMap<bool>();
        queue = new Queue<Value2D<GenSpace>>();
        obj.GetConnectedGrid().DrawPerimeter(Draw.Not(Draw.IsType<GenSpace>(GridType.NULL)), new StrokedAction<GenSpace>()
        {
            UnitAction = Draw.SetTo<GenSpace, bool>(visited, true),
            StrokeAction = Draw.AddTo(queue).And(Draw.SetTo<GenSpace, bool>(visited, true))
        }, false);
    }

    protected bool FindNextPathPoints(Container2D<GenSpace> map,
        Container2D<GenSpace> runningConnected,
        out LayoutObject<GenSpace> hit,
        DrawAction<GenSpace> pass,
        Queue<Value2D<GenSpace>> curQueue,
        Container2D<bool> curVisited,
        out Value2D<GenSpace> startPoint,
        out Value2D<GenSpace> endPoint)
    {
        if (!map.DrawBreadthFirstSearch(
            curQueue, curVisited, false,
            Draw.IsType<GenSpace>(GridType.NULL),
            pass,
            out endPoint))
        {
            hit = null;
            startPoint = null;
            return false;
        }
        if (!Container.GetObjAt(endPoint, out hit))
        {
            startPoint = null;
            return false;
        }
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.w(Logs.LevelGen, "Found unconnected object at " + endPoint);
        }
        #endregion
        Container2D<bool> hitVisited;
        Queue<Value2D<GenSpace>> hitQueue;
        ConstructBFS(hit, out hitQueue, out hitVisited);
        curQueue.Enqueue(hitQueue);
        curVisited.PutAll(hitVisited);
        return map.DrawBreadthFirstSearch(
            hitQueue, hitVisited, false,
            Draw.IsType<GenSpace>(GridType.NULL),
            pass.And(Draw.PointContainedIn(runningConnected)),
            out startPoint);
    }

    public static void ConfirmEdges(LayoutObject<GenSpace> obj)
    {
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Confirm Edges");
            obj.ToLog(Logs.LevelGen, "Pre Confirm Edges");
        }
        #endregion
        MultiMap<GenSpace> tmp = new MultiMap<GenSpace>();
        obj.DrawAll(Draw.Not(Draw.IsType<GenSpace>(GridType.NULL).Or(Draw.IsType<GenSpace>(GridType.Wall))).IfThen((arr, x, y) =>
        {
            GenSpace space;
            if (arr.TryGetValue(x, y, out space))
            {
                obj.DrawAround(x, y, true, Draw.IsType<GenSpace>(GridType.NULL).IfThen(Draw.SetTo(tmp, GridType.Wall, space.Theme)));
            }
            return true;
        }));
        obj.Grids.PutAll(tmp);
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            obj.ToLog(Logs.LevelGen, "Post Confirm Edges");
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

    protected bool PlaceMissingStair(bool up, Bounding otherStair, out Boxing placed)
    {
        Container2D<GenSpace> layout = Container.GetGrid();
        #region Debug
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Placing missing stair");
            BigBoss.Debug.w(Logs.LevelGen, "Up: " + up + ", other stair " + otherStair);
        }
        #endregion
        foreach (LayoutObject<GenSpace> obj in Container.Flatten().Randomize(Rand))
        {
            #region DEBUG
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                obj.ToLog("Trying in.");
            }
            #endregion
            if (otherStair != null)
            {
                if (otherStair.GetCenter().Distance(obj.Bounding.GetCenter()) < MinStairDist)
                {
                    #region DEBUG
                    if (BigBoss.Debug.logging(Logs.LevelGen))
                    {
                        BigBoss.Debug.w(Logs.LevelGen, "Skipping due to distance to other stair.");
                    }
                    #endregion
                    continue;
                }
            }
            StairElement stair = InitialTheme.Stair.SmartElement.Get(Rand) as StairElement;

            if (!stair.Place(layout, obj, InitialTheme, Rand, out placed))
            {
                #region DEBUG
                if (BigBoss.Debug.logging(Logs.LevelGen))
                {
                    BigBoss.Debug.w(Logs.LevelGen, "No options.");
                }
                #endregion
                continue;
            }
            obj.DrawRect(placed, Draw.SetTo(up ? GridType.StairUp : GridType.StairDown, InitialTheme).And(Draw.MergeIn(stair, InitialTheme)));
            #region Debug
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                obj.ToLog(Logs.LevelGen, "Placed stairs");
                Container.ToLog(Logs.LevelGen, "Layout");
                BigBoss.Debug.printFooter("Placing Missing Stair");
            }
            #endregion
            return true;
        }
        placed = null;
        #region Debug
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printFooter("Placing Missing Stair");
        }
        #endregion
        return false;
    }

    public static Point GenerateShiftMagnitude(int mag, System.Random rand)
    {
        Vector2 vect = rand.NextInUnitCircle() * mag;
        Point p = new Point(vect);
        while (p.isZero())
        {
            vect = UnityEngine.Random.insideUnitCircle * mag;
            p = new Point(vect);
        }
        return p;
    }
}
