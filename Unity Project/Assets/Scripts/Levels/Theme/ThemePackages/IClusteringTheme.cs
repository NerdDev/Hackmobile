using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface IClusteringTheme
{
    double ClusterSplitPercentProperty { get; }
}

public class ClusterInfo
{
    public Point Shift;
    public List<Point> Intersects;
    public override string ToString()
    {
        return Shift.ToString();
    }
}

public static class IClusteringThemeExt
{
    // Amount to shift rooms
    public const int SHIFT_RANGE = 10; //Max not inclusive

    public static bool GenerateClusterOptions<T>(
        LayoutObject<T> cluster,
        LayoutObject<T> obj,
        bool clusterByVolume,
        out ProbabilityPool<ClusterInfo> shiftOptions)
        where T : IGridType
    {
        return GenerateClusterOptions(cluster, cluster, obj, clusterByVolume, out shiftOptions);
    }

    public static bool GenerateClusterOptions<T>(
        LayoutObject<T> entireContainer,
        LayoutObject<T> cluster,
        LayoutObject<T> obj,
        bool clusterByVolume,
        out ProbabilityPool<ClusterInfo> shiftOptions)
        where T : IGridType
    {
        #region Debug
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader("Generate Cluster Options");
        }
        #endregion
        shiftOptions = new ProbabilityList<ClusterInfo>();
        List<LayoutObject<T>> items = new List<LayoutObject<T>>(cluster.Flatten(LayoutObjectType.Room));
        if (items.Count != 0)
        {
            Point shift = obj.GetCenterShiftOn(items[0]);
            shift = obj.GetShiftOutside(cluster, new Point(1, 0), shift, false);
            shift.x -= 1; // Shift to overlapping slightly
            MultiMap<bool> visited = new MultiMap<bool>();
            visited[0, 0] = true;
            Queue<Point> shiftQueue = new Queue<Point>();
            shiftQueue.Enqueue(shift);
            #region Debug
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                var tmp = new MultiMap<T>();
                tmp.PutAll(cluster);
                tmp.PutAll(obj);
                tmp.ToLog(Logs.LevelGen, "Starting cluster grid");
                tmp = new MultiMap<T>();
                tmp.PutAll(entireContainer);
                tmp.PutAll(obj, shift);
                tmp.ToLog(Logs.LevelGen, "Starting entire grid");
            }
            #endregion
            while (shiftQueue.Count > 0)
            {
                Point curShift = shiftQueue.Dequeue();
                #region Debug
                if (BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps) && BigBoss.Debug.logging(Logs.LevelGen))
                {
                    var tmpMap = new MultiMap<T>();
                    tmpMap.PutAll(entireContainer);
                    tmpMap.PutAll(obj, curShift);
                    tmpMap.ToLog(Logs.LevelGen, "Analyzing at shift " + curShift);
                }
                #endregion
                // Test if pass
                List<Point> intersectPoints = new List<Point>();
                bool overlappingSomething = false;
                bool intersectedCluster = false;
                HashSet<LayoutObject<T>> intersectingObjs = null;
                if (obj.DrawAll((arr, x, y) =>
                { // Draw on moving object to detect intersection pts
                    if (GridTypeEnum.EdgeType(arr[x, y].GetGridType()))
                    {
                        bool intersectsCluster;
                        T space;
                        if (!cluster.TryGetValue(x + curShift.x, y + curShift.y, out space) || space.Type == GridType.NULL)
                        {
                            if (!entireContainer.TryGetValue(x + curShift.x, y + curShift.y, out space) || space.Type == GridType.NULL)
                            {
                                // Not intersecting anything, move on.
                                return true;
                            }
                            intersectsCluster = false;
                        }
                        else
                        {
                            intersectsCluster = true;
                            intersectedCluster = true;
                        }
                        // Touching something
                        if (GridTypeEnum.EdgeType(space.Type))
                        {
                            intersectPoints.Add(new Point(x, y));
                        }
                        else
                        { // If not touching edge
                            overlappingSomething = true;
                            // If touching middle of cluster, not a good spots
                            if (intersectsCluster) return false;
                        }
                        return true;
                    }
                    else
                    {
                        // If center of moving object touching cluster, bad spot
                        return !cluster.Contains(x + curShift.x, y + curShift.y);
                    }
                })
                    && intersectPoints.Count > 0)
                { // Found intersection points, and didnt overlap
                    if (!intersectedCluster) continue;
                    // queue surrounding points
                    visited.DrawAround(curShift.x, curShift.y, true, Draw.Not(Draw.EqualTo(true)).IfThen(Draw.AddTo<bool>(shiftQueue).And(Draw.SetTo(true))));

                    if (!overlappingSomething)
                    {
                        double multiplier;
                        if (clusterByVolume)
                        {
                            multiplier = Math.Pow(intersectPoints.Count, 3);
                        }
                        else
                        {
                            intersectingObjs = new HashSet<LayoutObject<T>>();
                            foreach (Point intersect in intersectPoints)
                            {
                                LayoutObject<T> intersectObj;
                                if (!clusterByVolume && entireContainer.GetObjAt(intersect.x + curShift.x, intersect.y + curShift.y, out intersectObj))
                                {
                                    intersectingObjs.Add(intersectObj);
                                }
                            }
                            multiplier = Math.Pow(intersectingObjs.Count, 5);
                        }
                        shiftOptions.Add(new ClusterInfo() { Shift = curShift, Intersects = intersectPoints }, multiplier);
                        #region Debug
                        if (BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps) && BigBoss.Debug.logging(Logs.LevelGen))
                        {
                            BigBoss.Debug.w(Logs.LevelGen, "passed with " + intersectPoints.Count);
                            if (!clusterByVolume)
                            {
                                BigBoss.Debug.w(Logs.LevelGen, "intersected number of objs: " + intersectingObjs.Count);
                            }
                            BigBoss.Debug.w(Logs.LevelGen, "Mult " + multiplier);
                        }
                        #endregion
                    }
                }
            }
        }
        #region Debug
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            if (BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps))
            {
                shiftOptions.ToLog(BigBoss.Debug.Get(Logs.LevelGen), "Shift options");
            }
            BigBoss.Debug.printFooter("Generate Cluster Options");
        }
        #endregion
        return shiftOptions.Count > 0;
    }

    public static bool PlaceOrClusterAround<T>(this T theme, LevelGenerator gen, LayoutObject<GenSpace> clusterGroup, LayoutObject<GenSpace> obj)
        where T : Theme, IClusteringTheme
    {
        if (clusterGroup.Child || gen.Rand.Percent(theme.ClusterSplitPercentProperty))
        {
            if (PlaceRoom(theme, gen, clusterGroup, obj))
            {
                LayoutObject<GenSpace> cluster = new LayoutObject<GenSpace>(LayoutObjectType.Cluster);
                cluster.AddChild(obj);
                clusterGroup.AddChild(cluster);
                return true;
            }
        }
        else
        {
            List<LayoutObject<GenSpace>> clusterObjs = clusterGroup.GetChildren();
            while (clusterObjs.Count > 0)
            {
                LayoutObject<GenSpace> clusterObj = clusterObjs.RandomTake(gen.Rand);
                if (ClusterAround(theme, gen, clusterObj, obj))
                {
                    clusterObj.AddChild(obj);
                    return true;
                }
            }
        }
        return false;
    }

    public static bool ClusterAround<T>(this T theme, LevelGenerator gen, LayoutObject<GenSpace> cluster, LayoutObject<GenSpace> obj)
        where T : Theme, IClusteringTheme
    {
        #region Debug
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader("Cluster Around");
        }
        #endregion
        ProbabilityPool<ClusterInfo> shiftOptions;
        if (!IClusteringThemeExt.GenerateClusterOptions(gen.Layout, cluster, obj, true, out shiftOptions))
        {
            #region Debug
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                BigBoss.Debug.printFooter("Cluster Around");
            }
            #endregion
            return false;
        }

        List<Point> clusterDoorOptions = new List<Point>();
        ClusterInfo info;
        var placed = new List<Value2D<GenSpace>>(0);
        while (shiftOptions.Take(gen.Rand, out info))
        {
            gen.Layout.DrawPoints(info.Intersects, Draw.CanDrawDoor(true).IfThen(Draw.AddTo<GenSpace>(clusterDoorOptions)).Shift(info.Shift));
            #region Debug
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                BigBoss.Debug.w(Logs.LevelGen, "selected " + info.Shift);
                var tmpMap = new MultiMap<GenSpace>();
                cluster.DrawAll(Draw.CopyTo(tmpMap));
                obj.DrawAll(Draw.CopyTo(tmpMap, info.Shift));
                tmpMap.DrawPoints(info.Intersects, Draw.SetTo(GridType.INTERNAL_RESERVED_CUR, theme).Shift(info.Shift));
                tmpMap.ToLog(Logs.LevelGen, "Intersect Points");
                tmpMap = new MultiMap<GenSpace>();
                cluster.DrawAll(Draw.CopyTo(tmpMap));
                obj.DrawAll(Draw.CopyTo(tmpMap, info.Shift));
                tmpMap.DrawPoints(clusterDoorOptions, Draw.SetTo(GridType.Door, theme));
                tmpMap.ToLog(Logs.LevelGen, "Cluster door options");
            }
            #endregion
            if (clusterDoorOptions.Count > 0)
            { // Cluster side has door options
                obj.Shift(info.Shift.x, info.Shift.y);
                placed = theme.PlaceSomeDoors(obj, clusterDoorOptions, gen.Rand, true);
                if (placed.Count != 0)
                { // Placed a door
                    foreach (Point p in placed)
                    {
                        foreach (var room in obj.ConnectToRoomsAt(gen.Layout, p.x, p.y))
                        {
                            room.Remove(p);
                        }
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
        #region Debug
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            var tmpMap = new MultiMap<GenSpace>();
            tmpMap.PutAll(cluster);
            tmpMap.PutAll(obj);
            string extra = info != null ? info.Shift.ToString() : string.Empty;
            tmpMap.ToLog(Logs.LevelGen, "Final setup " + extra);
            BigBoss.Debug.printFooter("Cluster Around");
        }
        #endregion
        return placed.Count != 0;
    }

    public static bool PlaceRoom<T>(this T theme, LevelGenerator gen, LayoutObject<GenSpace> area, LayoutObject<GenSpace> obj, Point defaultPt = null)
        where T : Theme, IClusteringTheme
    {
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Place Rooms");
        }
        #endregion
        LayoutObject<GenSpace> startRoom;
        List<LayoutObject<GenSpace>> children = area.GetChildren();
        if (children.Count == 0)
        {
            obj.Shift(area.Center);
            #region DEBUG
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                BigBoss.Debug.w("First room to place");
                BigBoss.Debug.printFooter(Logs.LevelGen, "Place Rooms");
            }
            #endregion
            return true;
        }
        while (children.RandomTake(gen.Rand, out startRoom))
        {
            // Find where it will shift away
            Point shiftMagn = LevelGenerator.GenerateShiftMagnitude(SHIFT_RANGE, gen.Rand);
            for (int i = 0; i < 4; i++)
            {
                #region DEBUG
                if (BigBoss.Debug.logging(Logs.LevelGen))
                {
                    BigBoss.Debug.w(Logs.LevelGen, "Placing obj: " + obj);
                    startRoom.ToLog(Logs.LevelGen, "Starting on this");
                    BigBoss.Debug.w(Logs.LevelGen, "Shift: " + shiftMagn);
                }
                #endregion
                // Find room it will start from
                Point shift = obj.GetCenterShiftOn(startRoom);
                shift = obj.GetShiftOutside(area, shiftMagn, shift);
                if (!obj.Intersects(gen.Layout, shift.x, shift.y))
                {
                    obj.Shift(shift);
                    #region DEBUG
                    if (BigBoss.Debug.logging(Logs.LevelGen))
                    {
                        MultiMap<GenSpace> tmp = new MultiMap<GenSpace>();
                        tmp.PutAll(gen.Layout);
                        tmp.PutAll(obj);
                        tmp.ToLog(Logs.LevelGen, "Layout after placing room at: " + obj.Bounding);
                        BigBoss.Debug.printFooter(Logs.LevelGen, "Place Rooms");
                    }
                    #endregion
                    return true;
                }
                else
                { // Hit some other area
                    shiftMagn.Rotate(Rotation.ClockWise);
                    #region DEBUG
                    if (BigBoss.Debug.logging(Logs.LevelGen))
                    {
                        MultiMap<GenSpace> tmp = new MultiMap<GenSpace>();
                        tmp.PutAll(gen.Layout);
                        tmp.PutAll(obj, shift);
                        tmp.ToLog(Logs.LevelGen, "Failed to place room at: " + obj.Bounding);
                        BigBoss.Debug.printFooter(Logs.LevelGen, "Place Rooms");
                    }
                    #endregion
                }
            }
        }
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.w("Failed to place room");
            BigBoss.Debug.printFooter(Logs.LevelGen, "Place Rooms");
        }
        #endregion
        return false;
    }
}
