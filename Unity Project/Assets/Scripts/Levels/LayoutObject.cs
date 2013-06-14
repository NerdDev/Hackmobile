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
		Bounding bound = new Bounding(0, 5, 0, 16);
		GridType[,] arr = new GridType[bound.height(), bound.width()];
		for (int y = bound.yMin, yarr = 0; y < bound.yMax; y++, yarr++)
        {
            SortedDictionary<int, GridType> row = null;
            map.getRow(y, out row);
            GridType t;
            for (int x = bound.xMin, xarr = 0; x < bound.xMax; x++, xarr++)
            {
                if (row != null && row.TryGetValue(x, out t))
                {
                    arr[xarr, yarr] = t;
                }
            }
        }
			
		int wer = 23;
			
//        GridMap map = getMap();
//        Bounding bound = getBoundsInternal();
//        GridType[,] arr = new GridType[bound.width(), bound.height()];
//		int xarr = 0;
//		int yarr = 0;
//        for (int y = bound.yMin; y <= bound.yMax; y++)
//        {
//            SortedDictionary<int, GridType> row = null;
//            map.getRow(y, out row);
//            GridType t;
//            for (int x = bound.xMin; x <= bound.xMax; x++)
//            {
//                arr[xarr, yarr] = GridType.NULL;
//                if (row != null && row.TryGetValue(x, out t))
//                {
//                    arr[xarr, yarr] = t;
//                }
//				xarr++;
//            }
//			
//        }
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
        GridMap grids = getMap();
        List<string> ret = new List<string>();
		Bounding bound = getBounds ();
        for (int y = bound.yMin; y <= bound.yMax; y++)
        {
            SortedDictionary<int, GridType> row = null;
            grids.getRow(y, out row);
            string rowStr = "";
            GridType t;
            for (int x = bound.xMin; x <= bound.xMax; x++)
            {
                if (row != null && row.TryGetValue(x, out t))
                {
                    rowStr += getAscii(t);
                }
                else
                {
                    rowStr += " ";
                }
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
