using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

abstract public class LayoutObject {

    protected Point ShiftP = new Point();
    readonly HashSet<LayoutObject> _connectedTo = new HashSet<LayoutObject>();
    private static int _nextId = 0;
    public int Id { get; protected set; }

    protected LayoutObject()
    {
        Id = _nextId++;
    }

    #region Shifts
    public void shift(int x, int y)
	{
		ShiftP.shift(x,y);
	}
	
	public void shift(Point p)
	{
		ShiftP.shift(p);	
	}

    public void setShift(LayoutObject rhs)
    {
		Bounding bounds = GetBoundingInternal();
		Bounding rhsBounds = rhs.GetBoundingInternal();
		Point center = bounds.getCenter();
		Point centerRhs = rhsBounds.getCenter();
        ShiftP.x = rhs.ShiftP.x + (centerRhs.x - center.x);
        ShiftP.y = rhs.ShiftP.y + (centerRhs.y - center.y);
    }

    public Value2D<GridType> Shift(Value2D<GridType> val)
    {
        return new Value2D<GridType>(val.x + ShiftP.x, val.y + ShiftP.y, val.val);
    }
	
	public Point GetShift()
	{
		return new Point(ShiftP);	
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
		while(this.intersects(rhs, 0))
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

    #region Bounds
    public Bounding GetBounding()
    {
        Bounding bound = new Bounding(GetBoundingInternal());
        adjustBounding(bound, true);
        return bound;
    }

    protected void adjustBounding(Bounding bound, bool toExternal)
    {
        if (toExternal)
        {
            bound.xMin += ShiftP.x;
            bound.xMax += ShiftP.x;
            bound.yMin += ShiftP.y;
            bound.yMax += ShiftP.y;
        }
        else
        {
            bound.xMin -= ShiftP.x;
            bound.xMax -= ShiftP.x;
            bound.yMin -= ShiftP.y;
            bound.yMax -= ShiftP.y;
        }
    }

    protected abstract Bounding GetBoundingInternal();
    #endregion Bounds

    #region GetSet
    public abstract GridArray GetArray();

    public virtual GridArray GetPrintArray()
    {
        return GetArray();
    }

    public virtual GridType[,] GetMinimizedArray(GridArray inArr)
    {
        Bounding bounds = GetBoundingInternal();
        GridType[,] outArr = new GridType[bounds.height + 1, bounds.width + 1];
        for (int y = bounds.yMin; y <= bounds.yMax; y++)
        {
            for (int x = bounds.xMin; x <= bounds.xMax; x++)
            {
                outArr[y - bounds.yMin, x - bounds.xMin] = inArr[x, y];
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

    public GridArray GetConnectedGrid()
    {
        List<LayoutObject> connected;
        Bounding bounds;
        ConnectedToAll(out connected, out bounds);
        var arrOut = new GridArray(bounds, false);
        foreach (var obj in connected)
        {
            arrOut.PutAll(obj.GetArray());
        }
        return arrOut;
    }

    public void Connect(LayoutObject obj)
    {
        if (obj != null && isValid() && obj.isValid())
        {
            if (DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                DebugManager.w(DebugManager.Logs.LevelGen, "Connecting " + ToString() + " to " + obj.ToString());
            }
            _connectedTo.Add(obj);
            obj._connectedTo.Add(this);
        }
    }

    public void Connect(LayoutObjectContainer layout, Value2D<GridType> pt)
    {
        Connect(layout.GetObjAt(pt));
    }

    abstract public bool Contains(Value2D<GridType> val);

    public void ConnectedToAll(out List<LayoutObject> connected, out Bounding bounds)
    {
        connected = new List<LayoutObject>();
        bounds = new Bounding();
        ConnectedToRecursive(connected, bounds);
    }

    public List<LayoutObject> ConnectedToAll()
    {
        var connected = new List<LayoutObject>();
        ConnectedToRecursive(connected, null);
        return connected;
    }

    void ConnectedToRecursive(List<LayoutObject> list, Bounding bounds)
    {
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printHeader(DebugManager.Logs.LevelGen, "Connected To Recursive: " + this);
            DebugManager.w(DebugManager.Logs.LevelGen, "Connected to:");
            foreach (var connected in _connectedTo)
            {
                DebugManager.w(DebugManager.Logs.LevelGen, 1, connected.ToString());
            }
        }
        #endregion
        foreach (var connected in _connectedTo)
        {
            if (!list.Contains(connected))
            {

                list.Add(connected);
                DebugManager.w(DebugManager.Logs.LevelGen, "Added " + connected);
                if (bounds != null)
                {
                    bounds.absorb(connected.GetBounding());
                }
                connected.ConnectedToRecursive(list, bounds);
            }
        }
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printFooter(DebugManager.Logs.LevelGen);
        }
        #endregion
    }

    public virtual bool isValid()
    {
        return true;
    }

    public bool ConnectedTo(IEnumerable<LayoutObject> roomsToConnect, out LayoutObject failObj)
    {
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printHeader(DebugManager.Logs.LevelGen, "Connected To");
        }
        #endregion
        failObj = null;
        var connected = ConnectedToAll();
        foreach (var obj in roomsToConnect)
        {
            if (!connected.Contains(obj))
            {
                failObj = obj;
                break;
            }
        }
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printFooter(DebugManager.Logs.LevelGen);
        }
        #endregion
        return failObj == null;
    }

