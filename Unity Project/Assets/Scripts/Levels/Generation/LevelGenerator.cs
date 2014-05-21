using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using LevelGen;

public class LevelGenerator
{

    #region GlobalGenVariables
    // Number of areas
    public const int minAreas = 1;
    public const int maxAreas = 2;

    // Number of Rooms
    public static int minRooms { get { return 8; } }
    public static int maxRooms { get { return 16; } } //Max not inclusive

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

    private const double areaRadiusShrink = 5;
    #endregion

    public Theme InitialTheme;
    public ProbabilityPool<ThemeSet> ThemeSetOptions;
    public System.Random Rand;
    public int Depth;
    protected LevelLayout Layout = new LevelLayout();
    protected List<Area> Areas = new List<Area>();
    private int _debugNum = 0;

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
        Log("Areas", true, GenerateAreas);
        GenerateComponents();
        //Log("Confirm Connection", true, ConfirmConnection);
        //Log("Place Stairs", true, PlaceStairs);
        #region DEBUG
        if (BigBoss.Debug.logging())
        {
            BigBoss.Debug.w(Logs.LevelGenMain, "Generate Level took: " + (Time.realtimeSinceStartup - startTime));
            Layout.ToLog(Logs.LevelGenMain);
            BigBoss.Debug.printFooter(Logs.LevelGenMain, "Generating Level: " + Depth);
        }
        #endregion
        Layout.Random = Rand;
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
        int numAreas = Rand.NextNormalDist(minAreas, maxAreas);
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Generate Areas");
            BigBoss.Debug.w(Logs.LevelGen, "Number of areas: " + numAreas);
        }
        #endregion

        LayoutObjectContainer<GridTypeObj> areaCont = new LayoutObjectContainer<GridTypeObj>();
        GridTypeObj floor = new GridTypeObj() { Type = GridType.Floor };
        GridTypeObj wall = new GridTypeObj() { Type = GridType.Wall };

        for (int i = 0; i < numAreas; i++)
        {
            ThemeSet set = ThemeSetOptions.Get(Rand);

            LayoutObject<GridTypeObj> areaObj = new LayoutObject<GridTypeObj>("Area " + i);
            areaObj.DrawCircle(0, 0, (int)Math.Round(set.AvgRadius / areaRadiusShrink), new StrokedAction<GridTypeObj>()
            {
                UnitAction = Draw.SetTo(floor),
                StrokeAction = Draw.SetTo(wall)
            });

            ProbabilityPool<ClusterInfo> shiftOptions = IClusteringThemeExt.GenerateClusterOptions<GridTypeObj>(areaCont.GetGrid(), areaCont, areaObj, false);
            ClusterInfo info;
            if (shiftOptions.Get(Rand, out info))
            {
                areaObj.Shift(info.Shift);
            }

            Area area = new Area()
            {
                Set = set,
                NumRooms = Rand.NextNormalDist(set.MinRooms, set.MaxRooms),
                Center = areaObj.ShiftP
            };
            Areas.Add(area);
            areaCont.Objects.Add(areaObj);

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
                tmp.DrawCircle(area.Center.x, area.Center.y, (int)Math.Round(area.Set.AvgRadius / areaRadiusShrink), new StrokedAction<GridType>()
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

    protected void GenerateComponents()
    {
        int maxRooms = Areas.Max(a => a.NumRooms);
        var areasTmp = new List<Area>(Areas.Randomize(Rand));
        for (int i = 1; i <= maxRooms; i++)
        {
            foreach (Area a in areasTmp)
            {
                if (a.NumRooms == 0) continue;
                int div = maxRooms / a.NumRooms;
                if (i % div == div - 1)
                {
                    GenerateArea(a);
                }
            }
        }
    }

    protected void GenerateArea(Area a)
    {
        Theme t = a.Set.GetTheme(Rand);
        LayoutObject<GenSpace> room;
        t.GenerateRoom(this, a, out room);
        Layout.RoomContainer.Objects.Add(room);
        Layout.PutAll(room);
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
        var layoutCopy = Layout.GetGrid().Array;
        List<LayoutObject<GenSpace>> rooms = new List<LayoutObject<GenSpace>>(Layout.RoomContainer.Objects.Cast<LayoutObject<GenSpace>>());
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
                Layout.AllContainer.ConnectTo(first, pathObj, first, out leaf1, out pathObj);
                Layout.AllContainer.ConnectTo(second, pathObj, second, out leaf2, out pathObj);
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
                Layout.PutAll(pathObj);
                hitConnected.DrawAll(Draw.AddTo(runningConnected));
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
        if (!Layout.AllContainer.GetObjAt(endPoint.x, endPoint.y, out hit))
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

    protected bool PlaceMissingStair(bool up, Bounding otherStair, out Boxing placed)
    {
        Container2D<GenSpace> layout = Layout;
        #region Debug
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Placing missing stair");
            BigBoss.Debug.w(Logs.LevelGen, "Up: " + up + ", other stair " + otherStair);
        }
        #endregion
        foreach (LayoutObject<GenSpace> obj in Layout.RoomContainer.Objects.Randomize(Rand))
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
                Layout.ToLog(Logs.LevelGen, "Layout");
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
