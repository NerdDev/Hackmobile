using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class Path : IEnumerable<Value2D<GenSpace>>
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
    List<Value2D<GenSpace>> List;
    public bool Valid { get { return List.Count > 2; } }
    public Value2D<GenSpace> FirstEnd { get { return List.Count > 0 ? List[0] : null; } }
    public Value2D<GenSpace> SecondEnd { get { return List.Count > 0 ? List[List.Count - 1] : null; } }

    public Path(IEnumerable<Value2D<GenSpace>> stack)
    {
        List = new List<Value2D<GenSpace>>(stack);
    }

    public static IEnumerable<Value2D<GridType>> PathPrint(IEnumerable<Point> points)
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
                    yield return new Value2D<GridType>(cur.x, cur.y, GridType.Path_Horiz);
                }
                else if (Mathf.Abs(forward.y - backward.y) == 2)
                {
                    // Vertical
                    yield return new Value2D<GridType>(cur.x, cur.y, GridType.Path_Vert);
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
                            yield return new Value2D<GridType>(cur.x, cur.y, GridType.Path_RT);
                        }
                        else
                        {
                            yield return new Value2D<GridType>(cur.x, cur.y, GridType.Path_LT);
                        }
                    }
                    else
                    {
                        if (right)
                        {
                            yield return new Value2D<GridType>(cur.x, cur.y, GridType.Path_RB);
                        }
                        else
                        {
                            yield return new Value2D<GridType>(cur.x, cur.y, GridType.Path_LB);
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

    public LayoutObject Bake(Theme theme)
    {
        LayoutObject obj = new LayoutObject("Path");
        foreach (var v in PathPrint(List.Cast<Point>()))
        {
            obj[v.x, v.y] = new GenSpace(v.val, theme);
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
        foreach (Value2D<GenSpace> g in List) bounds.Absorb(g);
        Array2D<int> indexes = new Array2D<int>(bounds);
        List<Value2D<GenSpace>> tmp = new List<Value2D<GenSpace>>(List);
        int index = 0;
        foreach (Value2D<GenSpace> val in tmp)
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
                List<Value2D<GenSpace>> toRemove = List.GetRange(fromIndex, count);
                foreach (Value2D<GenSpace> r in toRemove)
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

    public IEnumerator<Value2D<GenSpace>> GetEnumerator()
    {
        return List.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}
