using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

abstract public class LayoutObject {

    protected Point shiftP = new Point();

    #region Shifts
    public void shift(int x, int y)
	{
		shiftP.shift(x,y);
	}
	
	public void shift(Point p)
	{
		shiftP.shift(p);	
	}

    public void setShift(LayoutObject rhs)
    {
        shiftP.x = rhs.shiftP.x;
        shiftP.y = rhs.shiftP.y;
    }

    public void ShiftOutside(LayoutObject rhs, Point dir)
    {
        Point reduc = dir.reduce();
		#region DEBUG
		if (DebugManager.logging(DebugManager.Logs.LevelGen)) {
            DebugManager.printHeader(DebugManager.Logs.LevelGen, "Shift Outside " + ToString());
            DebugManager.w(DebugManager.Logs.LevelGen, "Shifting outside of " + rhs.ToString());
            DebugManager.w(DebugManager.Logs.LevelGen, "Shift " + dir + "   Reduc shift: " + reduc);
			DebugManager.w (DebugManager.Logs.LevelGen, "Bounds: " + GetBounding() + "  RHS bounds: " + rhs.GetBounding());
		}
		#endregion
		while(this.intersects(rhs))
		{
            // Shift small increments until not overlapping
            shift(reduc);
            #region DEBUG
            if (DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                DebugManager.w(DebugManager.Logs.LevelGen, "Shifted to: " + GetBounding());
            }
            #endregion
		}
        // Shift final distance away from object
        shift(dir);
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printFooter(DebugManager.Logs.LevelGen);
        }
        #endregion
    }
    #endregion Shifts

    #region UNUSED
    public void ShiftOutsideBulk(LayoutObject rhs, Point dir)
    { // Unused atm, broken
        DebugManager.printHeader(DebugManager.Logs.LevelGen, "Shift Outside Bulk");
		
		setShift (rhs);  // Algorithm assumes they're centered on each other
        int xMagn = (int)Math.Abs(dir.x);
        int yMagn = (int)Math.Abs(dir.y);
        int xMove, yMove;
		Bounding bound = GetBounding();
		Bounding rhsBound = rhs.GetBounding();
        float magRatio;
        // Find which has largest magnitude and move fully that direction first
        if (xMagn > yMagn)
        { // X larger magnitude
            xMove = bound.width / 2 + rhsBound.width / 2 + 1;
            magRatio = yMagn == 0 ? 0 : ((float)yMagn) / xMagn;
            yMove = (int)(xMove * magRatio);
        } 
        else
        { // Y larger magnitude
            yMove = bound.height / 2 + rhsBound.height / 2 + 1;
            magRatio = xMagn == 0 ? 0 : ((float)xMagn) / yMagn;
            xMove = (int)(yMove * magRatio);
        }

        // Execute shift to the outside, adjusting to give right direction
        xMove = xMove * Math.Sign(dir.x);
        yMove = yMove * Math.Sign(dir.y);
        shift(xMove, yMove);

        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
			DebugManager.w(DebugManager.Logs.LevelGen, "Bounds1: " + bound + " Bounds2: " + rhsBound);
            DebugManager.w(DebugManager.Logs.LevelGen, "XMag: " + xMagn + " YMag: " + yMagn + " magRatio: " + magRatio + " xMove: " + xMove + " yMove: " + yMove);
        }
        DebugManager.printFooter(DebugManager.Logs.LevelGen);
    }
    #endregion

    #region Bounds
    public Bounding GetBounding()
    {
        Bounding bound = new Bounding(GetBoundingInternal());
        bound.xMin += shiftP.x;
        bound.xMax += shiftP.x;
        bound.yMin += shiftP.y;
        bound.yMax += shiftP.y;
        return bound;
    }

    protected abstract Bounding GetBoundingInternal();
    #endregion Bounds

    #region GetSet
    public abstract GridArray GetArray();

    public abstract GridArray GetBakedArray();

    public GridType[,] GetMinimizedArray()
    {
        GridArray inArr = GetArray();
        Bounding bounds = GetBoundingInternal();
        GridType[,] outArr = new GridType[bounds.height + 1, bounds.width + 1];
        for (int y = bounds.yMin; y <= bounds.yMax; y++)
        {
            for (int x = bounds.xMin; x <= bounds.xMax; x++)
            {
                outArr[y - bounds.yMin, x - bounds.xMin] = inArr.Get (x, y);
            }
        }
        return outArr;
    }
    #endregion GetSet

    #region SpecGet
    public GridMap getType(GridType t)
    {
        return getType(GetArray(), t);
    }

    public GridMap getType(GridArray grids, GridType t)
    {
        GridMap ret = new GridMap();
        foreach (Value2D<GridType> val in grids)
        {
            if (t == val.val)
            {
                val.x += shiftP.x;
                val.y += shiftP.y;
                ret.Put(val);
            }
        }
        return ret;
    }

    public GridMap getTypes(params GridType[] ts)
    {
        return getTypes(new HashSet<GridType>(ts));
    }

    public GridMap getTypes(HashSet<GridType> ts)
    {
        return getTypes(GetArray(), ts);
    }

    public GridMap getTypes(GridArray grids, params GridType[] ts)
    {
        return getTypes(grids, new HashSet<GridType>(ts));
    }

    public GridMap getTypes(GridArray grids, HashSet<GridType> ts)
    {
        GridMap ret = new GridMap();
        foreach (Value2D<GridType> val in grids)
        {
            if (ts.Contains(val.val))
            {
                val.x += shiftP.x;
                val.y += shiftP.y;
                ret.Put(val);
            }
        }
        return ret;
    }

    public GridMap getCorneredBy(GridType target, params GridType[] by)
    {
        return getCorneredBy(target, new HashSet<GridType>(by));
    }

    public GridMap getCorneredBy(GridType target, HashSet<GridType> by)
    {
        GridMap ret = new GridMap();
        GridArray grids = GetArray();
        GridMap targets = getType(grids, target);
        GridMap cornerOptions = getTypes(grids, by);
        foreach (Value2D<GridType> tval in targets)
        {
            bool corneredHoriz = cornerOptions.Contains(tval.x + 1, tval.y)
                || cornerOptions.Contains(tval.x - 1, tval.y);
            bool corneredVert = cornerOptions.Contains(tval.x, tval.y + 1)
                || cornerOptions.Contains(tval.x, tval.y - 1);
            if (corneredHoriz && corneredVert)
            {
                ret.Put(tval);
            }
        }
        return ret;
    }
    #endregion

    #region Intersects
    public bool intersects(LayoutObject rhs)
    {
        return GetBounding().intersects(rhs.GetBounding());
    }

    public bool intersects(List<LayoutObject> list)
    {
		return null != intersectObj(list);
    }
	
	public LayoutObject intersectObj(List<LayoutObject> list)
	{
        foreach (LayoutObject rhs in list)
        {
            if (intersects(rhs))
            {
                return rhs;
            }
        }
        return null;
	}
    #endregion Intersects

    #region Printing
    public override string ToString() {
		return "Layout Object";
	}
	
    protected string printContent()
    {
        string ret = "";
        foreach (string s in ToRowStrings())
        {
            ret += s + "\n";
        }
        return ret;
    }

    protected virtual List<string> ToRowStrings()
    {
		GridType[,] array = GetMinimizedArray();
        List<string> ret = new List<string>();
		for (int y = array.GetLength(0) - 1; y >= 0; y -= 1) {
            string rowStr = "";
    		for (int x = 0; x < array.GetLength(1); x += 1) {
        		rowStr += getAscii(array[y,x]);
    		}
            ret.Add(rowStr);
		}
        return ret;
    }

    public static char getAscii(GridType type)
    {
        switch (type)
        {
            case GridType.Floor:
                return '.';
            case GridType.PathFloor:
                return '-';
            case GridType.TrapDoor:
                return 'T';
            case GridType.Door:
                return '|';
            case GridType.Wall:
                return '#';
            case GridType.NULL:
                return ' ';
            case GridType.INTERNAL_RESERVED_REJECTED:
                return '<';
            case GridType.INTERNAL_RESERVED_ACCEPTED:
                return '>';
            case GridType.INTERNAL_RESERVED_BLOCKED:
                return '*';
            case GridType.INTERNAL_RESERVED_CUR:
                return '%';
            case GridType.INTERNAL_RESERVED_HORIZ:
                return (char) 205;
            case GridType.INTERNAL_RESERVED_VERT:
                return (char) 186;
            case GridType.INTERNAL_RESERVED_LT:
                return (char) 188;
            case GridType.INTERNAL_RESERVED_LB:
                return (char) 187;
            case GridType.INTERNAL_RESERVED_RT:
                return (char) 200;
            case GridType.INTERNAL_RESERVED_RB:
                return (char) 201;
            default:
                return '?';
        }
    }

    public virtual void toLog(DebugManager.Logs log, params String[] customContent)
    {
        if (DebugManager.logging(log))
        {
            DebugManager.printHeader(log, ToString());
            foreach (String s in customContent)
            {
                DebugManager.w(log, s);
            }
            foreach (string s in ToRowStrings())
            {
                DebugManager.w(log, s);
            }
            DebugManager.w(log, "Bounds: " + GetBounding().ToString());
			DebugManager.printFooter(log);
        }
    }

    public virtual void toLog(DebugManager.Logs log)
    {
        if (DebugManager.logging(log))
        {
            toLog(log, new String[0]);
        }
    }
    #endregion Printing

}
