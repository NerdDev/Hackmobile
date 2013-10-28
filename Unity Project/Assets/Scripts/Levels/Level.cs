using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level : IEnumerable<Value2D<GridSpace>>
{
    protected LevelLayout Layout { get; private set; }
    public bool Populated { get; set; }
    private GridSpace[,] Arr;
    private List<RoomMap> roomMaps = new List<RoomMap>();
    private MultiMap<RoomMap> roomMapping = new MultiMap<RoomMap>();
    public Surrounding<GridSpace> Surrounding { get; set; }
    public Theme Theme { get; protected set; }

    public Level(LevelLayout layout, Theme theme)
    {
        Layout = layout;
        Arr = GridSpace.Convert(layout.GetArray());
        Surrounding = new Surrounding<GridSpace>(Arr, true);
        LoadRoomMaps();
        Theme = theme;
    }

    private void LoadRoomMaps()
    {
        foreach (Room room in Layout.GetRooms())
        {
            RoomMap roomMap = new RoomMap(room, Arr);
            roomMaps.Add(roomMap);
            foreach (Value2D<GridSpace> floor in roomMap)
            {
                roomMapping[floor.x, floor.y] = roomMap;
            }
        }
    }

    public GridSpace this[int x, int y]
    {
        get
        {
            if (x < Arr.GetLength(1) && y < Arr.GetLength(0))
            {
                GridSpace space = Arr[y, x];
                if (space == null)
                { // Create empty gridspace
                    space = new GridSpace(GridType.NULL);
                    Arr[y, x] = space;
                    return space;
                }
                return space;
            }
            return null;
        }
    }

    #region ConvenienceFunctions
    public void Put(int x, int y, WorldObject obj)
    {
        this[x,y].Put(obj);
    }

    public void Remove(int x, int y, WorldObject obj)
    {
        this[x,y].Remove(obj);
    }

    public void Move(WorldObject obj, int xFrom, int yFrom, int xTo, int yTo)
    {
        Remove(xFrom, yFrom, obj);
        Put(xTo, yTo, obj);
    }

    public bool Accept(int x, int y, WorldObject obj)
    {
        return this[x,y].Accept(obj);
    }

    public bool IsBlocked(int x, int y)
    {
        return this[x,y].IsBlocked();
    }

    public bool HasNonBlocking(int x, int y)
    {
        return this[x,y].HasNonBlocking();
    }

    public bool HasObject(int x, int y)
    {
        return this[x,y].HasObject();
    }

    public bool IsEmpty(int x, int y)
    {
        return this[x,y].IsEmpty();
    }

    public List<WorldObject> GetContained(int x, int y)
    {
        return this[x,y].GetContained();
    }

    public List<WorldObject> GetFreeObjects(int x, int y)
    {
        return this[x,y].GetFreeObjects();
    }

    public List<WorldObject> GetBlockingObjects(int x, int y)
    {
        return this[x,y].GetBlockingObjects();
    }
    #endregion

    public void ToLog(Logs log, params string[] customContent)
    {
        if (BigBoss.Debug.logging(log))
        {
            GridArray ga = this;
            ga.ToLog(log, customContent);
        }
    }

    public static implicit operator GridArray(Level lev)
    {
        GridType[,] ret = new GridType[lev.Arr.GetLength(0), lev.Arr.GetLength(1)];
        for (int y = 0; y < lev.Arr.GetLength(0); y++)
        {
            for (int x = 0; x < lev.Arr.GetLength(1); x++)
            {
                if (lev.Arr[y, x] != null)
                    ret[y, x] = lev.Arr[y, x];
            }
        }
        return new GridArray(ret);
    }

    public MultiMap<GridSpace> GetArea(Bounding bounds)
    {
        MultiMap<GridSpace> ret = new MultiMap<GridSpace>();
        Bounding inBound = bounds.InBounds(Arr);
        for (int y = inBound.YMin; y < inBound.YMax; y++)
        {
            for (int x = inBound.XMin; x < inBound.XMax; x++)
            {
                GridSpace space = Arr[y, x];
                if (space != null)
                    ret[x, y] = space;
            }
        }
        return ret;
    }

    public IEnumerable<GridSpace> Iterate()
    {
        for (int y = 0; y < Arr.GetLength(0); y++)
        {
            for (int x = 0; x < Arr.GetLength(1); x++)
            {
                GridSpace space = Arr[y, x];
                if (space != null)
                    yield return space;
            }
        }
    }

    public List<RoomMap> GetRooms()
    {
        return roomMaps;
    }

    public IEnumerator<Value2D<GridSpace>> GetEnumerator()
    {
        for (int y = 0; y < Arr.GetLength(0); y++)
        {
            for (int x = 0; x < Arr.GetLength(1); x++)
            {
                GridSpace space = Arr[y, x];
                if (space != null)
                    yield return new Value2D<GridSpace>(x, y, Arr[y, x]);
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    public IEnumerable<Value2D<GridSpace>> getSurroundingSpaces(int x, int y)
    {
        Surrounding.Load(x, y);
        foreach (Value2D<GridSpace> val in Surrounding)
        {
            yield return val;
        }
    }
}
