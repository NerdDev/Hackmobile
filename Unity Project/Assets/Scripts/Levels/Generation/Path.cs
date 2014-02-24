using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class Path : IEnumerable<Value2D<GridSpace>>
{
    private static HashSet<GridType> _pathTypes = new HashSet<GridType>(new[] {
            GridType.Floor,
            GridType.Door,
            GridType.Path_Horiz,
            GridType.Path_LB,
            GridType.Path_LT,
            GridType.Path_RB,
            GridType.Path_RT,
            GridType.Path_Vert
    });
    public static HashSet<GridType> PathTypes { get { return _pathTypes; } }
    List<Value2D<GridSpace>> List;
    public bool Valid { get { return List.Count > 2; } }
    public Value2D<GridSpace> FirstEnd { get { return List.Count > 0 ? List[0] : null; } }
    public Value2D<GridSpace> SecondEnd { get { return List.Count > 0 ? List[List.Count - 1] : null; } }

    public Path(IEnumerable<Value2D<GridSpace>> stack)
    {
        List = new List<Value2D<GridSpace>>(stack);
    }

    public static IEnumerable<GridSpace> PathPrint(IEnumerable<Point> points)
    {
        Point backward = null;
        Point cur = null;
        Point forward = null;
        foreach (Point p in points)
        {
            forward = p;
            if (cur != null)
            {
                if (backward == null) { }
                else if (Mathf.Abs(forward.x - backward.x) == 2)
                {
                    // Horizontal
                    yield return new GridSpace(GridType.Path_Horiz, cur.x, cur.y);
                }
                else if (Mathf.Abs(forward.y - backward.y) == 2)
                {
                    // Vertical
                    yield return new GridSpace(GridType.Path_Vert, cur.x, cur.y);
                }
                else
                {
                    // Corner
                    bool top = (forward.y == (cur.y + 1)) || (backward.y == (cur.y + 1));
                    bool right = (forward.x == (cur.x + 1)) || (backward.x == (cur.x + 1));
                    if (top)
                    {
                        if (right)
                        {
                            yield return new GridSpace(GridType.Path_RT, cur.x, cur.y);
                        }
                        else
                        {
                            yield return new GridSpace(GridType.Path_LT, cur.x, cur.y);
                        }
                    }
                    else
                    {
                        if (right)
                        {
                            yield return new GridSpace(GridType.Path_RB, cur.x, cur.y);
                        }
                        else
                        {
                            yield return new GridSpace(GridType.Path_LB, cur.x, cur.y);
                        }
                    }
                }
            }
            // Set up for next point
            backward = cur;
            cur = forward;
        }
    }

    public void Simplify()
    {
        Prune();
    }

    public LayoutObject Bake()
    {
        LayoutObject obj = new LayoutObject("Path");
        foreach (var v in PathPrint(List.Cast<Point>()))
        {
            obj[v.X, v.Y] = v;
        }
        return obj;
    }

    void Prune()
    {
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen) && BigBoss.Debug.Flag(DebugManager.DebugFlag.LevelGen_Path_Simplify_Prune))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Prune");
        }
        #endregion
        Bounding bounds = new Bounding();
        foreach (Value2D<GridSpace> g in List) bounds.Absorb(g);
        Array2D<int> indexes = new Array2D<int>(bounds);
        List<Value2D<GridSpace>> tmp = new List<Value2D<GridSpace>>(List);
        int index = 0;
        foreach (Value2D<GridSpace> val in tmp)
        { // For each point on the path
            int lastDiff = 0;
            Value2D<int> neighbor = null;
            indexes.DrawAround(val.x, val.y, false, (arr2, x, y) =>
            { // Find neighboring point on path with the largest distance from current
                if (arr2[x, y] == 0) return true;
                int valDiff = Mathf.Abs(index - arr2[x, y]);
                if (valDiff > 1 // Diff meets requirements
                    && (neighbor == null || lastDiff < valDiff)) // Larger than last found diff
                {
                    lastDiff = valDiff;
                    neighbor = new Value2D<int>(x, y, arr2[x, y]);
                }
                return true;
            });
            #region DEBUG
            if (BigBoss.Debug.logging(Logs.LevelGen) && BigBoss.Debug.Flag(DebugManager.DebugFlag.LevelGen_Path_Simplify_Prune))
            {
                BigBoss.Debug.w(Logs.LevelGen, "Evaluating " + val);
                if (neighbor != null)
                {
                    BigBoss.Debug.w(Logs.LevelGen, "Found Neighbor " + neighbor);
                }
            }
            #endregion
            if (neighbor != null)
            { // If point of interest exists, prune	
                int fromIndex = neighbor.val + 1;
                int count = index - neighbor.val - 1;
                // Set indices to 0
                List<Value2D<GridSpace>> toRemove = List.GetRange(fromIndex, count);
                foreach (Value2D<GridSpace> r in toRemove)
                {
                    indexes[r.x, r.y] = 0;
                }
                // Remove
                List.RemoveRange(fromIndex, count);
                // Set next index to proper number
                index = neighbor.val + 1;
                #region DEBUG
                if (BigBoss.Debug.logging(Logs.LevelGen) && BigBoss.Debug.Flag(DebugManager.DebugFlag.LevelGen_Path_Simplify_Prune))
                {
                    BigBoss.Debug.w(Logs.LevelGen, "Removed index: " + fromIndex + " count: " + count);
                    MultiMap<GridType> map = new MultiMap<GridType>();
                    foreach (var v in List)
                    {
                        map[v] = v.val.Type;
                    }
                    map.ToLog(Logs.LevelGen);
                }
                #endregion
            }
            indexes[val.x, val.y] = index;
            index++;
        }
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen) && BigBoss.Debug.Flag(DebugManager.DebugFlag.LevelGen_Path_Simplify_Prune))
        {
            BigBoss.Debug.printFooter(Logs.LevelGen, "Prune");
        }
        #endregion
    }

    public IEnumerator<Value2D<GridSpace>> GetEnumerator()
    {
        return List.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}
