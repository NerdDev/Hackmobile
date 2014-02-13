using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Level : IEnumerable<Value2D<GridSpace>>
{
    protected LevelLayout Layout { get; private set; }
    public bool Populated;
    public Array2D<GridSpace> Array { get; protected set; }
    public List<Container2D<GridType>> RoomMaps = new List<Container2D<GridType>>();
    private MultiMap<Container2D<GridType>> roomMapping = new MultiMap<Container2D<GridType>>(); // floor space to roommap
    public Point UpStartPoint;
    public Point DownStartPoint;
    public Theme Theme { get; protected set; }

    public Level(LevelLayout layout, Theme theme)
    {
        Layout = layout;
        Array = GridSpace.Convert(layout.Bake().Grids);
        LoadRoomMaps();
        Theme = theme;
        UpStartPoint = layout.UpStart;
        DownStartPoint = layout.DownStart;
    }

    private void LoadRoomMaps()
    {
        foreach (LayoutObject room in Layout.GetRooms())
        {
            MultiMap<GridType> roomMap = new MultiMap<GridType>(room.Grids, room.ShiftP);
            RoomMaps.Add(roomMap);
            foreach (Value2D<GridType> floor in room.Grids)
            {
                roomMapping[floor.x + room.ShiftP.x, floor.y + room.ShiftP.y] = room.Grids;
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
        Array.DrawAll(
            Draw.Not(Draw.IsType(GridType.NULL))
            .IfThen((arr, x, y) =>
            {
                GridSpace gs = arr[x, y];
                bound.Absorb(gs.X, gs.Y);
                return true;
            }));
        return bound.GetCenter();
    }

    public void PlacePlayer(bool up)
    {
        Point startPoint;
        if (up)
        {
            startPoint = UpStartPoint;
        }
        else
        {
            startPoint = DownStartPoint;
        }
        BigBoss.Debug.w(Logs.Main, "Placing player in position.");
        Value2D<GridSpace> start;
        this.Array.GetPointAround(startPoint.x, startPoint.y, false, Draw.IsType(GridType.StairPlace), out start);
        BigBoss.PlayerInfo.transform.position = new Vector3(start.x, -.5f, start.y);
        BigBoss.Player.GridSpace = start.val;
        BigBoss.Levels.Builder.Build(start);
        BigBoss.Debug.w(Logs.Main, "Placed player on " + start);
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

    public IEnumerator<Value2D<GridSpace>> GetEnumerator()
    {
        return Array.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}
