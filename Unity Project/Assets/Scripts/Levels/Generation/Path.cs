using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class Path : LayoutObject
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
    List<Value2D<GridType>> _list;

    public Value2D<GridType> FirstEnd { get { return _list.Count > 0 ? _list[0] : null; } }
    public Value2D<GridType> SecondEnd { get { return _list.Count > 0 ? _list[_list.Count - 1] : null; } }

    public Path(IEnumerable<Value2D<GridType>> stack)
    {
        _list = new List<Value2D<GridType>>(stack);
    }

    public override Bounding Bounding
    {
        get
        {
            if (Grids != null)
            {
                return base.Bounding;
            }
            Bounding ret = new Bounding();
            foreach (Value2D<GridType> val in _list)
            {
                ret.Absorb(val);
            }
            return ret;
        }
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

    public override Container2D<GridType> Grids
    {
        get
        {
            if (_list == null)
                return base.Grids;
            MultiMap<GridType> ret = new MultiMap<GridType>();
            if (_list.Count == 0) return ret;
            foreach (Value2D<GridType> val in PathPrint(_list.Cast<Point>()))
            {
                ret[val] = val.val;
            }
            return ret;
        }
        protected set
        {
            base.Grids = value;
        }
    }

    public override LayoutObject Bake()
    {
        base.Grids = Grids;
        _list = null;
        return base.Bake();
    }

    public void Simplify()
    {
        Prune();
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
        foreach (Value2D<GridType> g in _list) bounds.Absorb(g);
        Array2D<int> indexes = new Array2D<int>(bounds);
        List<Value2D<GridType>> tmp = new List<Value2D<GridType>>(_list);
        int index = 0;
        foreach (Value2D<GridType> val in tmp)
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
                List<Value2D<GridType>> toRemove = _list.GetRange(fromIndex, count);
                foreach (Value2D<GridType> r in toRemove)
                {
                    indexes[r.x, r.y] = 0;
                }
                // Remove
                _list.RemoveRange(fromIndex, count);
                // Set next index to proper number
                index = neighbor.val + 1;
                #region DEBUG
                if (BigBoss.Debug.logging(Logs.LevelGen) && BigBoss.Debug.Flag(DebugManager.DebugFlag.LevelGen_Path_Simplify_Prune))
                {
                    BigBoss.Debug.w(Logs.LevelGen, "Removed index: " + fromIndex + " count: " + count);
                    ToLog(Logs.LevelGen);
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

    public override bool isValid()
    {
        return _list == null || _list.Count > 2;
    }

    public void ConnectEnds(LayoutObjectContainer container, Point shift)
    {
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Connect Ends");
        }
        #endregion
        container.FindAndConnect(this, _list[0] + shift);
        container.FindAndConnect(this, _list[_list.Count - 1] + shift);
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printFooter(Logs.LevelGen, "Connect Ends");
        }
        #endregion
    }

    public override string GetTypeString()
    {
        return "Path";
    }
}
