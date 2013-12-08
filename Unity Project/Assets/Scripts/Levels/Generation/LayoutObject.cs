using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

abstract public class LayoutObject
{
    protected bool finalized = false;
    protected Bounding bakedBounds = new Bounding();
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
        ShiftP.Shift(x, y);
    }

    public void shift(Point p)
    {
        ShiftP.Shift(p);
    }

    public void setShift(LayoutObject rhs)
    {
        Bounding bounds = GetBoundingUnshifted();
        Bounding rhsBounds = rhs.GetBoundingUnshifted();
        Point center = bounds.GetCenter();
        Point centerRhs = rhsBounds.GetCenter();
        ShiftP.x = rhs.ShiftP.x + (centerRhs.x - center.x);
        ShiftP.y = rhs.ShiftP.y + (centerRhs.y - center.y);
    }

    public Value2D<GridType> ShiftValue(Value2D<GridType> val)
    {
        return new Value2D<GridType>(val.x + ShiftP.x, val.y + ShiftP.y, val.val);
    }

    public Point GetShift()
    {
        return new Point(ShiftP);
    }

    public void ShiftOutside(LayoutObject rhs, Point dir, bool rough, bool finalShift)
    {
        Point reducBase = dir.reduce();
        Point reduc = new Point(reducBase);
        int xShift, yShift;
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Shift Outside " + ToString());
            BigBoss.Debug.w(Logs.LevelGen, "Shifting outside of " + rhs.ToString());
            BigBoss.Debug.w(Logs.LevelGen, "Shift " + dir + "   Reduc shift: " + reduc);
            BigBoss.Debug.w(Logs.LevelGen, "Bounds: " + GetBounding(true) + "  RHS bounds: " + rhs.GetBounding(true));
        }
        #endregion
        while (this.IntersectsSmart(rhs))
        {
            if (rough)
            {
                shift(reduc);
            }
            else
            {
                reduc.Take(out xShift, out yShift);
                shift(xShift, yShift);
                if (reduc.isZero())
                {
                    reduc = new Point(reducBase);
                }
            }
        }
        if (finalShift)
            shift(dir);
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printFooter(Logs.LevelGen);
        }
        #endregion
    }
    #endregion Shifts

    #region Bounds
    public Bounding GetBounding(bool shifted)
    {
        Bounding bound = new Bounding(GetBoundingUnshifted());
        if (shifted)
            adjustBounding(bound, true);
        return bound;
    }

    protected void adjustBounding(Bounding bound, bool toExternal)
    {
        if (toExternal)
        {
            bound.Shift(ShiftP);
        }
        else
        {
            bound.Shift(ShiftP.Invert());
        }
    }

    protected abstract Bounding GetBoundingUnshifted();
    #endregion Bounds

    #region GetSet
    public abstract GridArray GetArray();

    public virtual GridArray GetPrintArray()
    {
        return GetArray();
    }

    public virtual GridType[,] GetMinimizedArray(GridArray inArr)
    {
        Bounding bounds = GetBoundingUnshifted();
        GridType[,] outArr = new GridType[bounds.Height + 1, bounds.Width + 1];
        for (int y = bounds.YMin; y <= bounds.YMax; y++)
        {
            for (int x = bounds.XMin; x <= bounds.XMax; x++)
            {
                outArr[y - bounds.YMin, x - bounds.XMin] = inArr[x, y];
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
        GridType[,] arr = grids;
        for (int y = 0; y < arr.GetLength(0); y++)
        {
            for (int x = 0; x < arr.GetLength(1); x++)
            {
                if (t == arr[y, x])
                    ret[x + ShiftP.x, y + ShiftP.y] = arr[y, x];
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
        GridType[,] arr = grids;
        for (int y = 0; y < arr.GetLength(0); y++)
        {
            for (int x = 0; x < arr.GetLength(1); x++)
            {
                if (ts.Contains(arr[y, x]))
                    ret[x + ShiftP.x, y + ShiftP.y] = arr[y, x];
            }
        }
        return ret;
    }

    public GridMap GetTouchingNull(GridType target)
    {
        GridMap ret = new GridMap();
        GridArray grids = GetArray();
        GridType[,] arr = grids.GetArr();
        GridMap targets = getType(grids, target);
        foreach (Value2D<GridType> val in targets)
        {
            Value2D<GridType> nullDir;
            if (arr.GetPointAround(val.x, val.y, false, Draw.EqualTo(GridType.NULL), out nullDir))
                ret[val] = val.val;
        }
        return ret;
    }

    public GridMap GetCorneredBy(GridType target, params GridType[] by)
    {
        return GetCorneredBy(target, new HashSet<GridType>(by));
    }

    public GridMap GetCorneredBy(GridType target, HashSet<GridType> by)
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

    public Value2D<GridType> ScanForFirst(GridType type)
    {
        foreach (Value2D<GridType> val in GetArray())
        {
            if (val.val == type)
            {
                return val;
            }
        }
        return null;
    }
    #endregion

    public virtual void Bake(bool shiftCompensate)
    {
        bakedBounds = GetBounding(true);
        finalized = true;
    }

    public Bounding GetConnectedBounds()
    {
        List<LayoutObject> connected;
        Bounding bounds;
        ConnectedToAll(out connected, out bounds);
        return bounds;
    }

    public GridArray GetConnectedGrid()
    {
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen) && BigBoss.Debug.Flag(DebugManager.DebugFlag.LevelGen_Connected_To))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Get Connected Grid " + this);
        }
        #endregion
        List<LayoutObject> connected;
        Bounding bounds;
        ConnectedToAll(out connected, out bounds);
        var arrOut = new GridArray(bounds, false);
        foreach (var obj in connected)
        {
            arrOut.PutAll(obj);
        }
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen) && BigBoss.Debug.Flag(DebugManager.DebugFlag.LevelGen_Connected_To))
        {
            BigBoss.Debug.printFooter(Logs.LevelGen);
        }
        #endregion
        return arrOut;
    }

    public void Connect(LayoutObject obj)
    {
        if (obj != null && isValid() && obj.isValid())
        {
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                BigBoss.Debug.w(Logs.LevelGen, "Connecting " + ToString() + " to " + obj.ToString());
            }
            _connectedTo.Add(obj);
            obj._connectedTo.Add(this);
        }
    }

    public void Connect(LayoutObjectContainer layout, Value2D<GridType> pt)
    {
        Connect(layout.GetObjAt(pt));
    }

    abstract public bool ContainsPoint(Value2D<GridType> val);

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
        if (BigBoss.Debug.logging(Logs.LevelGen) && BigBoss.Debug.Flag(DebugManager.DebugFlag.LevelGen_Connected_To))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Connected To Recursive: " + this);
            BigBoss.Debug.w(Logs.LevelGen, "Connected to:");
            foreach (var connected in _connectedTo)
            {
                BigBoss.Debug.w(Logs.LevelGen, 1, connected.ToString());
            }
        }
        #endregion
        list.Add(this);
        if (bounds != null)
        {
            bounds.absorb(GetBounding(true));
        }
        foreach (var connected in _connectedTo)
        {
            if (!list.Contains(connected))
            {
                connected.ConnectedToRecursive(list, bounds);
            }
        }
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen) && BigBoss.Debug.Flag(DebugManager.DebugFlag.LevelGen_Connected_To))
        {
            BigBoss.Debug.printFooter(Logs.LevelGen);
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
        if (BigBoss.Debug.logging(Logs.LevelGen) && BigBoss.Debug.Flag(DebugManager.DebugFlag.LevelGen_Connected_To))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Connected To");
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
        if (BigBoss.Debug.logging(Logs.LevelGen) && BigBoss.Debug.Flag(DebugManager.DebugFlag.LevelGen_Connected_To))
        {
            BigBoss.Debug.printFooter(Logs.LevelGen);
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
        var visited = new HashSet<LayoutObject> { this };
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
    public bool IntersectsSmart(LayoutObject rhs)
    {
        // Get Arrays
        GridType[,] arr = GetArray().GetArr();
        GridType[,] rhsArr = rhs.GetArray().GetArr();

        // Get Limits
        Bounding bounds = GetBounding(true);
        Bounding rhsBounds = rhs.GetBounding(true);
        Bounding intersect = bounds.IntersectBounds(rhsBounds);

        for (int y = intersect.YMin; y < intersect.YMax; y++)
        {
            for (int x = intersect.XMin; x < intersect.XMax; x++)
            {
                if (arr[y - ShiftP.y, x - ShiftP.x] != GridType.NULL
                    && rhsArr[y - rhs.ShiftP.y, x - rhs.ShiftP.x] != GridType.NULL)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public LayoutObject IntersectSmart<T>(IEnumerable<T> list) where T : LayoutObject
    {
        foreach (LayoutObject rhs in list)
        {
            if (IntersectsSmart(rhs))
            {
                return rhs;
            }
        }
        return null;
    }

    public bool IntersectsBounds(LayoutObject rhs, int buffer)
    {
        Bounding rhsBound = rhs.GetBounding(true);
        if (rhsBound.IsValid())
        {
            rhsBound.expand(buffer);
            return GetBounding(true).Intersects(rhsBound);
        }
        return false;
    }

    public bool InsersectsBounds(List<LayoutObject> list, int buffer)
    {
        return null != IntersectObjBounds(list, buffer);
    }

    public LayoutObject IntersectObjBounds<T>(IEnumerable<T> list, int buffer) where T : LayoutObject
    {
        foreach (LayoutObject rhs in list)
        {
            if (IntersectsBounds(rhs, buffer))
            {
                return rhs;
            }
        }
        return null;
    }
    #endregion Intersects

    #region Printing
    public override string ToString()
    {
        return GetTypeString() + " " + Id;
    }

    public abstract string GetTypeString();

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
        return ToRowStrings(GetBounding(true));
    }

    protected virtual List<string> ToRowStrings(Bounding bounds)
    {
        GridType[,] array = GetMinimizedArray(GetPrintArray());
        List<string> ret = array.ToRowStrings();
        ret = Nifty.AddRuler(ret, bounds);
        return ret;
    }

    public virtual void ToLog(Logs log, params String[] customContent)
    {
        if (BigBoss.Debug.logging(log))
        {
            BigBoss.Debug.printHeader(log, ToString());
            foreach (String s in customContent)
            {
                BigBoss.Debug.w(log, s);
            }
            foreach (string s in ToRowStrings())
            {
                BigBoss.Debug.w(log, s);
            }
            Bounding bounds = GetBounding(true);
            BigBoss.Debug.w(log, "Bounds Shifted: " + bounds.ToString());
            bounds.Shift(ShiftP.Invert());
            BigBoss.Debug.w(log, "Bounds: " + bounds.ToString());
            BigBoss.Debug.printFooter(log);
        }
    }

    public virtual void ToLog(Logs log)
    {
        if (BigBoss.Debug.logging(log))
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
        return Equals((LayoutObject)obj);
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
