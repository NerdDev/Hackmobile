using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Path : LayoutObject {
	
	List<Value2D<GridType>> list;
	
	public Path (Stack<Value2D<GridType>> stack)        
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
	                    ret.Put(GridType.INTERNAL_RESERVED_CUR, curPoint.x, curPoint.y);
	                }
	                else if (Mathf.Abs(forwardPt.x - backwardPt.x) == 2)
	                { // Horizontal
	                    ret.Put(GridType.INTERNAL_RESERVED_HORIZ, curPoint.x, curPoint.y);
	                }
	                else if (Mathf.Abs(forwardPt.y - backwardPt.y) == 2)
	                { // Vertical
	                    ret.Put(GridType.INTERNAL_RESERVED_VERT, curPoint.x, curPoint.y);
	                }
	                else
	                { // Corner
	                    bool top = (forwardPt.y == (curPoint.y + 1)) || (backwardPt.y == (curPoint.y + 1));
	                    bool right = (forwardPt.x == (curPoint.x + 1)) || (backwardPt.x == (curPoint.x + 1));
	                    if (top)
	                    {
	                        if (right)
	                        {
	                            ret.Put(GridType.INTERNAL_RESERVED_RT, curPoint.x, curPoint.y);
	                        }
	                        else
	                        {
	                            ret.Put(GridType.INTERNAL_RESERVED_LT, curPoint.x, curPoint.y);
	                        }
	                    }
	                    else
	                    {
	                        if (right)
	                        {
	                            ret.Put(GridType.INTERNAL_RESERVED_RB, curPoint.x, curPoint.y);
	                        }
	                        else
	                        {
	                            ret.Put(GridType.INTERNAL_RESERVED_LB, curPoint.x, curPoint.y);
	                        }
	                    }
	                }
	            }
	            // Set up for next point
	            backwardPt = curPoint;
	            curPoint = forwardPt;
	        }
	        ret.Put(GridType.INTERNAL_RESERVED_CUR, curPoint.x, curPoint.y);
		}
        return ret;
    }

    public void simplify()
    {
		#region DEBUG
		if (DebugManager.logging(DebugManager.Logs.LevelGen))
		{
			DebugManager.printHeader(DebugManager.Logs.LevelGen, "Simplify");	
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
			if (DebugManager.logging(DebugManager.Logs.LevelGen))
			{
				DebugManager.w (DebugManager.Logs.LevelGen, "Evaluating " + val);
				if (neighbor != null)
				{
					DebugManager.w (DebugManager.Logs.LevelGen, "Found Neighbor " + neighbor);
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
                    indexes.Put(0, r.x, r.y);
                }
                // Remove
                list.RemoveRange(fromIndex, count);
                // Set next index to proper number
                index = neighbor.val + 1;
				#region DEBUG
				if (DebugManager.logging(DebugManager.Logs.LevelGen))
				{
					DebugManager.w (DebugManager.Logs.LevelGen, "Removed index: " + fromIndex + " count: " + count); 
					toLog(DebugManager.Logs.LevelGen);
				}
				#endregion
            }
            indexes.Put(index, val.x, val.y);
            index++;
        }
		#region DEBUG
		if (DebugManager.logging(DebugManager.Logs.LevelGen))
		{
			DebugManager.printFooter(DebugManager.Logs.LevelGen);	
		}
		#endregion
    }
}
