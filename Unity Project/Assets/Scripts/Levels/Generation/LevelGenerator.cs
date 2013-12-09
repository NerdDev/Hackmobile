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

    Theme theme;
    int levelDepth;

    public LevelGenerator(Theme theme, int levelDepth)
    {
        this.theme = theme;
        this.levelDepth = levelDepth;
    }

    public LevelLayout GenerateLayout()
    {
        return GenerateLayout(Probability.LevelRand.Next());
    }

    public LevelLayout GenerateLayout(int seed)
    {
        if (seed == -1)
        {
            return GenerateLayout(Probability.Rand.Next());
        }
        #region DEBUG
        int debugNum = 1;
        float stepTime = 0, startTime = 0;
        if (BigBoss.Debug.logging(Logs.LevelGenMain))
        {
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                BigBoss.Debug.printHeader(Logs.LevelGenMain, "Generating Level: " + levelDepth);
                BigBoss.Debug.w(Logs.LevelGenMain, "Random seed int: " + seed);
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
        if (BigBoss.Debug.logging(Logs.LevelGenMain))
        {
            BigBoss.Debug.w(Logs.LevelGenMain, "Modding Rooms took: " + (Time.realtimeSinceStartup - stepTime));
            stepTime = Time.realtimeSinceStartup;
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                BigBoss.Debug.CreateNewLog(Logs.LevelGen, "Level Depth " + levelDepth + "/" + levelDepth + " " + debugNum++ + " - Cluster Rooms");
            }
        }
        #endregion
        // Not complete
        //ClusterRooms(rooms);
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGenMain))
        {
            BigBoss.Debug.w(Logs.LevelGenMain, "Cluster Rooms took: " + (Time.realtimeSinceStartup - stepTime));
            stepTime = Time.realtimeSinceStartup;
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                BigBoss.Debug.CreateNewLog(Logs.LevelGen, "Level Depth " + levelDepth + "/" + levelDepth + " " + debugNum++ + " - Place Rooms");
            }
        }
        #endregion
        PlaceRooms(rooms, layout);
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGenMain))
        {
            BigBoss.Debug.w(Logs.LevelGenMain, "Place Rooms took: " + (Time.realtimeSinceStartup - stepTime));
            stepTime = Time.realtimeSinceStartup;
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                BigBoss.Debug.CreateNewLog(Logs.LevelGen, "Level Depth " + levelDepth + "/" + levelDepth + " " + debugNum++ + " - Place Doors");
            }
        }
        #endregion
        PlaceDoors(layout);
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGenMain))
        {
            BigBoss.Debug.w(Logs.LevelGenMain, "Place Doors took: " + (Time.realtimeSinceStartup - stepTime));
            stepTime = Time.realtimeSinceStartup;
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                BigBoss.Debug.CreateNewLog(Logs.LevelGen, "Level Depth " + levelDepth + "/" + levelDepth + " " + debugNum++ + " - Place Paths");
            }
        }
        #endregion
        PlacePaths(layout);
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGenMain))
        {
            BigBoss.Debug.w(Logs.LevelGenMain, "Place paths took: " + (Time.realtimeSinceStartup - stepTime));
            stepTime = Time.realtimeSinceStartup;
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                BigBoss.Debug.CreateNewLog(Logs.LevelGen, "Level Depth " + levelDepth + "/" + levelDepth + " " + debugNum++ + " - Confirm Connections");
            }
        }
        #endregion
        ConfirmConnection(layout);
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGenMain))
        {
            BigBoss.Debug.w(Logs.LevelGenMain, "Confirm Connection took: " + (Time.realtimeSinceStartup - stepTime));
            stepTime = Time.realtimeSinceStartup;
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                BigBoss.Debug.CreateNewLog(Logs.LevelGen, "Level Depth " + levelDepth + "/" + levelDepth + " " + debugNum++ + " - Confirm Edges");
            }
        }
        #endregion
        ConfirmEdges(layout);
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGenMain))
        {
            BigBoss.Debug.w(Logs.LevelGenMain, "Confirm Edges took: " + (Time.realtimeSinceStartup - stepTime));
            stepTime = Time.realtimeSinceStartup;
        }
        #endregion

        #region DEBUG
        if (BigBoss.Debug.logging())
        {
            BigBoss.Debug.w(Logs.LevelGenMain, "Generate Level took: " + (Time.realtimeSinceStartup - startTime));
            layout.ToLog(Logs.LevelGenMain);
            BigBoss.Debug.printFooter(Logs.LevelGenMain);
        }
        #endregion
        return layout;
    }

    List<Room> GenerateRoomShells()
    {
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Generate Rooms");
        }
        #endregion
        List<Room> rooms = new List<Room>();
        int numRooms = Probability.LevelRand.Next(minRooms, maxRooms);
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.w(Logs.LevelGen, "Generating " + numRooms + " rooms.");
        }
        #endregion
        for (int i = 1; i <= numRooms; i++)
        {
            Room room = new Room();
            rooms.Add(room);
        }
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printFooter(Logs.LevelGen);
        }
        #endregion
        return rooms;
    }

    void ModRooms(IEnumerable<Room> rooms)
    {
        foreach (Room room in rooms)
        {
            #region DEBUG
            double stepTime = 0, time = 0;
            if (BigBoss.Debug.logging(Logs.LevelGenMain))
            {
                BigBoss.Debug.w(Logs.LevelGenMain, "Mods for " + room);
            }
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                BigBoss.Debug.CreateNewLog(Logs.LevelGen, "Level Depth " + levelDepth + "/" + levelDepth + " " + 0 + " - Generate Room " + room.Id);
                BigBoss.Debug.printHeader(Logs.LevelGen, "Modding " + room);
                time = Time.realtimeSinceStartup;
            }
            #endregion
            RoomSpec spec = new RoomSpec(room, levelDepth, theme, Probability.LevelRand);
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
            room.Bake(false);
            #region DEBUG
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                room.ToLog(Logs.LevelGen); 
                BigBoss.Debug.w(Logs.LevelGen, "Modding " + room + " took " + (Time.realtimeSinceStartup - time) + " seconds.");
                BigBoss.Debug.printFooter(Logs.LevelGen);
            }
            #endregion
        }
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printFooter(Logs.LevelGen);
        }
        #endregion
    }

    static List<RoomModifier> PickMods()
    {
        List<RoomModifier> mods = new List<RoomModifier>();
        RoomModifier baseMod = RoomModifier.GetBase();
        mods.Add(baseMod);
        int numFlex = Probability.LevelRand.Next(1, maxFlexMod);
        List<RoomModifier> flexMods = RoomModifier.GetFlexible(numFlex);
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
        if (chanceNoFinalMod < Probability.LevelRand.Next(100))
        {
            RoomModifier finalMod = RoomModifier.GetFinal();
            mods.Add(finalMod);
            #region DEBUG
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                BigBoss.Debug.w(Logs.LevelGen, "Picked Base Mod: " + finalMod);
            }
            #endregion
        }
        return mods;
    }

    static List<LayoutObject> ClusterRooms(List<Room> rooms)
    {
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Custer Rooms");
        }
        #endregion
        List<LayoutObject> ret = new List<LayoutObject>();
        int numClusters = Probability.LevelRand.Next(maxRoomClusters - minRoomClusters) + minRoomClusters;
        // Num clusters cannot be more than half num rooms
        if (numClusters > rooms.Count / 2)
            numClusters = rooms.Count / 2;
        List<LayoutCluster> clusters = new List<LayoutCluster>().Populate(numClusters);
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.w(Logs.LevelGen, "Number of clusters: " + numClusters);
        }
        #endregion
        // Add two rooms to each
        foreach (LayoutCluster cluster in clusters)
        {
            cluster.AddObject(rooms.Take());
            cluster.AddObject(rooms.Take());
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
            BigBoss.Debug.w(Logs.LevelGen, "Rooms left: " + rooms.Count);
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
            BigBoss.Debug.printFooter(Logs.LevelGen);
        }
        #endregion
        return ret;
    }

    static void PlaceRooms(List<Room> rooms, LevelLayout layout)
    {
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Place Rooms");
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
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                BigBoss.Debug.w(Logs.LevelGen, "Placing room: " + room);
                BigBoss.Debug.w(Logs.LevelGen, "Picked starting room number: " + roomNum);
                BigBoss.Debug.w(Logs.LevelGen, "Shift: " + shiftMagn);
            }
            #endregion
            // Keep moving until room doesn't overlap any other rooms
            LayoutObject intersect;
            while ((intersect = room.IntersectObjBounds(placedRooms, 1)) != null)
            {
                #region DEBUG
                if (BigBoss.Debug.logging(Logs.LevelGen))
                {
                    BigBoss.Debug.w(Logs.LevelGen, "This layout led to an overlap: " + room.GetBounding(true));
                    layout.ToLog(Logs.LevelGen);
                }
                #endregion
                room.ShiftOutside(intersect, shiftMagn, true, true);
            }
            placedRooms.Add(room);
            layout.AddRoom(room);
            layout.RemoveObject(seedRoom);
            #region DEBUG
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                BigBoss.Debug.w(Logs.LevelGen, "Layout after placing room at: " + room.GetBounding(true));
                layout.ToLog(Logs.LevelGen);
                BigBoss.Debug.printBreakers(Logs.LevelGen, 4);
            }
            #endregion
        }
        #region DEBUG
        BigBoss.Debug.printFooter(Logs.LevelGen);
        #endregion
    }

    static void PlaceDoors(LevelLayout layout)
    {
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Place Doors");
        }
        #endregion
        foreach (Room room in layout.GetRooms())
        {
            #region DEBUG
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                BigBoss.Debug.printHeader(Logs.LevelGen, "Place Doors on " + room);
            }
            #endregion
            var potentialDoors = new GridMap();
            room.Array.DrawPotentialExternalDoors(Draw.AddTo(potentialDoors));
            int numDoors = Probability.LevelRand.Next(doorsMin, doorsMax);
            #region DEBUG
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                potentialDoors.ToLog(Logs.LevelGen, "Potential Doors");
                BigBoss.Debug.w(Logs.LevelGen, "Number of doors to generate: " + numDoors);
            }
            #endregion
            foreach (Value2D<GridType> doorSpace in potentialDoors.Random(Probability.LevelRand, numDoors, minDoorSpacing))
            {
                room.put(GridType.Door, doorSpace.x, doorSpace.y);
                #region DEBUG
                if (BigBoss.Debug.logging(Logs.LevelGen))
                {
                    room.ToLog(Logs.LevelGen, "Generated door at: " + doorSpace);
                    potentialDoors.ToLog(Logs.LevelGen, "Remaining options");
                }
                #endregion
            }
            #region DEBUG
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                room.ToLog(Logs.LevelGen, "Final Room After placing doors");
                BigBoss.Debug.printFooter(Logs.LevelGen);
            }
            #endregion
        }
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            layout.ToLog(Logs.LevelGen);
            BigBoss.Debug.printFooter(Logs.LevelGen);
        }
        #endregion
    }

    static void PlacePaths(LevelLayout layout)
    {
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Place Paths");
            layout.ToLog(Logs.LevelGen, "Pre Path Layout");
        }
        #endregion
        var grids = layout.GetArray(layoutMargin);
        GridMap doors = layout.getTypes(grids, GridType.Door);
        GridType[,] arr = grids.GetArr();
        #region DEBUG
        GridArray debugArr = null;
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            debugArr = new GridArray(grids);
        }
        #endregion
        foreach (var door in doors)
        {
            // Block nearby floors
            arr.DrawAround(door.x, door.y, false, (arr2, x, y) =>
                {
                    if (arr2[y, x] == GridType.Floor)
                        arr2[y, x] = GridType.INTERNAL_RESERVED_BLOCKED;
                    return true;
                });
        }
        foreach (var door in doors)
        {

            var path = new Path(door, grids);
            #region DEBUG
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                GridArray messyPathArr = new GridArray(debugArr);
                messyPathArr.PutAll(path.GetArray());
                messyPathArr.ToLog(Logs.LevelGen, "Map after placing for door: " + door);
            }
            #endregion
            if (path.isValid())
            {
                path.Simplify();
                path.ConnectEnds(layout);
                #region DEBUG
                if (BigBoss.Debug.logging(Logs.LevelGen))
                {
                    debugArr.PutAll(path);
                }
                #endregion
                path.Bake(true);
                grids.PutAll(path);
                layout.AddPath(path);
            }
            #region DEBUG
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                debugArr.ToLog(Logs.LevelGen, "Map after simplifying path for door: " + door);
            }
            #endregion
        }
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            layout.ToLog(Logs.LevelGen, "Final Layout");
            BigBoss.Debug.printFooter(Logs.LevelGen);
        }
        #endregion
    }

    private static void ConfirmConnection(LevelLayout layout)
    {
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Confirm Connections");
        }
        #endregion
        var roomsToConnect = new List<LayoutObject>(layout.GetRooms().Cast<LayoutObject>());
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.w(Logs.LevelGen, "Connecting List:");
            foreach (var layoutobj in layout.GetRooms())
            {
                BigBoss.Debug.w(Logs.LevelGen, 1, layoutobj.ToString());
            }
        }
        #endregion
        foreach (var layoutObj in layout.GetRooms())
        {
            #region DEBUG
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                layoutObj.ToLog(Logs.LevelGen, "Connecting");
            }
            #endregion
            roomsToConnect.Remove(layoutObj);
            LayoutObject fail;
            if (!layoutObj.ConnectedTo(roomsToConnect, out fail))
            {
                #region DEBUG
                if (BigBoss.Debug.logging(Logs.LevelGen))
                {
                    fail.ToLog(Logs.LevelGen, layoutObj + " failed to connect to:");
                }
                #endregion
                MakeConnection(layout, layoutObj, fail);
            }
            #region DEBUG
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                BigBoss.Debug.printFooter(Logs.LevelGen);
            }
            #endregion
        }
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printFooter(Logs.LevelGen);
        }
        #endregion
    }

    private static void ConfirmEdges(LevelLayout layout)
    {
        layout.ShiftAll(1, 1);
        GridArray ga = layout.GetArray(1);
        GridType[,] arr = ga.GetArr();
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Confirm Edges");
            layout.ToLog(Logs.LevelGen, "Pre Confirm Edges");
        }
        #endregion
        LayoutObjectLeaf leaf = new LayoutObjectLeaf(ga.getWidth(), ga.getHeight());
        layout.AddObject(leaf);
        foreach (Value2D<GridType> val in ga)
        {
            if (val.val == GridType.Floor)
            {
                arr.DrawAround(val.x, val.y, true, (arr2, x, y) =>
                    {
                        if (arr2[y, x] == GridType.NULL)
                            leaf.put(GridType.Wall, x, y);
                        return true;
                    });
            }
        }
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            layout.ToLog(Logs.LevelGen, "Post Confirm Edges");
            BigBoss.Debug.printFooter(Logs.LevelGen);
        }
        #endregion
    }

    private static void MakeConnection(LevelLayout layout, LayoutObject obj1, LayoutObject obj2)
    {
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Make Connection - " + obj1 + " AND " + obj2);
        }
        #endregion
        GridArray smallest;
        GridArray largest;
        GridArray layoutArr = layout.GetArray();
        layoutArr.PutAs(layoutArr, GridType.INTERNAL_RESERVED_BLOCKED);
        Container2D<GridType>.Smallest(obj1.GetConnectedGrid(), obj2.GetConnectedGrid(), out smallest, out largest);
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            smallest.ToLog(Logs.LevelGen, "Smallest");
            largest.ToLog(Logs.LevelGen, "Largest");
        }
        #endregion
        var startPtStack = smallest.GetArr().DrawDepthFirstSearch(
            1, 1,
            Draw.EqualTo(GridType.NULL),
            Draw.ContainedIn(Path.PathTypes()),
            Probability.LevelRand,
            true);
        if (startPtStack.Count > 0)
        {
            layoutArr.PutAll(largest);
            Value2D<GridType> startPoint = startPtStack.Pop();
            #region DEBUG
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                layoutArr[startPoint.x, startPoint.y] = GridType.INTERNAL_RESERVED_CUR;
                layoutArr.ToLog(Logs.LevelGen, "Largest after putting blocked");
                BigBoss.Debug.w(Logs.LevelGen, "Start Point:" + startPoint);
            }
            #endregion
            var path = new Path(startPoint, layoutArr);
            if (path.isValid())
            {
                #region DEBUG
                if (BigBoss.Debug.logging(Logs.LevelGen))
                {
                    largest.PutAll(path);
                    largest.ToLog(Logs.LevelGen, "Connecting Path");
                }
                #endregion
                path.Finalize(layout);
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
            BigBoss.Debug.printFooter(Logs.LevelGen);
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
