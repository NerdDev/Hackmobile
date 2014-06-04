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

    public bool RandomChild(System.Random rand, out LayoutObject<T> rhs)
    {
        return children.Random(rand, out rhs);
    }

    public List<LayoutObject<T>> GetChildren()
    {
        return new List<LayoutObject<T>>(children);
    }

    public virtual bool ContainsPoint(Point pt)
    {
        return grids.Contains(pt);
    }

    public List<LayoutObject<T>> ConnectedToAll()
    {
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen) && BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Connected To All: " + this);
        }
        #endregion
        var connected = new List<LayoutObject<T>>();
        ConnectedToRecursive(connected);
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen) && BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps))
        {
            BigBoss.Debug.printFooter(Logs.LevelGen, "Connected To All: " + this);
        }
        #endregion
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

    public override void ToLog(Logs log, params String[] customContent)
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

    public override void ToLog(Logs log)
    {
        if (BigBoss.Debug.logging(log))
        {
            ToLog(log, new String[0]);
        }
    }

    public void ToLogNumberRooms(Log log, params string[] customContent)
    {
        MultiMap<char> tmp = new MultiMap<char>();
        foreach (var room in IterateAllChildren())
        {
            char c = (char)((room.Id + 33) % 93);
            foreach (var v in room)
            {
                tmp[v] = c;
            }
            Point center = room.Bounding.GetCenter();
            string number = room.Id.ToString();
            center.x -= number.Length / 2;
            foreach (char c1 in number)
            {
                tmp[center] = c1;
                center.x++;
            }
        }
        tmp.ToLog(log, customContent);
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

    public IEnumerable<LayoutObject<T>> IterateAllChildren()
    {
        if (Child)
        {
            yield return this;
        }
        foreach (LayoutObject<T> child in children)
        {
            foreach (LayoutObject<T> subChild in child.IterateAllChildren())
            {
                yield return subChild;
            }
        }
    }

    public void PrintChildrenTree(Log log)
    {
        log.printHeader(this.ToString());
        foreach (var child in children)
        {
            child.PrintChildrenTree(log);
        }
        log.printFooter(this.ToString());
    }

    public override bool DrawAll(DrawAction<T> call, Container2D<T> on)
    {
        return grids.DrawAll(call, on);
    }

    public bool GetObjAt(int x, int y, out LayoutObject<T> obj)
    {
        if (!grids.Contains(x, y))
        {
            obj = null;
            return false;
        }
        foreach (var child in children)
        {
            if (child.GetObjAt(x, y, out obj))
            {
                return true;
            }
        }
        obj = this;
        return true;
    }

    public List<LayoutObject<T>> ConnectToChildrenAt(LayoutObject<T> rhs, int x, int y)
    {
        List<LayoutObject<T>> ret = new List<LayoutObject<T>>();
        foreach (var room in rhs.GetChildrenAt(x, y))
        {
            if (this.ConnectTo(room))
            {
                ret.Add(room);
            }
        }
        return ret;
    }

    public IEnumerable<LayoutObject<T>> GetObjsAt(int x, int y, LayoutObjectType type)
    {
        if (grids.Contains(x, y))
        {
            foreach (var child in children)
            {
                if (child.Type == type)
                {
                    yield return child;
                }
                foreach (var recursive in child.GetObjsAt(x, y, type))
                {
                    yield return recursive;
                }
            }
        }
    }

    public List<LayoutObject<T>> GetChildrenAt(int x, int y)
    {
        List<LayoutObject<T>> ret = new List<LayoutObject<T>>();
        if (grids.Contains(x, y))
        {
            foreach (var child in children)
            {
                if (child.Child)
                {
                    if (child.Contains(x, y))
                    {
                        ret.Add(child);
                    }
                }
                else
                {
                    ret.AddRange(child.GetChildrenAt(x, y));
                }
            }
        }
        return ret;
    }

    public bool ConnectTo(LayoutObject<T> rhs)
    {
        if (rhs == null)
        {
            throw new ArgumentException();
        }
        if (this.Equals(rhs))
        {
            return false;
        }
        _connectedTo.Add(rhs);
        rhs._connectedTo.Add(this);
        return true;
    }

    public override void Rotate(Rotation rotate)
    {
        grids.Rotate(rotate);
    }

    public Point GetCenterShiftOn(LayoutObject<T> rhs)
    {
        Point rhsCenter = rhs.Bounding.GetCenter();
        return rhsCenter - Bounding.GetCenter();
    }
}

public static class LayoutObjectExt
{
    #region Door Placement
    public static ProbabilityList<int> DoorRatioPicker;
    public static List<Value2D<GenSpace>> PlaceSomeDoors(
        this LayoutObject<GenSpace> cont, 
        IEnumerable<Point> points, 
        System.Random rand,
        bool external, 
        int desiredWallToDoorRatio = -1)
    {
        return PlaceSomeDoors(cont, cont, points, rand, external, desiredWallToDoorRatio);
    }

