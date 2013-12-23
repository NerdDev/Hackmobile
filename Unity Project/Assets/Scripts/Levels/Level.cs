using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level : IEnumerable<Value2D<GridSpace>>
{
    protected LevelLayout Layout { get; private set; }
    public bool Populated { get; set; }
    public GridSpace[,] Array { get; protected set; }
    private List<RoomMap> roomMaps = new List<RoomMap>();
    private MultiMap<RoomMap> roomMapping = new MultiMap<RoomMap>(); // floor space to roommap
    public StairLink UpStairs { get; set; }
    public StairLink DownStairs { get; set; }
    public Theme Theme { get; protected set; }

    public Level(LevelLayout layout, Theme theme)
    {
        Layout = layout;
        Array = GridSpace.Convert(layout.GetArray());
        LoadRoomMaps();
        Theme = theme;
    }

    private void LoadRoomMaps()
    {
        foreach (LayoutObjectLeaf room in Layout.GetRooms())
        {
            RoomMap roomMap = new RoomMap(room, Array);
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
            if (x < Array.GetLength(1) && y < Array.GetLength(0))
            {
                GridSpace space = Array[y, x];
                if (space == null)
                { // Create empty gridspace
                    space = new GridSpace(GridType.NULL);
                    Array[y, x] = space;
                    return space;
                }
                return space;
            }
            return null;
        }
    }

    public Point CenterShift()
    {
        Bounding bound = new Bounding();
        Array.DrawSquare(Draw.IfThen<GridSpace>((arr, x, y) =>
            {
                return arr[y, x].Type == GridType.NULL;
            },
            (arr, x, y) =>
            {
                GridSpace gs = arr[y, x];
                bound.Absorb(gs.X, gs.Y);
                return true;
            }));
        return bound.GetCenter();
    }

    #region ConvenienceFunctions
    public void Put(int x, int y, WorldObject obj)
    {
        this[x, y].Put(obj);
    }

    public void Remove(int x, int y, WorldObject obj)
    {
        this[x, y].Remove(obj);
    }

    public void Move(WorldObject obj, int xFrom, int yFrom, int xTo, int yTo)
    {
        Remove(xFrom, yFrom, obj);
        Put(xTo, yTo, obj);
    }

    public bool Accept(int x, int y, WorldObject obj)
    {
        return this[x, y].Accept(obj);
    }

    public bool IsBlocked(int x, int y)
    {
        return this[x, y].IsBlocked();
    }

    public bool HasNonBlocking(int x, int y)
    {
        return this[x, y].HasNonBlocking();
    }

    public bool HasObject(int x, int y)
    {
        return this[x, y].HasObject();
    }

    public bool IsEmpty(int x, int y)
    {
        return this[x, y].IsEmpty();
    }

    public List<WorldObject> GetContained(int x, int y)
    {
        return this[x, y].GetContained();
    }

    public List<WorldObject> GetFreeObjects(int x, int y)
    {
        return this[x, y].GetFreeObjects();
    }

    public List<WorldObject> GetBlockingObjects(int x, int y)
    {
        return this[x, y].GetBlockingObjects();
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
        GridType[,] ret = new GridType[lev.Array.GetLength(0), lev.Array.GetLength(1)];
        for (int y = 0; y < lev.Array.GetLength(0); y++)
        {
            for (int x = 0; x < lev.Array.GetLength(1); x++)
            {
                if (lev.Array[y, x] != null)
                    ret[y, x] = lev.Array[y, x];
            }
        }
        return new GridArray(ret);
    }

    public MultiMap<GridSpace> GetArea(Bounding bounds)
    {
        MultiMap<GridSpace> ret = new MultiMap<GridSpace>();
        Bounding inBound = bounds.InBounds(Array);
        for (int y = inBound.YMin; y < inBound.YMax; y++)
        {
            for (int x = inBound.XMin; x < inBound.XMax; x++)
            {
                GridSpace space = Array[y, x];
                if (space != null)
                    ret[x, y] = space;
            }
        }
        return ret;
    }

    public IEnumerable<GridSpace> Iterate()
    {
        for (int y = 0; y < Array.GetLength(0); y++)
        {
            for (int x = 0; x < Array.GetLength(1); x++)
            {
                GridSpace space = Array[y, x];
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
        for (int y = 0; y < Array.GetLength(0); y++)
        {
            for (int x = 0; x < Array.GetLength(1); x++)
            {
                GridSpace space = Array[y, x];
                if (space != null)
                    yield return new Value2D<GridSpace>(x, y, Array[y, x]);
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}
