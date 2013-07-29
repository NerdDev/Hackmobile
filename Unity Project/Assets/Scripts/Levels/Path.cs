using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Path : LayoutObjectLeaf
{

    private static GridType[] searchTypes = new GridType[]
        {
            GridType.Floor,
            GridType.Door
        };
    private static GridSet typesSet = new GridSet(searchTypes);
    List<Value2D<GridType>> _list;

    public Path(Value2D<GridType> startPoint, GridArray grids)
        : base()
    {
        DFSSearcher searcher = new DFSSearcher(LevelGenerator.Rand);
        _list = new List<Value2D<GridType>>(searcher.Search(startPoint, grids, GridType.NULL, typesSet));
    }

    public Path(IEnumerable<Value2D<GridType>> stack)
        : base()
	{
        _list = new List<Value2D<GridType>>(stack);
	}

    public static GridSet PathTypes()
    {
        return typesSet;
    }

    protected override Bounding GetBoundingUnshifted()
    {
        if (grids != null)
        {
            return base.GetBoundingUnshifted();
        }
        Bounding ret = new Bounding();
        foreach (Value2D<GridType> val in _list)
        {
            ret.absorb(val);
        }
        return ret;
    }

    public override GridArray GetArray()
    {
        return GetArray(true);
    }

    public override GridArray GetPrintArray()
    {
        return GetArray(true);
    }

    public void Finalize(LayoutObjectContainer obj)
    {
        Simplify();
        ConnectEnds(obj);
        Bake(true);
    }

    public override void Bake(bool shiftCompensate)
    {
        grids = GetArray(false);
        _list = null;
        base.Bake(shiftCompensate);
    }

    public GridArray GetArray(bool print)
    {
        if (grids != null)
        {
            return grids;
        }
        Bounding bounds = GetBoundingUnshifted();
        GridArray ret = new GridArray(bounds, false);
        if (_list.Count > 0)
        {
            if (print)
            {
                Value2D<GridType> backward = null;
                Value2D<GridType> cur = null;
                Value2D<GridType> forward = null;
                foreach (Value2D<GridType> val in _list)
                {
                    forward = val;
                    if (print)
                    { // Handle piping print logic
                        if (cur != null)
                        {
                            if (backward == null)
                            {
                                ret[cur.x, cur.y] = GridType.INTERNAL_RESERVED_CUR;
                            }
                            else if (Mathf.Abs(forward.x - backward.x) == 2)
                            {
                                // Horizontal
                                ret[cur.x, cur.y] = GridType.Path_Horiz;
                            }
                            else if (Mathf.Abs(forward.y - backward.y) == 2)
                            {
                                // Vertical
                                ret[cur.x, cur.y] = GridType.Path_Vert;
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
                                        ret[cur.x, cur.y] = GridType.Path_RT;
                                    }
                                    else
                                    {
                                        ret[cur.x, cur.y] = GridType.Path_LT;
                                    }
                                }
                                else
                                {
                                    if (right)
                                    {
                                        ret[cur.x, cur.y] = GridType.Path_RB;
                                    }
                                    else
                                    {
                                        ret[cur.x, cur.y] = GridType.Path_LB;
                                    }
                                }
                            }
                        }
                        // Set up for next point
                        backward = cur;
                        cur = forward;
                    }
                    else
                    {
                        ret[forward.x, forward.y] = GridType.Floor;
                    }
                }
                ret[forward.x, forward.y] = GridType.INTERNAL_RESERVED_CUR;
            }
            else
            {
                Value2D<GridType> first = _list[0];
                Value2D<GridType> last = null;
                foreach (Value2D<GridType> val in _list)
                {
                    last = val;
                    ret[val] = GridType.Floor;
                }
                ret[first] = GridType.NULL;
                ret[last] = GridType.NULL;
            }
        }
        return ret;
    }

    public void Simplify()
    {
        Prune();
    }

    void Prune()
    {
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen) && DebugManager.Flag(DebugManager.DebugFlag.LevelGen_Path_Simplify_Prune))
        {
            DebugManager.printHeader(DebugManager.Logs.LevelGen, "Prune");
        }
        #endregion
        Bounding bounds = GetBounding();
        Array2D<int> indexes = new Array2D<int>(bounds, false);
        List<Value2D<GridType>> tmp = new List<Value2D<GridType>>(_list);
        SurroundingInt surround = new SurroundingInt(indexes.GetArr());
        int index = 0;
        foreach (Value2D<GridType> val in tmp)
        { // For each point on the path
            surround.Load(val.x, val.y);
            Value2D<int> neighbor = surround.GetDirWithValDiffLarger(index, 1);
            #region DEBUG
            if (DebugManager.logging(DebugManager.Logs.LevelGen) && DebugManager.Flag(DebugManager.DebugFlag.LevelGen_Path_Simplify_Prune))
            {
                DebugManager.w(DebugManager.Logs.LevelGen, "Evaluating " + val);
                if (neighbor != null)
                {
                    DebugManager.w(DebugManager.Logs.LevelGen, "Found Neighbor " + neighbor);
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
                if (DebugManager.logging(DebugManager.Logs.LevelGen) && DebugManager.Flag(DebugManager.DebugFlag.LevelGen_Path_Simplify_Prune))
                {
                    DebugManager.w(DebugManager.Logs.LevelGen, "Removed index: " + fromIndex + " count: " + count);
                    ToLog(DebugManager.Logs.LevelGen);
                }
                #endregion
            }
            indexes[val.x, val.y] = index;
            index++;
        }
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen) && DebugManager.Flag(DebugManager.DebugFlag.LevelGen_Path_Simplify_Prune))
        {
            DebugManager.printFooter(DebugManager.Logs.LevelGen);
        }
        #endregion
    }

    public override bool isValid()
    {
        return grids != null || (_list != null && _list.Count > 2);
    }

    public void ConnectEnds(LayoutObjectContainer container)
    {
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printHeader(DebugManager.Logs.LevelGen, "Connect Ends");
        }
        #endregion
        container.FindAndConnect(this, _list[0]);
        container.FindAndConnect(this, _list[_list.Count - 1]);
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printFooter(DebugManager.Logs.LevelGen);
        }
        #endregion
    }

    public override string GetTypeString()
    {
        return "Path";
    }
}
