using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

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
			DebugManager.w (DebugManager.Logs.LevelGen, "Bounds: " + getBounds() + "  RHS bounds: " + rhs.getBounds());
		}
		#endregion
		while(this.intersects(rhs))
		{
            // Shift small increments until not overlapping
            shift(reduc);
            #region DEBUG
            if (DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                DebugManager.w(DebugManager.Logs.LevelGen, "Shifted to: " + getBounds());
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
		Bounding bound = getBounds();
		Bounding rhsBound = rhs.getBounds();
        float magRatio;
        // Find which has largest magnitude and move fully that direction first
        if (xMagn > yMagn)
        { // X larger magnitude
            xMove = bound.width() / 2 + rhsBound.width() / 2 + 1;
            magRatio = yMagn == 0 ? 0 : ((float)yMagn) / xMagn;
            yMove = (int)(xMove * magRatio);
        } 
        else
        { // Y larger magnitude
            yMove = bound.height() / 2 + rhsBound.height() / 2 + 1;
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
    public Bounding getBounds()
    {
        Bounding bound = new Bounding(getBoundsInternal());
        bound.xMin += shiftP.x;
        bound.xMax += shiftP.x;
        bound.yMin += shiftP.y;
        bound.yMax += shiftP.y;
        return bound;
    }

    protected abstract Bounding getBoundsInternal();
    #endregion Bounds

    #region GetSet
    public abstract GridMap getMap();

    public abstract GridMap getBakedMap();
	
    public GridType[,] getArr()
    {
		GridMap map = getMap();
		Bounding bound = getBounds();
		GridType[,] arr = new GridType[bound.height() + 1, bound.width() + 1];
		foreach (KeyValuePair<int, SortedDictionary<int, GridType>> row in map)
		{
			foreach (KeyValuePair<int, GridType> val in row.Value)
			{
				arr[row.Key - bound.yMin, val.Key - bound.xMin] = val.Value;
			}
		}
        return arr;
    }
    #endregion GetSet

    #region Intersects
    public bool intersects(LayoutObject rhs)
    {
        return getBounds().intersects(rhs.getBounds());
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
		GridType[,] array = getArr ();
        List<string> ret = new List<string>();
		for (int y = 0; y < array.GetLength(0); y += 1) {
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
            case GridType.TrapDoor:
                return 'T';
            case GridType.Door:
                return '|';
            case GridType.Wall:
                return '#';
            case GridType.NULL:
                return ' ';
            default:
                return '?';
        }
    }

    public virtual void toLog(DebugManager.Logs log)
    {
        if (DebugManager.logging(log))
        {
			DebugManager.printHeader(log, ToString()); 
            foreach (string s in ToRowStrings())
            {
                DebugManager.w(log, s);
            }
            DebugManager.w(log, "Bounds: " + getBounds().ToString());
			DebugManager.printFooter(log);
        }
    }
    #endregion Printing
}