    public bool ConnectedTo(LayoutObject obj)
    {
        List<LayoutObject> list;
        return GetPathTo(obj, out list);
    }

    public bool GetPathTo(LayoutObject target, out List<LayoutObject> list)
    {
        var visited = new HashSet<LayoutObject> {this};
        return GetPathToRecursive(this, target, visited, out list);
    }

    bool GetPathToRecursive(LayoutObject cur, LayoutObject target, HashSet<LayoutObject> visited, out List<LayoutObject> list)
    {
        list = new List<LayoutObject>();
        if (_connectedTo.Contains(target))
        {
            list.Add(target);
            return true;
        }
        // Recursively search
        foreach (var connected in _connectedTo)
        {
            if (!visited.Contains(connected))
            {
                List<LayoutObject> targetPath;
                visited.Add(connected);
                if (GetPathToRecursive(connected, target, visited, out targetPath))
                {
                    list.AddRange(targetPath);
                    return true;
                }
            }
        }
        return false;
    }

    #region Intersects
    public bool intersects(LayoutObject rhs, int buffer)
    {
        Bounding rhsBound = rhs.GetBounding();
        rhsBound.expand(buffer);
        return GetBounding().intersects(rhsBound);
    }

    public bool intersects(List<LayoutObject> list, int buffer)
    {
		return null != intersectObj(list, buffer);
    }
	
	public LayoutObject intersectObj(List<LayoutObject> list, int buffer)
	{
        foreach (LayoutObject rhs in list)
        {
            if (intersects(rhs, buffer))
            {
                return rhs;
            }
        }
        return null;
	}
    #endregion Intersects

    #region Printing
    public override string ToString() {
		return GetTypeString() + " " + Id;
	}

    public abstract String GetTypeString();
	
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
		GridType[,] array = GetMinimizedArray(GetPrintArray());
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
            case GridType.TrapDoor:
                return 'T';
            case GridType.Door:
                return '|';
            case GridType.Wall:
                return '#';
            case GridType.NULL:
                return ' ';
            case GridType.INTERNAL_RESERVED_BLOCKED:
                return '*';
            case GridType.INTERNAL_RESERVED_CUR:
                return '%';
            case GridType.Path_Horiz:
                return (char) 205;
            case GridType.Path_Vert:
                return (char) 186;
            case GridType.Path_LT:
                return (char) 188;
            case GridType.Path_LB:
                return (char) 187;
            case GridType.Path_RT:
                return (char) 200;
            case GridType.Path_RB:
                return (char) 201;
            default:
                return '?';
        }
    }

    public virtual void ToLog(DebugManager.Logs log, params String[] customContent)
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

    public virtual void ToLog(DebugManager.Logs log)
    {
        if (DebugManager.logging(log))
        {
            ToLog(log, new String[0]);
        }
    }
    #endregion Printing

    protected bool Equals(LayoutObject other)
    {
        return Id == other.Id;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((LayoutObject) obj);
    }

    public override int GetHashCode()
    {
        return Id;
    }

    public static bool operator ==(LayoutObject left, LayoutObject right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(LayoutObject left, LayoutObject right)
    {
        return !Equals(left, right);
    }
}
