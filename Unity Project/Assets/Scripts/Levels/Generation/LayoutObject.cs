using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

abstract public class LayoutObject
{
    public Point ShiftP { get; protected set; }
    readonly HashSet<LayoutObject> _connectedTo = new HashSet<LayoutObject>();
    private static int _nextId = 0;
    public abstract Container2D<GridType> Grids { get; protected set; }
    public virtual Bounding Bounding
    {
        get
        {
            Bounding bound = Grids.Bounding;
            AdjustBounding(bound, true);
            return bound;
        }
    }
    public int Id { get; protected set; }

    protected LayoutObject()
    {
        Id = _nextId++;
        ShiftP = new Point();
    }

    #region Shifts
    public void Shift(int x, int y)
    {
        ShiftP.Shift(x, y);
    }

    public void Shift(Point p)
    {
        ShiftP.Shift(p);
    }

    public virtual void Bake()
    {
    }

    public void CenterOn(LayoutObject rhs)
    {
        Point center = Bounding.GetCenter();
        Point centerRhs = Bounding.GetCenter();
        ShiftP.x = rhs.ShiftP.x + (centerRhs.x - center.x);
        ShiftP.y = rhs.ShiftP.y + (centerRhs.y - center.y);
    }

    public void ShiftOutside(IEnumerable<LayoutObject> rhs, Point dir)
    {
        List<Container2D<GridType>> list = new List<Container2D<GridType>>();
        foreach (LayoutObject obj in rhs)
            list.Add(obj.Grids);
        Container2D<GridType> intersect;
        while (Grids.Intersects(list, out intersect))
        {
            ShiftOutside(intersect, dir, true, true);
        }
    }

    public void ShiftOutside(Container2D<GridType> rhs, Point dir, bool rough, bool finalShift)
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
            BigBoss.Debug.w(Logs.LevelGen, "Bounds: " + Bounding + "  RHS bounds: " + rhs.Bounding);
        }
        #endregion
        while (Grids.Intersects(rhs))
        {
            if (rough)
            {
                Shift(reduc);
            }
            else
            {
                reduc.Take(out xShift, out yShift);
                Shift(xShift, yShift);
                if (reduc.isZero())
                {
                    reduc = new Point(reducBase);
                }
            }
        }
        if (finalShift)
            Shift(dir);
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printFooter(Logs.LevelGen, "Shift Outside " + ToString());
        }
        #endregion
    }
    #endregion Shifts

    #region Bounds
    public Bounding GetBounding(bool shifted)
    {
        Bounding bound = Grids.Bounding;
        AdjustBounding(bound, true);
        return bound;
    }

    protected void AdjustBounding(Bounding bound, bool toExternal)
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
    #endregion Bounds

    #region GetSet
    public virtual GridType[,] GetMinimizedArray(GridArray inArr)
    {
        Bounding bounds = Grids.Bounding;
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

    public Bounding GetConnectedBounds()
    {
        List<LayoutObject> connected;
        Bounding bounds;
        ConnectedToAll(out connected, out bounds);
        return bounds;
    }

    public Container2D<GridType> GetConnectedGrid()
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
        var arrOut = new MultiMap<GridType>();
        foreach (var obj in connected)
            arrOut.PutAll(obj.Grids);
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen) && BigBoss.Debug.Flag(DebugManager.DebugFlag.LevelGen_Connected_To))
        {
            BigBoss.Debug.printFooter(Logs.LevelGen, "Get Connected Grid " + this);
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

    public virtual bool ContainsPoint(Point pt)
    {
        return Grids.Contains(pt);
    }

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
            bounds.Absorb(GetBounding(true));
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
            BigBoss.Debug.printFooter(Logs.LevelGen, "Connected To Recursive: " + this);
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
            BigBoss.Debug.printFooter(Logs.LevelGen, "Connected To");
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
        return Grids.ToRowStrings();
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
            BigBoss.Debug.printFooter(log, ToString());
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
