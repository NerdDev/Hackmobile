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
		Bounding bounds = GetBounding();
		int[,] indexes = new int[bounds.height, bounds.width];
		List<Value2D<GridType>> tmp = new List<Value2D<GridType>>(list);
		int index = 0;
		foreach (Value2D<GridType> val in tmp)
		{ // For each point on the path
			Surrounding<int> surround = Surrounding<int>.Get(indexes, val.x - bounds.xMin, val.y - bounds.yMin);
            foreach (Value2D<int> s in surround)
            { // Check each surrounding point
                if (s != null && s.val != 0)
                { // If point exists, prune
                    // Set indices to 0
                    List<Value2D<GridType>> toRemove = list.GetRange(s.val, index - s.val);
                    foreach (Value2D<GridType> remove in toRemove)
                    {
                        indexes[remove.y, remove.x] = 0;
                    }
                    // Remove
                    list.RemoveRange(s.val, index - s.val);
                    // Set next index to proper number
                    index = s.val + 1;
                }
            }
			indexes[val.y - bounds.yMin, val.x - bounds.xMin] = index;
		}
	}
}
