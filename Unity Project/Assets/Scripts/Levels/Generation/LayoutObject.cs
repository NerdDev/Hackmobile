using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class LayoutObject : Container2D<GridType>
{
    public Point ShiftP { get; protected set; }
    readonly HashSet<LayoutObject> _connectedTo = new HashSet<LayoutObject>();
    private static int _nextId = 0;
    public virtual Container2D<GridType> Grids { get; protected set; }
    public override Bounding Bounding
    {
        get
        {
            Bounding bound = Grids.Bounding;
            AdjustBounding(bound, true);
            return bound;
        }
    }
    public int Id { get; protected set; }

    public LayoutObject()
    {
        Id = _nextId++;
        ShiftP = new Point();
        Grids = new MultiMap<GridType>();
    }

    public override GridType this[int x, int y]
    {
        get
        {
            return Grids[x - ShiftP.x, y - ShiftP.y];
        }
        set
        {
            Grids[x - ShiftP.x, y - ShiftP.y] = value;
        }
    }

    #region Shifts
    public override void Shift(int x, int y)
    {
        ShiftP.Shift(x, y);
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
        LayoutObject intersect;
        Point hint;
        while (Intersects(rhs, out intersect, out hint))
        {
            ShiftOutside(intersect, dir, hint, true, true);
        }
    }

    public bool Intersects(IEnumerable<LayoutObject> rhs, out LayoutObject obj, out Point at)
    {
        foreach (LayoutObject l in rhs)
        {
            if (Intersects(l, null, out at))
            {
                obj = l;
                return true;
            }
        }
        obj = null;
        at = null;
        return false;
    }

    public bool Intersects(LayoutObject rhs, Point hint, out Point at)
    {
        if (hint != null && rhs.ContainsPoint(hint))
        {
            at = hint;
            return true;
        }
        foreach (Value2D<GridType> val in this)
        {
            if (rhs.ContainsPoint(val))
            {
                at = val;
                return true;
            }
        }
        at = null;
        return false;
    }

    public void ShiftOutside(LayoutObject rhs, Point dir, Point hint, bool rough, bool finalShift)
    {
        Point reducBase = dir.Reduce();
        Point reduc = new Point(reducBase);
        int xShift, yShift;
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Shift Outside " + ToString());
            BigBoss.Debug.w(Logs.LevelGen, "Shifting outside of " + rhs.ToString());
            BigBoss.Debug.w(Logs.LevelGen, "Shift " + dir + "   Reduc shift: " + reduc);
            BigBoss.Debug.w(Logs.LevelGen, "Bounds: " + Bounding + "  RHS bounds: " + rhs.Bounding);
            MultiMap<GridType> tmp = new MultiMap<GridType>();
            tmp.PutAll(rhs.Grids, rhs.ShiftP);
            tmp.PutAll(Grids, ShiftP);
            tmp.ToLog(Logs.LevelGen, "Before shifting");
        }
        #endregion
        Point at;
        while (Intersects(rhs, hint, out at))
        {
            if (rough)
            {
                Shift(reduc);
                at.Shift(reduc);
                hint = at;
            }
            else
            {
                reduc.Take(out xShift, out yShift);
                Shift(xShift, yShift);
                if (reduc.isZero())
                {
                    reduc = new Point(reducBase);
                }
                at.Shift(xShift, yShift);
                hint = at;
            }
            #region DEBUG
            if (BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps) && BigBoss.Debug.logging(Logs.LevelGen))
            {
                BigBoss.Debug.w(Logs.LevelGen, "Intersected at " + at);
                MultiMap<GridType> tmp = new MultiMap<GridType>();
                tmp.PutAll(rhs.Grids, rhs.ShiftP);
                tmp.PutAll(Grids, ShiftP);
                tmp.ToLog(Logs.LevelGen, "After shifting");
            }
            #endregion
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

    public Container2D<GridType> GetConnectedGrid()
    {
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen) && BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Get Connected Grid " + this);
        }
        #endregion
        List<LayoutObject> connected = ConnectedToAll();
        var arrOut = new MultiMap<GridType>();
        foreach (var obj in connected)
            arrOut.PutAll(obj.Grids, obj.ShiftP);
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen) && BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps))
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
        return Grids.Contains(pt - ShiftP);
    }

    public List<LayoutObject> ConnectedToAll()
    {
        var connected = new List<LayoutObject>();
        ConnectedToRecursive(connected);
        return connected;
    }

    void ConnectedToRecursive(List<LayoutObject> list)
    {
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen) && BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps))
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
        foreach (var connected in _connectedTo)
        {
            if (!list.Contains(connected))
            {
                connected.ConnectedToRecursive(list);
            }
        }
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen) && BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps))
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
        if (BigBoss.Debug.logging(Logs.LevelGen) && BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps))
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
        if (BigBoss.Debug.logging(Logs.LevelGen) && BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps))
        {
            BigBoss.Debug.printFooter(Logs.LevelGen, "Connected To");
        }
        #endregion
        return failObj == null;
    }

    public bool ConnectedTo(LayoutObject obj)
    {
        var connected = ConnectedToAll();
        return connected.Contains(obj);
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

    public virtual string GetTypeString()
    {
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

    public override IEnumerator<Value2D<GridType>> GetEnumerator()
    {
        foreach (Value2D<GridType> val in Grids)
        {
            val.x += ShiftP.x;
            val.y += ShiftP.y;
            yield return val;
        }
    }

    #region Container2D
    public override bool TryGetValue(int x, int y, out GridType val)
    {
        return Grids.TryGetValue(x - ShiftP.x, y - ShiftP.y, out val);
    }

    public override int Count
    {
        get { return Grids.Count; }
    }

    public override Array2D<GridType> Array
    {
        get 
        {
            Grids.Shift(ShiftP);
            ShiftP = new Point();
            return Grids.Array;
        }
    }

    public override bool Contains(int x, int y)
    {
        return Grids.Contains(x - ShiftP.x, y - ShiftP.y);
    }

    public override bool InRange(int x, int y)
    {
        return Grids.Contains(x - ShiftP.x, y - ShiftP.y);
    }

    public override bool DrawAll(DrawAction<GridType> call)
    {
        return Grids.DrawAll(call);
    }

    public override void Clear()
    {
        Grids.Clear();
    }

    public override Array2DRaw<GridType> RawArray(out Point shift)
    {
        Array2DRaw<GridType> ret = Grids.RawArray(out shift);
        shift += ShiftP;
        return ret;
    }

    public override bool Remove(int x, int y)
    {
        return Grids.Remove(x - ShiftP.x, y - ShiftP.y);
    }
    #endregion
}
