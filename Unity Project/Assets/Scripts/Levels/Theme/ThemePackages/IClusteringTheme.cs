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

    public static ProbabilityPool<ClusterInfo> GenerateClusterOptions<T>(
        Container2D<T> clusterGrid,
        LayoutObjectContainer<T> cluster,
        LayoutObject<T> obj,
        bool clusterByVolume)
        where T : IGridType
    {
        return GenerateClusterOptions(clusterGrid, cluster, clusterGrid, cluster, obj, clusterByVolume);
    }

    public static ProbabilityPool<ClusterInfo> GenerateClusterOptions<T>(
        Container2D<T> entireGrid,
        LayoutObjectContainer<T> entireContainer,
        Container2D<T> clusterGrid,
        ILayoutObject<T> cluster,
        LayoutObject<T> obj,
        bool clusterByVolume)
        where T : IGridType
    {
        #region Debug
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader("Generate Cluster Options");
        }
        #endregion
        ProbabilityList<ClusterInfo> shiftOptions = new ProbabilityList<ClusterInfo>();
        List<LayoutObject<T>> items = cluster.Flatten();
        if (items.Count != 0)
        {
            obj.ShiftP = new Point(items[0].ShiftP);
            obj.ShiftOutside(cluster, new Point(1, 0), null, false, false);
            obj.Shift(-1, 0); // Shift to overlapping slightly
            MultiMap<bool> visited = new MultiMap<bool>();
            visited[0, 0] = true;
            Queue<Point> shiftQueue = new Queue<Point>();
            shiftQueue.Enqueue(new Point());
            Container2D<T> objGrid = obj.GetGrid();
            #region Debug
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                var tmp = new MultiMap<T>();
                tmp.PutAll(obj.GetGrid());
                tmp.PutAll(clusterGrid);
                tmp.ToLog(Logs.LevelGen, "Starting cluster grid");
                tmp = new MultiMap<T>();
                tmp.PutAll(obj.GetGrid());
                tmp.PutAll(entireGrid);
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
                    tmpMap.PutAll(entireGrid);
                    tmpMap.PutAll(objGrid, curShift);
                    tmpMap.ToLog(Logs.LevelGen, "Analyzing at shift " + curShift);
                }
                #endregion
                // Test if pass
                List<Point> intersectPoints = new List<Point>();
                bool overlappingSomething = false;
                bool intersectsCluster = false;
                HashSet<LayoutObject<T>> intersectingObjs = null;
                if (objGrid.DrawAll((arr, x, y) =>
                { // Draw on moving object to detect intersection pts
                    if (GridTypeEnum.EdgeType(arr[x, y].GetGridType()))
                    {
                        GridType type = clusterGrid[x + curShift.x, y + curShift.y].GetGridType();
                        if (type == GridType.NULL)
                        {
                            type = entireGrid[x + curShift.x, y + curShift.y].GetGridType();
                            if (type == GridType.NULL) return true;
                        }
                        else
                        {
                            intersectsCluster = true;
                        }
                        // Touching something
                        if (GridTypeEnum.EdgeType(type))
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
                        // If center of moving object touching anything on grid, bad spot
                        return !entireGrid.Contains(x + curShift.x, y + curShift.y);
                    }
                })
                    && intersectPoints.Count > 0)
                { // Found intersection points, and didnt overlap
                    if (!intersectsCluster) continue;
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
        return shiftOptions;
    }

    public static bool PlaceOrClusterAround<T>(this T theme, LevelGenerator gen, LayoutObjectContainer<GenSpace> clusterGroup, LayoutObject<GenSpace> obj)
        where T : Theme, IClusteringTheme
    {
        if (clusterGroup.Objects.Count == 0 || gen.Rand.Percent(theme.ClusterSplitPercentProperty))
        {
            if (PlaceRoom(theme, gen, clusterGroup, obj))
            {
                LayoutObjectContainer<GenSpace> cluster = new LayoutObjectContainer<GenSpace>();
                cluster.Objects.Add(obj);
                clusterGroup.Objects.Add(cluster);
                return true;
            }
        }
        else
        {
            LayoutObjectContainer<GenSpace> layoutObj = (LayoutObjectContainer<GenSpace>)clusterGroup.Objects.Random(gen.Rand);
            if (ClusterAround(theme, gen, layoutObj, obj))
            {
                layoutObj.Objects.Add(obj);
                return true;
            }
        }
        return false;
    }

    public static bool ClusterAround<T>(this T theme, LevelGenerator gen, LayoutObjectContainer<GenSpace> cluster, LayoutObject<GenSpace> obj)
        where T : Theme, IClusteringTheme
    {
        #region Debug
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader("Cluster Around");
        }
        #endregion
        ProbabilityPool<ClusterInfo> shiftOptions = IClusteringThemeExt.GenerateClusterOptions(gen.Layout.Grids, gen.Layout.AllContainer, cluster.GetGrid(), cluster, obj, true);
        List<Point> clusterDoorOptions = new List<Point>();
        ClusterInfo info;
        Container2D<GenSpace> clusterGrid = cluster.GetGrid();
        var placed = new List<Value2D<GenSpace>>(0);
        while (shiftOptions.Take(gen.Rand, out info))
        {
            clusterGrid.DrawPoints(info.Intersects, Draw.CanDrawDoor().IfThen(Draw.AddTo<GenSpace>(clusterDoorOptions)).Shift(info.Shift));
            #region Debug
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                BigBoss.Debug.w(Logs.LevelGen, "selected " + info.Shift);
                var tmpMap = new MultiMap<GenSpace>();
                clusterGrid.DrawAll(Draw.CopyTo(tmpMap));
                obj.GetGrid().DrawAll(Draw.CopyTo(tmpMap, info.Shift));
                tmpMap.DrawPoints(info.Intersects, Draw.SetTo(GridType.INTERNAL_RESERVED_CUR, theme).Shift(info.Shift));
                tmpMap.ToLog(Logs.LevelGen, "Intersect Points");
                tmpMap = new MultiMap<GenSpace>();
                clusterGrid.DrawAll(Draw.CopyTo(tmpMap));
                obj.GetGrid().DrawAll(Draw.CopyTo(tmpMap, info.Shift));
                tmpMap.DrawPoints(clusterDoorOptions, Draw.SetTo(GridType.Door, theme));
                tmpMap.ToLog(Logs.LevelGen, "Cluster door options");
            }
            #endregion
            if (clusterDoorOptions.Count > 0)
            { // Cluster side has door options
                obj.Shift(info.Shift.x, info.Shift.y);
                placed = theme.PlaceSomeDoors(obj, clusterDoorOptions, gen.Rand);
                if (placed.Count != 0)
                { // Placed a door
                    foreach (Point p in placed)
                    {
                        LayoutObject<GenSpace> clusterObj;
                        cluster.GetObjAt(p.x, p.y, out clusterObj);
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
        return placed.Count != 0;
    }

    public static bool PlaceRoom<T>(this T theme, LevelGenerator gen, LayoutObjectContainer<GenSpace> area, LayoutObject<GenSpace> obj)
        where T : Theme, IClusteringTheme
    {
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Place Rooms");
        }
        #endregion
        if (area.Objects.Count != 0)
        {
            // Find room it will start from
            int roomNum = gen.Rand.Next(area.Objects.Count);
            ILayoutObject<GenSpace> startRoom = area.Objects[roomNum];
            obj.CenterOn(startRoom);
            // Find where it will shift away
            Point shiftMagn = LevelGenerator.GenerateShiftMagnitude(SHIFT_RANGE, gen.Rand);
            #region DEBUG
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                BigBoss.Debug.w(Logs.LevelGen, "Placing obj: " + obj);
                BigBoss.Debug.w(Logs.LevelGen, "Picked starting obj number: " + roomNum);
                BigBoss.Debug.w(Logs.LevelGen, "Shift: " + shiftMagn);
            }
            #endregion
            obj.ShiftOutside(area.Objects, shiftMagn);
        }
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            area.ToLog(Logs.LevelGen, "Layout after placing room at: " + obj.Bounding);
            BigBoss.Debug.printFooter(Logs.LevelGen, "Place Rooms");
        }
        #endregion
        return true;
    }
}
