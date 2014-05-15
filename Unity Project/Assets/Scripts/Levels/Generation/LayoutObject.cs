using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class LayoutObject<T> : Container2D<T>, ILayoutObject<T>
    where T : IGridType
{
    public Point ShiftP;
    readonly HashSet<LayoutObject<T>> _connectedTo = new HashSet<LayoutObject<T>>();
    private static int _nextId = 0;
    public virtual Container2D<T> Grids { get; set; }
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
    string _name;
    public LayoutObject<T> Object { get { return this; } }

    public LayoutObject(String name)
    {
        _name = name;
        Id = _nextId++;
        ShiftP = new Point();
        Grids = new MultiMap<T>();
    }

    public override T this[int x, int y]
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

    public override void Shift(int x, int y)
    {
        ShiftP.x += x;
        ShiftP.y += y;
    }

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

    public Container2D<T> GetConnectedGrid()
    {
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen) && BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Get Connected Grid " + this);
        }
        #endregion
        List<LayoutObject<T>> connected = ConnectedToAll();
        var arrOut = new MultiMap<T>();
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

    public void Connect(LayoutObject<T> obj)
    {
        if (obj != null)
        {
            if (BigBoss.Debug.logging(Logs.LevelGen))
            {
                BigBoss.Debug.w(Logs.LevelGen, "Connecting " + ToString() + " to " + obj.ToString());
            }
            _connectedTo.Add(obj);
            obj._connectedTo.Add(this);
        }
    }

    public virtual bool ContainsPoint(Point pt)
    {
        return Grids.Contains(pt - ShiftP);
    }

    public List<LayoutObject<T>> ConnectedToAll()
    {
        var connected = new List<LayoutObject<T>>();
        ConnectedToRecursive(connected);
        return connected;
    }

    void ConnectedToRecursive(List<LayoutObject<T>> list)
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

    public bool ConnectedTo(IEnumerable<LayoutObject<T>> roomsToConnect, out LayoutObject<T> failObj)
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

    public bool ConnectedTo(LayoutObject<T> obj)
    {
        var connected = ConnectedToAll();
        return connected.Contains(obj);
    }

    public bool GetPathTo(LayoutObject<T> target, out List<LayoutObject<T>> list)
    {
        var visited = new HashSet<LayoutObject<T>> { this };
        return GetPathToRecursive(this, target, visited, out list);
    }

    bool GetPathToRecursive(LayoutObject<T> cur, LayoutObject<T> target, HashSet<LayoutObject<T>> visited, out List<LayoutObject<T>> list)
    {
        list = new List<LayoutObject<T>>();
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
                List<LayoutObject<T>> targetPath;
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
        return _name + " " + Id;
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
        return Grids.ToRowStrings(Grids.Bounding.Shift(ShiftP));
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
            Bounding bounds = GetBounding(true);
            foreach (string s in ToRowStrings(bounds))
            {
                BigBoss.Debug.w(log, s);
            }
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

    protected bool Equals(LayoutObject<T> other)
    {
        return Id == other.Id;
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((LayoutObject<T>)obj);
    }

    public override int GetHashCode()
    {
        return Id;
    }

    public static bool operator ==(LayoutObject<T> left, LayoutObject<T> right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(LayoutObject<T> left, LayoutObject<T> right)
    {
        return !Equals(left, right);
    }

    public override IEnumerator<Value2D<T>> GetEnumerator()
    {
        foreach (var val in Grids)
        {
            val.x += ShiftP.x;
            val.y += ShiftP.y;
            yield return val;
        }
    }

    public override IEnumerable<T> GetEnumerateValues()
    {
        return Grids.GetEnumerateValues();
    }

    #region Container2D
    public override bool TryGetValue(int x, int y, out T val)
    {
        return Grids.TryGetValue(x - ShiftP.x, y - ShiftP.y, out val);
    }

    public override int Count
    {
        get { return Grids.Count; }
    }

    public override Array2D<T> Array
    {
        get 
        {
            Point tmp = new Point(ShiftP);
            Grids.Shift(ShiftP);
            var arr = Grids.Array;
            Grids.Shift(-tmp);
            return arr;
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

    public override bool DrawAll(DrawAction<T> call)
    {
        return Grids.DrawAll(call);
    }

    public override void Clear()
    {
        Grids.Clear();
    }

    public override Array2DRaw<T> RawArray(out Point shift)
    {
        var ret = Grids.RawArray(out shift);
        shift += ShiftP;
        return ret;
    }

    public override bool Remove(int x, int y)
    {
        return Grids.Remove(x - ShiftP.x, y - ShiftP.y);
    }
    #endregion

    public Container2D<T> GetGrid()
    {
        var map = new MultiMap<T>();
        map.PutAll(Grids, ShiftP);
        return map;
    }

    public List<LayoutObject<T>> Flatten()
    {
        return new List<LayoutObject<T>>(new[] { this });
    }
}
