using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class LayoutObject<T> : Container2D<T>
    where T : IGridType
{
    private static int _nextId = 0;
    public int Id { get; protected set; }
    public LayoutObjectType Type { get; protected set; }
    readonly HashSet<LayoutObject<T>> _connectedTo = new HashSet<LayoutObject<T>>();
    protected Container2D<T> grids = new MultiMap<T>();
    public override Bounding Bounding { get { return grids.Bounding; } }
    protected List<LayoutObject<T>> children = new List<LayoutObject<T>>(0);
    public bool Child { get { return children.Count == 0; } }
    public int ChildCount { get { return children.Count; } }
    public event Action<LayoutObject<T>, int, int> OnShifted;
    public event Action<int, int, T> OnModifiedSpace;

    public LayoutObject(LayoutObjectType type)
    {
        this.Type = type;
        Id = _nextId++;
    }

    public override T this[int x, int y]
    {
        get
        {
            return grids[x, y];
        }
        set
        {
            grids[x, y] = value;
            if (OnModifiedSpace != null)
            {
                OnModifiedSpace(x, y, value);
            }
        }
    }

    public override void Shift(int x, int y)
    {
        grids.Shift(x, y);
        if (OnShifted != null)
        {
            OnShifted(this, x, y);
        }
    }

    public void AddChild(LayoutObject<T> rhs)
    {
        children.Add(rhs);
        PutAll(rhs);
        rhs.OnModifiedSpace += (x, y, t) => this[x, y] = t;
        rhs.OnShifted += (l, x, y) => { throw new NotImplementedException(); };
    }

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
        {
            arrOut.PutAll(obj.grids);
        }
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

    public bool RandomChild(System.Random rand, out LayoutObject<T> rhs)
    {
        return children.Random(rand, out rhs);
    }

    public virtual bool ContainsPoint(Point pt)
    {
        return grids.Contains(pt);
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
        return Type + " " + Id;
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
        return grids.ToRowStrings();
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
            Bounding bounds = Bounding;
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

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        LayoutObject<T> rhs = obj as LayoutObject<T>;
        if (rhs == null) return false;
        return this.Id == rhs.Id;
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
        return grids.GetEnumerator();
    }

    public override IEnumerable<T> GetEnumerateValues()
    {
        return grids.GetEnumerateValues();
    }

    #region Container2D
    public override bool TryGetValue(int x, int y, out T val)
    {
        return grids.TryGetValue(x, y, out val);
    }

    public override int Count { get { return grids.Count; } }

    public override Array2D<T> Array { get { return grids.Array; } }

    public override bool Contains(int x, int y)
    {
        return grids.Contains(x, y);
    }

    public override bool InRange(int x, int y)
    {
        return grids.InRange(x, y);
    }

    public override void Clear()
    {
        grids.Clear();
    }

    public override Array2DRaw<T> RawArray(out Point shift)
    {
        return grids.RawArray(out shift);
    }

    public override bool Remove(int x, int y)
    {
        return grids.Remove(x, y);
    }
    #endregion

    public IEnumerable<LayoutObject<T>> Flatten(LayoutObjectType type)
    {
        if (Type == type)
        {
            yield return this;
        }
        foreach (LayoutObject<T> obj in children)
        {
            foreach (var target in obj.Flatten(type))
            {
                yield return target;
            }
        }
    }

    public override bool DrawAll(DrawAction<T> call, Container2D<T> on)
    {
        grids.DrawAll(call, on);
    }
}
