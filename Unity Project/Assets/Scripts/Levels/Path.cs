using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Path : LayoutObject {
	
	List<Value2D<GridType>> list;

    public Path(IEnumerable<Value2D<GridType>> stack)
        : base()
	{
        list = new List<Value2D<GridType>>(stack);
	}

    protected override Bounding GetBoundingInternal()
    {
        Bounding ret = new Bounding();
        foreach (Value2D<GridType> val in list)
        {
            ret.absorb(val);
        }
        return ret;
    }

    public override GridArray GetArray()
    {
        return GetArray(false);
    }

    public GridArray GetArray(bool ending)
    {
        Bounding bounds = GetBoundingInternal();
        GridArray ret = new GridArray(bounds, false);
        if (list.Count > 0)
        {
            Value2D<GridType> backwardPt = null;
            Value2D<GridType> curPoint = null;
            foreach (Value2D<GridType> forwardPt in list)
            {
                if (curPoint != null)
                {
                    if (backwardPt == null)
                    { // Start Point
                        if (ending)
                        {
                            ret[curPoint.x, curPoint.y] = GridType.INTERNAL_RESERVED_CUR;
                        }
                    }
                    else if (Mathf.Abs(forwardPt.x - backwardPt.x) == 2)
                    { // Horizontal
                        ret[curPoint.x, curPoint.y] = GridType.Path_Horiz;
                    }
                    else if (Mathf.Abs(forwardPt.y - backwardPt.y) == 2)
                    { // Vertical
                        ret[curPoint.x, curPoint.y] = GridType.Path_Vert;
                    }
                    else
                    { // Corner
                        bool top = (forwardPt.y == (curPoint.y + 1)) || (backwardPt.y == (curPoint.y + 1));
                        bool right = (forwardPt.x == (curPoint.x + 1)) || (backwardPt.x == (curPoint.x + 1));
                        if (top)
                        {
                            if (right)
                            {
                                ret[curPoint.x, curPoint.y] = GridType.Path_RT;
                            }
                            else
                            {
                                ret[curPoint.x, curPoint.y] = GridType.Path_LT;
                            }
                        }
                        else
                        {
                            if (right)
                            {
                                ret[curPoint.x, curPoint.y] = GridType.Path_RB;
                            }
                            else
                            {
                                ret[curPoint.x, curPoint.y] = GridType.Path_LB;
                            }
                        }
                    }
                }
                // Set up for next point
                backwardPt = curPoint;
                curPoint = forwardPt;
            }
            if (ending)
            {
                ret[curPoint.x, curPoint.y] = GridType.INTERNAL_RESERVED_CUR;
            }
        }
        return ret;
    }

    public override GridArray GetPrintArray()
    {
        return GetArray(true);
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
        List<Value2D<GridType>> tmp = new List<Value2D<GridType>>(list);
        int index = 0;
        foreach (Value2D<GridType> val in tmp)
        { // For each point on the path
            SurroundingInt surround = SurroundingInt.Get(indexes, val.x, val.y);
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
                List<Value2D<GridType>> toRemove = list.GetRange(fromIndex, count);
                foreach (Value2D<GridType> r in toRemove)
                {
                    indexes[r.x, r.y] = 0;
                }
                // Remove
                list.RemoveRange(fromIndex, count);
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
        return list.Count > 0;
    }

    public override bool Contains(Value2D<GridType> val)
    {
        return list.Contains(val);
    }

    public void ConnectEnds(LevelLayout layout)
    {
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printHeader(DebugManager.Logs.LevelGen, "Connect Ends");
        }
        #endregion
        layout.FindAndConnect(this, list[0]);
        layout.FindAndConnect(this, list[list.Count - 1]);
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
