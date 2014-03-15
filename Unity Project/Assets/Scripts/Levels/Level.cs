using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Level : Container2D<GridSpace>
{
    public bool Populated;
    protected Array2D<GridSpace> _array;
    public List<Container2D<GridSpace>> RoomMaps = new List<Container2D<GridSpace>>();
    private MultiMap<Container2D<GridSpace>> roomMapping = new MultiMap<Container2D<GridSpace>>(); // floor space to roommap
    public Bounding UpStartPoint;
    public Bounding DownStartPoint;
    public System.Random Random { get; protected set; }

    public Level(Container2D<GridSpace> spaces, LevelLayout layout, Theme theme, System.Random rand)
    {
        _array = spaces.Array;
        LoadRoomMaps(layout);
        UpStartPoint = layout.UpStart;
        DownStartPoint = layout.DownStart;
        Random = rand;
    }

    private void LoadRoomMaps(LevelLayout layout)
    {
        foreach (LayoutObject room in layout.Rooms)
        {
            var roomMap = new MultiMap<GridSpace>();
            RoomMaps.Add(roomMap);
            foreach (Value2D<GenSpace> floor in room.Grids)
            {
                int x = floor.x + room.ShiftP.x;
                int y = floor.y = room.ShiftP.y;
                roomMap[x, y] = _array[x, y];
                roomMapping[x, y] = roomMap;
            }
        }
    }

    #region Container2D
    public override GridSpace this[int x, int y]
    {
        get
        {
            if (x < _array.Width && y < _array.Height)
            {
                GridSpace space = _array[x, y];
                if (space == null)
                { // Create empty gridspace
                    space = new GridSpace(GridType.NULL, x, y);
                    _array[x, y] = space;
                    return space;
                }
                return space;
            }
            return null;
        }
        set
        {
            _array[x, y] = value;
        }
    }

    public override Array2D<GridSpace> Array { get { return _array; } }

    public override bool TryGetValue(int x, int y, out GridSpace val)
    {
        if (_array.InRange(x, y))
        {
            val = this[x, y];
            return true;
        }
        else
        {
            val = null;
            return false;
        }
    }

    public override int Count
    {
        get { return _array.Count; }
    }

    public override Bounding Bounding
    {
        get { return _array.Bounding; }
    }

    public override bool Contains(int x, int y)
    {
        return _array.Contains(x, y);
    }

    public override bool InRange(int x, int y)
    {
        return _array.InRange(x, y);
    }

    public override bool DrawAll(DrawAction<GridSpace> call)
    {
        return _array.DrawAll(call);
    }

    public override void Clear()
    {
        _array.Clear();
    }

    public override Array2DRaw<GridSpace> RawArray(out Point shift)
    {
        return _array.RawArray(out shift);
    }

    public override bool Remove(int x, int y)
    {
        return _array.Remove(x, y);
    }

    public override void Shift(int x, int y)
    {
        _array.Shift(x, y);
    }

    public override IEnumerable<GridSpace> GetEnumerateValues()
    {
        return _array.GetEnumerateValues();
    }

    public override IEnumerator<Value2D<GridSpace>> GetEnumerator()
    {
        return _array.GetEnumerator();
    }
    #endregion

    public void PlacePlayer(bool up)
    {
        Bounding startBounding;
        if (up)
        {
            startBounding = UpStartPoint;
        }
        else
        {
            startBounding = DownStartPoint;
        }
        BigBoss.Debug.w(Logs.Main, "Placing player in position.");
        RandomPicker<GridSpace> picker;
        this._array.DrawRect(startBounding, Draw.IsType<GridSpace>(GridType.StairPlace).IfThen(Draw.PickRandom(out picker)));
        Value2D<GridSpace> start;
        if (picker.Pick(Random, out start))
        {
            throw new ArgumentException("Cannot place player");
        }
        BigBoss.PlayerInfo.transform.position = new Vector3(start.x, 0, start.y);
        BigBoss.Player.GridSpace = start.val;
        BigBoss.Levels.Builder.Instantiate(start);
        BigBoss.Debug.w(Logs.Main, "Placed player on " + start);
    }
}