    public static List<Value2D<GenSpace>> PlaceSomeDoors(
        this LayoutObject<GenSpace> cont, 
        LayoutObject<GenSpace> referenceCont, 
        IEnumerable<Point> points, 
        System.Random rand, 
        bool external,
        int desiredWallToDoorRatio = -1)
    {
        if (desiredWallToDoorRatio < 0)
        {
            desiredWallToDoorRatio = LevelGenerator.desiredWallToDoorRatio;
        }
        var acceptablePoints = new MultiMap<GenSpace>();
        Counter numPoints = new Counter();
        DrawAction<GenSpace> call = Draw.Count<GenSpace>(numPoints).And(Draw.CanDrawDoor(external).IfThen(Draw.AddTo(acceptablePoints)));
        referenceCont.DrawPoints(points, call);
        if (acceptablePoints.Count == 0)
        {
            return new List<Value2D<GenSpace>>(0);
        }
        if (DoorRatioPicker == null)
        {
            DoorRatioPicker = new ProbabilityList<int>();
            DoorRatioPicker.Add(-2, .25);
            DoorRatioPicker.Add(-1, .5);
            DoorRatioPicker.Add(0, 1);
            DoorRatioPicker.Add(1, .5);
            DoorRatioPicker.Add(2, .25);
        }
        int numDoors = numPoints / desiredWallToDoorRatio;
        numDoors += DoorRatioPicker.Get(rand);
        if (numDoors <= 0)
        {
            numDoors = 1;
        }
        List<Value2D<GenSpace>> pickedPts = acceptablePoints.GetRandom(rand, numDoors, 1);
        DrawAction<GenSpace> additionalTest = null;
        MultiMap<GenSpace> notAllowed = null;
        MultiMap<GenSpace> allowed = new MultiMap<GenSpace>();
        foreach (Point p in points)
        {
            allowed[p] = null;
        }
        notAllowed = new MultiMap<GenSpace>();
        foreach (var v in pickedPts)
        {
            notAllowed[v] = null;
        }
        additionalTest = Draw.PointContainedIn(allowed).And(Draw.Not(Draw.HasAround(false, Draw.PointContainedIn(notAllowed))));
        foreach (Point picked in pickedPts)
        {
            notAllowed.Remove(picked);
            PlaceDoor(cont, picked.x, picked.y, rand, external, additionalTest);
            notAllowed[picked] = null;
        }
        return pickedPts;
    }

    public static bool PlaceDoor(
        this LayoutObject<GenSpace> cont,
        int x,
        int y,
        System.Random rand,
        bool considerNulls,
        DrawAction<GenSpace> additionalTest = null)
    {
        return PlaceDoor(cont, cont, x, y, rand, considerNulls, additionalTest);
    }

    public static bool PlaceDoor(
        this LayoutObject<GenSpace> cont,
        LayoutObject<GenSpace> referenceCont,
        int x, 
        int y,
        System.Random rand, 
        bool considerNulls, 
        DrawAction<GenSpace> additionalTest = null)
    {
        DrawAction<GenSpace> test = Draw.CanDrawDoor(considerNulls);
        if (additionalTest != null)
        {
            test = test.And(additionalTest);
        }

        // Count largest option
        Counter horiz = new Counter();
        referenceCont.DrawLineExpanding(x, y, GridDirection.HORIZ, 5, test.And(Draw.Count<GenSpace>(horiz)));
        Counter vert = new Counter();
        referenceCont.DrawLineExpanding(x, y, GridDirection.VERT, 5, test.And(Draw.Count<GenSpace>(vert)));

        // Find largest
        GridDirection dir;
        int count;
        if (horiz.Count < vert || (horiz == vert && rand.NextBool()))
        {
            count = vert;
            dir = GridDirection.VERT;
        }
        else
        {
            count = horiz;
            dir = GridDirection.HORIZ;
        }
        count = Math.Max(count, 1);

        // Pick random size
        for (; count > 1; count--)
        {
            if (rand.NextBool())
            {
                break;
            }
        }

        // Pick door
        GenSpace space;
        if (referenceCont.TryGetValue(x, y, out space))
        {
            SmartThemeElement doorElement;
            while (!space.Theme.Door.Select(rand, count, 1, out doorElement) && count > 1)
            {
                count--;
            }

            ThemeElement door = doorElement.Get(rand);
            cont.DrawLineExpanding(x, y, dir, count / 2, Draw.MergeIn(door, space.Theme, GridType.Door, false));
            return true;
        }
        return false;
    }
    #endregion
}