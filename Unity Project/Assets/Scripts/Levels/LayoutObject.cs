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
        if (BigBoss.Debug.logging(DebugManager.Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(DebugManager.Logs.LevelGen, "Shift Outside " + ToString());
            BigBoss.Debug.w(DebugManager.Logs.LevelGen, "Shifting outside of " + rhs.ToString());
            BigBoss.Debug.w(DebugManager.Logs.LevelGen, "Shift " + dir + "   Reduc shift: " + reduc);
            BigBoss.Debug.w(DebugManager.Logs.LevelGen, "Bounds: " + GetBounding() + "  RHS bounds: " + rhs.GetBounding());
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
        if (BigBoss.Debug.logging(DebugManager.Logs.LevelGen))
        {
            BigBoss.Debug.printFooter(DebugManager.Logs.LevelGen);
        }
        #endregion
    }
    #endregion Shifts

    #region Bounds
    public Bounding GetBounding()
    {
        Bounding bound = new Bounding(GetBoundingUnshifted());
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

    public GridMap GetTouchingNull(GridType target)
    {
        GridMap ret = new GridMap();
        GridArray grids = GetArray();
        Surrounding<GridType> surround = new Surrounding<GridType>(grids);
        GridMap targets = getType(grids, target);
        foreach (Value2D<GridType> val in targets)
        {
            surround.Load(val);
            Value2D<GridType> nullDir = surround.GetDirWithVal(true, GridType.NULL);
            if (nullDir != null)
            {
                ret[val] = val.val;
            }
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

    public GridMap GetBfsPerimeter()
    {
        GridArray grids = GetArray();
        GridMap ret = new GridMap();
        // Get null spaces surrounding room
        BFSSearcher searcher = new BFSSearcher(Probability.LevelRand);
        Array2D<bool> bfs = searcher.SearchFill(new Value2D<GridType>(), grids, GridType.NULL);
        // Invert to be room
        Array2D<bool>.invert(bfs);
        Surrounding<bool> surround = new Surrounding<bool>(bfs.GetArr());
        foreach (Value2D<bool> val in bfs)
        {
            // If space part of room
            if (val.val)
            {
                surround.Load(val);
                // If space is an edge (next to a false)
                if (surround.GetDirWithVal(true, false) != null)
                {
                    ret[val.x, val.y] = grids[val.x, val.y];
                }
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
        bakedBounds = GetBounding();
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
        if (BigBoss.Debug.logging(DebugManager.Logs.LevelGen) && BigBoss.Debug.Flag(DebugManager.DebugFlag.LevelGen_Connected_To))
        {
            BigBoss.Debug.printHeader(DebugManager.Logs.LevelGen, "Get Connected Grid " + this);
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
        if (BigBoss.Debug.logging(DebugManager.Logs.LevelGen) && BigBoss.Debug.Flag(DebugManager.DebugFlag.LevelGen_Connected_To))
        {
            BigBoss.Debug.printFooter(DebugManager.Logs.LevelGen);
        }
        #endregion
        return arrOut;
    }

    public void Connect(LayoutObject obj)
    {
        if (obj != null && isValid() && obj.isValid())
        {
            if (BigBoss.Debug.logging(DebugManager.Logs.LevelGen))
            {
                BigBoss.Debug.w(DebugManager.Logs.LevelGen, "Connecting " + ToString() + " to " + obj.ToString());
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
        if (BigBoss.Debug.logging(DebugManager.Logs.LevelGen) && BigBoss.Debug.Flag(DebugManager.DebugFlag.LevelGen_Connected_To))
        {
            BigBoss.Debug.printHeader(DebugManager.Logs.LevelGen, "Connected To Recursive: " + this);
            BigBoss.Debug.w(DebugManager.Logs.LevelGen, "Connected to:");
            foreach (var connected in _connectedTo)
            {
                BigBoss.Debug.w(DebugManager.Logs.LevelGen, 1, connected.ToString());
            }
        }
        #endregion
        list.Add(this);
        if (bounds != null)
        {
            bounds.absorb(GetBounding());
        }
        foreach (var connected in _connectedTo)
        {
            if (!list.Contains(connected))
            {
                connected.ConnectedToRecursive(list, bounds);
            }
        }
        #region DEBUG
        if (BigBoss.Debug.logging(DebugManager.Logs.LevelGen) && BigBoss.Debug.Flag(DebugManager.DebugFlag.LevelGen_Connected_To))
        {
            BigBoss.Debug.printFooter(DebugManager.Logs.LevelGen);
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
        if (BigBoss.Debug.logging(DebugManager.Logs.LevelGen) && BigBoss.Debug.Flag(DebugManager.DebugFlag.LevelGen_Connected_To))
        {
            BigBoss.Debug.printHeader(DebugManager.Logs.LevelGen, "Connected To");
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
        if (BigBoss.Debug.logging(DebugManager.Logs.LevelGen) && BigBoss.Debug.Flag(DebugManager.DebugFlag.LevelGen_Connected_To))
        {
            BigBoss.Debug.printFooter(DebugManager.Logs.LevelGen);
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
        Bounding bounds = GetBounding();
        Bounding rhsBounds = rhs.GetBounding();
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
        Bounding rhsBound = rhs.GetBounding();
        if (rhsBound.IsValid())
        {
            rhsBound.expand(buffer);
            return GetBounding().Intersects(rhsBound);
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
        return ToRowStrings(GetBounding());
    }

    protected virtual List<string> ToRowStrings(Bounding bounds)
    {
        GridType[,] array = GetMinimizedArray(GetPrintArray());
        List<string> ret = new List<string>();
        for (int y = array.GetLength(0) - 1; y >= 0; y -= 1)
        {
            string rowStr = "";
            for (int x = 0; x < array.GetLength(1); x += 1)
            {
                rowStr += getAscii(array[y, x]);
            }
            ret.Add(rowStr);
        }
        ret = Nifty.AddRuler(ret, bounds);
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
                return (char)205;
            case GridType.Path_Vert:
                return (char)186;
            case GridType.Path_LT:
                return (char)188;
            case GridType.Path_LB:
                return (char)187;
            case GridType.Path_RT:
                return (char)200;
            case GridType.Path_RB:
                return (char)201;
            default:
                return '?';
        }
    }

    public virtual void ToLog(DebugManager.Logs log, params String[] customContent)
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
            Bounding bounds = GetBounding();
            BigBoss.Debug.w(log, "Bounds Shifted: " + bounds.ToString());
            bounds.Shift(ShiftP.Invert());
            BigBoss.Debug.w(log, "Bounds: " + bounds.ToString());
            BigBoss.Debug.printFooter(log);
        }
    }

    public virtual void ToLog(DebugManager.Logs log)
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
