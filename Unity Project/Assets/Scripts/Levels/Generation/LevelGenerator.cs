using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using LevelGen;

public class LevelGenerator
{
    #region GlobalGenVariables
    // Stairs
    public const float MinStairDist = 30;

    // Doors
    public const int desiredWallToDoorRatio = 10;

    private const double areaRadiusShrink = 5;
    private const double areaSquishFactor = 2;
    #endregion

    public Theme DebugTheme;
    public ProbabilityPool<ThemeSet> ThemeSetOptions;
    public System.Random Rand;
    public int Depth;
    public LevelLayout Layout = new LevelLayout();
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
        Log("Confirm Connection", true, ConfirmConnection);
        //Log("Place Stairs", true, PlaceStairs);
        #region DEBUG
        if (BigBoss.Debug.logging())
        {
            BigBoss.Debug.w(Logs.LevelGenMain, "Generate Level took: " + (Time.realtimeSinceStartup - startTime));
            Layout.ToLog(Logs.LevelGenMain);
            // Print areas
            MultiMap<char> tmp = new MultiMap<char>();
            char c = (char)33;
            foreach (Area area in Areas)
            {
                area.DrawAll(Draw.SetTo<GenSpace, char>(tmp, c++));
            }
            tmp.ToLog(Logs.LevelGenMain, "Area Coverage");
            // Print themes
            tmp = new MultiMap<char>();
            c = (char)33;
            Dictionary<Theme, char> themeSet = new Dictionary<Theme, char>();
            foreach (var v in Layout)
            {
                char c2;
                if (!themeSet.TryGetValue(v.val.Theme, out c2))
                {
                    c2 = c++;
                    themeSet[v.val.Theme] = c2;
                }
                tmp[v] = c2;
            }
            foreach (var pair in themeSet)
            {
                BigBoss.Debug.w(Logs.LevelGenMain, pair.Key.ToString() + ": " + pair.Value);
            }
            tmp.ToLog(Logs.LevelGenMain, "Theme Coverage");
            BigBoss.Debug.printFooter(Logs.LevelGenMain, "Generating Level: " + Depth);
        }
        #endregion
        Layout.Random = Rand;
        return Layout;
    }

    protected void NewLog(string name)
    {
        BigBoss.Debug.CreateNewLog(Logs.LevelGen, "Level Depth " + Depth + "/" + Depth + " " + ++_debugNum + " - " + name);
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
            {
                NewLog(name);
            }
            BigBoss.Debug.w(Logs.LevelGen, "Start time: " + time);
        }
        foreach (Action action in a)
        {
            action();
        }
        if (BigBoss.Debug.logging(Logs.LevelGenMain))
        {
            BigBoss.Debug.w(Logs.LevelGen, "End time: " + Time.realtimeSinceStartup + ", Total time: " + (Time.realtimeSinceStartup - time));
            BigBoss.Debug.w(Logs.LevelGenMain, name + " took: " + (Time.realtimeSinceStartup - time));
        }
    }

    protected void GenerateAreas()
    {
        int numAreas = Rand.NextNormalDist(BigBoss.Levels.MinAreas, BigBoss.Levels.MaxAreas);
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Generate Areas");
            BigBoss.Debug.w(Logs.LevelGen, "Number of areas: " + numAreas);
        }
        #endregion

        GridTypeObj floor = new GridTypeObj() { Type = GridType.Floor };
        GridTypeObj wall = new GridTypeObj() { Type = GridType.Wall };

        LayoutObject<GridTypeObj> areaCont = new LayoutObject<GridTypeObj>(LayoutObjectType.Layout);

        for (int i = 0; i < numAreas; i++)
        {
            ThemeSet set = ThemeSetOptions.Get(Rand);

            LayoutObject<GridTypeObj> areaObj = new LayoutObject<GridTypeObj>(LayoutObjectType.Room);
            areaObj.DrawCircle(0, 0, (int)Math.Round(set.AvgRadius / areaRadiusShrink), new StrokedAction<GridTypeObj>()
            {
                UnitAction = Draw.SetTo(floor),
                StrokeAction = Draw.SetTo(wall)
            });

            ProbabilityPool<ClusterInfo> shiftOptions;
            IClusteringThemeExt.GenerateClusterOptions<GridTypeObj>(areaCont, areaObj, false, out shiftOptions);
            ClusterInfo info;
            Point shift;
            if (shiftOptions.Get(Rand, out info))
            {
                areaObj.Shift(info.Shift);
                shift = info.Shift;
                shift.x = (int)(shift.x * areaRadiusShrink / areaSquishFactor);
                shift.y = (int)(shift.y * areaRadiusShrink / areaSquishFactor);
            }
            else
            {
                shift = new Point();
            }

            Area area = new Area(i)
            {
                Set = set,
                NumRooms = Rand.NextNormalDist(set.MinRooms, set.MaxRooms),
                CenterPt = shift
            };
            Areas.Add(area);
            Layout.AddChild(area);
            areaCont.AddChild(areaObj);

            #region DEBUG
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                BigBoss.Debug.w(Logs.LevelGen, "Area center at " + area.CenterPt + ", using set " + set + ", with " + area.NumRooms + " rooms.");
                areaCont.ToLog(Logs.LevelGen);
            }
            #endregion
        }

        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            MultiMap<GridTypeObj> tmp = new MultiMap<GridTypeObj>();
            foreach (Area a in Areas)
            {
                tmp[a.CenterPt] = new GridTypeObj() { Type = GridType.INTERNAL_MARKER_1 };
            }
            tmp.ToLog(Logs.LevelGen, "Area Centers Expanded");
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
        LayoutObject<GenSpace> room = new LayoutObject<GenSpace>(LayoutObjectType.Room);
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            NewLog(a + " " + room);
        }
        #endregion
        Theme t = GetTheme(a);
        t.GenerateRoom(this, a, room);
        a.NumRoomsGenerated++;
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            Layout.PrintChildrenTree(BigBoss.Debug.Get(Logs.LevelGen));
            room.ToLog(Logs.LevelGen, "Room generated");
            room.GetConnectedGrid().ToLog("Connected rooms");
            Layout.ToLog(Logs.LevelGen, "Layout after generating Room " + a.NumRoomsGenerated);
        }
        #endregion
    }

    protected Theme GetTheme(Area a)
    {
        Theme t = a.Set.GetTheme(Rand);
        Theme picked;
        if (!a.PickedPrototypes.TryGetValue(t, out picked))
        {
            picked = t;
            int numThemeMods = Rand.NextNormalDist(picked.MinThemeMods, picked.MaxThemeMods);
            for (int i = 0; i < numThemeMods; i++)
            {
                ThemeMod mod;
                if (!picked.ThemeMods.Get(Rand, out mod))
                {
                    break;
                }
                mod.ModTheme(picked);
            }
            a.PickedPrototypes[picked] = picked;
        }
        return picked;
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
        DrawAction<GenSpace> passTest = Draw.And(Draw.ContainedIn<GenSpace>(Path.PathTypes).Or(Draw.CanDrawDoor(true)), Draw.HasAround(true, Draw.IsType<GenSpace>(GridType.NULL)));
        List<LayoutObject<GenSpace>> rooms = new List<LayoutObject<GenSpace>>(Layout.Flatten(LayoutObjectType.Room));
        LayoutObject<GenSpace> runningConnected = new LayoutObject<GenSpace>(LayoutObjectType.Layout);
        // Create initial queue and visited
        var startingRoom = rooms.Take();
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            startingRoom.ToLog(Logs.LevelGen, "Starting room");
        }
        #endregion
        foreach (var connected in startingRoom.ConnectedToAll())
        {
            runningConnected.AddChild(connected);
        }
        Container2D<bool> visited;
        Queue<Value2D<GenSpace>> queue;
        ConstructBFS(startingRoom, out queue, out visited);
        visited = visited.Array;
        LayoutObject<GenSpace> fail;
        int count = 1000;
        while (!startingRoom.ConnectedTo(rooms, out fail))
        {
            if (0 == count--)
            {
                throw new ArgumentException("Infinite loop connecting rooms.");
            }

            #region DEBUG
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                runningConnected.ToLogNumberRooms(BigBoss.Debug.Get(Logs.LevelGen), "Source Setup");
                fail.ToLogNumberRooms(BigBoss.Debug.Get(Logs.LevelGen), "Failed to connect to this");
            }
            #endregion

            LayoutObject<GenSpace> hit;
            if (ConnectViaDoor(runningConnected, fail))
            {
                hit = fail;
                Queue<Value2D<GenSpace>> hitQueue;
                Container2D<bool> hitVisited;
                ConstructBFS(fail, out hitQueue, out hitVisited);
                queue.Enqueue(hitQueue);
                visited.PutAll(hitVisited);
            }
            else
            {
                // Draw Path
                // Find start points
                Value2D<GenSpace> startPoint;
                Value2D<GenSpace> endPoint;
                if (!FindNextPathPoints(Layout, runningConnected, out hit, passTest, queue, visited, out startPoint, out endPoint))
                {
                    throw new ArgumentException("Cannot find path to fail room");
                }
                Theme pathTheme = Rand.NextBool() ? startPoint.val.Theme : endPoint.val.Theme;
                // Connect
                #region DEBUG
                if (BigBoss.Debug.logging(Logs.LevelGen))
                {
                    var tmp = new MultiMap<GenSpace>();
                    tmp.PutAll(Layout);
                    tmp.SetTo(startPoint, GridType.INTERNAL_RESERVED_CUR, DebugTheme);
                    tmp.ToLog(Logs.LevelGen, "Largest after putting blocked");
                    BigBoss.Debug.w(Logs.LevelGen, "Start Point:" + startPoint);
                }
                #endregion
                var hitConnected = hit.GetConnectedGrid();
                var stack = Layout.DrawJumpTowardsSearch(
                    startPoint.x,
                    startPoint.y,
                    3,
                    5,
                    Draw.IsType<GenSpace>(GridType.NULL).And(Draw.Inside<GenSpace>(Layout.Bounding.Expand(5))),
                    passTest.And(Draw.PointContainedIn(hitConnected)),
                    Rand,
                    endPoint,
                    true);
                var path = new Path(stack);
                if (path.Valid)
                {
                    path.Simplify();
                    Point first = path.FirstEnd;
                    Point second = path.SecondEnd;
                    LayoutObject<GenSpace> pathObj = path.Bake(pathTheme);
                    #region DEBUG
                    if (BigBoss.Debug.logging(Logs.LevelGen))
                    {
                        pathObj.ToLog(Logs.LevelGen, "Connecting Path");
                    }
                    #endregion
                    if (pathObj.ConnectToChildrenAt(Layout, first.x, first.y).Count() == 0
                        || pathObj.ConnectToChildrenAt(Layout, second.x, second.y).Count() == 0)
                    {
                        throw new ArgumentException("Cannot connect at path ends.");
                    }
                    GenSpace space;
                    if (!Layout.TryGetValue(first, out space) || space.Type == GridType.Wall)
                    {
                        Layout.PlaceDoor(first.x, first.y, Rand, true);
                    }
                    if (!Layout.TryGetValue(second, out space) || space.Type == GridType.Wall)
                    {
                        Layout.PlaceDoor(second.x, second.y, Rand, true);
                    }
                    // Expand path
                    foreach (var p in path)
                    {
                        Layout.DrawAround(p.x, p.y, false, Draw.IsType<GenSpace>(GridType.NULL).IfThen(Draw.SetTo(pathObj, GridType.Floor, pathTheme).And(Draw.SetTo(GridType.Floor, pathTheme))));
                        Layout.DrawCorners(p.x, p.y, new DrawAction<GenSpace>((arr, x, y) =>
                        {
                            if (!arr.IsType(x, y, GridType.NULL)) return false;
                            return arr.Cornered(x, y, Draw.IsType<GenSpace>(GridType.Floor));
                        }).IfThen(Draw.SetTo(pathObj, GridType.Floor, pathTheme)));
                    }
                    // Mark path on layout object
                    foreach (var v in pathObj)
                    {
                        runningConnected.AddChild(pathObj);
                        if (!visited[v])
                        {
                            queue.Enqueue(v);
                        }
                        visited[v] = true;
                    }
                    Layout.PutAll(pathObj);
                }
                else
                {
                    throw new ArgumentException("Cannot create path to hit room");
                }
            }

            foreach (var child in hit.ConnectedToAll())
            {
                runningConnected.AddChild(child);
            }
            #region DEBUG
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                Layout.ToLog(Logs.LevelGen, "Final Connection");
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

    protected bool ConnectViaDoor(LayoutObject<GenSpace> runningConnected, LayoutObject<GenSpace> fail)
    {
        var failConnectedGrid = fail.GetConnectedGrid();
        List<Value2D<GenSpace>> intersects = runningConnected.IntersectPoints(failConnectedGrid);
        if (intersects.Count != 0)
        { // Try just making a door
            #region DEBUG
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                MultiMap<GenSpace> tmp = new MultiMap<GenSpace>();
                tmp.PutAll(runningConnected);
                tmp.PutAll(failConnectedGrid);
                tmp.DrawPoints(intersects.Cast<Point>(), Draw.SetTo(GridType.INTERNAL_RESERVED_CUR, DebugTheme));
                tmp.ToLog(Logs.LevelGen, "Intersects");
            }
            #endregion
            var placedDoors = fail.PlaceSomeDoors(Layout, intersects.Cast<Point>(), Rand, false);
            if (placedDoors.Count > 0)
            {
                foreach (var door in placedDoors)
                {
                    foreach (var room in fail.ConnectToChildrenAt(Layout, door.x, door.y))
                    {
                        GenSpace otherSpace;
                        if (room.TryGetValue(door, out otherSpace))
                        {
                            otherSpace.Type = GridType.Door;
                        }
                    }
                }
                #region DEBUG
                if (BigBoss.Debug.logging(Logs.LevelGen))
                {
                    fail.ToLog(Logs.LevelGen, "With placed doors");
                }
                #endregion
                return true;
            }
        }
        return false;
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
        if (!Layout.GetObjAt(endPoint.x, endPoint.y, out hit))
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
        obj.PutAll(tmp);
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            obj.ToLog(Logs.LevelGen, "Post Confirm Edges");
            BigBoss.Debug.printFooter(Logs.LevelGen, "Confirm Edges");
        }
        #endregion
    }
    #endregion

    //protected bool PlaceMissingStair(bool up, Bounding otherStair, out Boxing placed)
    //{
    //    Container2D<GenSpace> layout = Layout;
    //    #region Debug
    //    if (BigBoss.Debug.logging(Logs.LevelGen))
    //    {
    //        BigBoss.Debug.printHeader(Logs.LevelGen, "Placing missing stair");
    //        BigBoss.Debug.w(Logs.LevelGen, "Up: " + up + ", other stair " + otherStair);
    //    }
    //    #endregion
    //    foreach (LayoutObject<GenSpace> obj in Layout.RoomContainer.Objects.Randomize(Rand))
    //    {
    //        #region DEBUG
    //        if (BigBoss.Debug.logging(Logs.LevelGen))
    //        {
    //            obj.ToLog("Trying in.");
    //        }
    //        #endregion
    //        if (otherStair != null)
    //        {
    //            if (otherStair.GetCenter().Distance(obj.Bounding.GetCenter()) < MinStairDist)
    //            {
    //                #region DEBUG
    //                if (BigBoss.Debug.logging(Logs.LevelGen))
    //                {
    //                    BigBoss.Debug.w(Logs.LevelGen, "Skipping due to distance to other stair.");
    //                }
    //                #endregion
    //                continue;
    //            }
    //        }
    //        StairElement stair = InitialTheme.Stair.SmartElement.Get(Rand) as StairElement;

    //        if (!stair.Place(layout, obj, InitialTheme, Rand, out placed))
    //        {
    //            #region DEBUG
    //            if (BigBoss.Debug.logging(Logs.LevelGen))
    //            {
    //                BigBoss.Debug.w(Logs.LevelGen, "No options.");
    //            }
    //            #endregion
    //            continue;
    //        }
    //        obj.DrawRect(placed, Draw.SetTo(up ? GridType.StairUp : GridType.StairDown, InitialTheme).And(Draw.MergeIn(stair, InitialTheme)));
    //        #region Debug
    //        if (BigBoss.Debug.logging(Logs.LevelGen))
    //        {
    //            obj.ToLog(Logs.LevelGen, "Placed stairs");
    //            Layout.ToLog(Logs.LevelGen, "Layout");
    //            BigBoss.Debug.printFooter("Placing Missing Stair");
    //        }
    //        #endregion
    //        return true;
    //    }
    //    placed = null;
    //    #region Debug
    //    if (BigBoss.Debug.logging(Logs.LevelGen))
    //    {
    //        BigBoss.Debug.printFooter("Placing Missing Stair");
    //    }
    //    #endregion
    //    return false;
    //}

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
