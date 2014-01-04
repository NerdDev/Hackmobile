using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level : IEnumerable<Value2D<GridSpace>>
{
    protected LevelLayout Layout { get; private set; }
    public bool Populated { get; set; }
    public Array2D<GridSpace> Array { get; protected set; }
    public List<Container2D<GridType>> RoomMaps = new List<Container2D<GridType>>();
    private MultiMap<Container2D<GridType>> roomMapping = new MultiMap<Container2D<GridType>>(); // floor space to roommap
    public StairLink UpStairs { get; set; }
    public StairLink DownStairs { get; set; }
    public Theme Theme { get; protected set; }

    public Level(LevelLayout layout, Theme theme)
    {
        Layout = layout;
        Array = GridSpace.Convert(layout.Grids);
        LoadRoomMaps();
        Theme = theme;
    }

    private void LoadRoomMaps()
    {
        foreach (LayoutObjectLeaf room in Layout.GetRooms())
        {
            RoomMaps.Add(room.Grids);
            foreach (Value2D<GridType> floor in room.Grids)
            {
                roomMapping[floor.x, floor.y] = room.Grids;
            }
        }
    }

    public GridSpace this[int x, int y]
    {
        get
        {
            if (x < Array.Width && y < Array.Height)
            {
                GridSpace space = Array[x, y];
                if (space == null)
                { // Create empty gridspace
                    space = new GridSpace(GridType.NULL, x, y);
                    Array[x, y] = space;
                    return space;
                }
                return space;
            }
            return null;
        }
    }

    public GridSpace this[Point p] { get { return this[p.x, p.y]; } }

    public Point CenterShift()
    {
        Bounding bound = new Bounding();
        Array.DrawSquare(Draw.IfThen<GridSpace>((arr, x, y) =>
            {
                return arr[x, y].Type == GridType.NULL;
            },
            (arr, x, y) =>
            {
                GridSpace gs = arr[x, y];
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
            Array.ToLog(log, customContent);
        }
    }

    public MultiMap<GridSpace> GetArea(Bounding bounds)
    {
        MultiMap<GridSpace> ret = new MultiMap<GridSpace>();
        Bounding inBound = bounds.InBounds(Array);
        for (int y = inBound.YMin; y < inBound.YMax; y++)
        {
            for (int x = inBound.XMin; x < inBound.XMax; x++)
            {
                GridSpace space = this[x, y];
                if (space != null)
                    ret[x, y] = space;
            }
        }
        return ret;
    }

    public IEnumerable<GridSpace> Iterate()
    {
        for (int y = 0; y < Array.Height; y++)
        {
            for (int x = 0; x < Array.Width; x++)
            {
                GridSpace space = this[x, y];
                if (space != null)
                    yield return space;
            }
        }
    }

    public IEnumerator<Value2D<GridSpace>> GetEnumerator()
    {
        for (int y = 0; y < Array.Height; y++)
        {
            for (int x = 0; x < Array.Width; x++)
            {
                GridSpace space = this[x, y];
                if (space != null)
                    yield return new Value2D<GridSpace>(x, y, this[x, y]);
            }
        }
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}
