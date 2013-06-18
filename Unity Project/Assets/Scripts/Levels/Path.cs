using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Path : LayoutObject {
	
	Stack<Value2D<GridType>> stack;
	
	public Path (Stack<Value2D<GridType>> stack)
	{
		this.stack = stack;
	}

    protected override Bounding GetBoundingInternal()
    {
        Bounding ret = new Bounding();
        foreach (Value2D<GridType> val in stack)
        {
            ret.absorb(val);
        }
        return ret;
    }

    public override GridArray GetArray()
    {
        Bounding bounds = GetBoundingInternal();
        GridArray ret = new GridArray(bounds);
        Value2D<GridType> backwardPt = null;
        Value2D<GridType> curPoint = null;
        foreach (Value2D<GridType> forwardPt in stack)
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
        return ret;
    }

    public override GridArray GetBakedArray()
    {
        return new GridArray(GetArray(), shiftP);
    }
}
