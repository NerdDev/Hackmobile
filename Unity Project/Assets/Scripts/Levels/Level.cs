using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Level : Container2D<GridSpace>
{
    public bool Populated;
    protected MultiMap<GridSpace> map;
    public MultiMap<GridSpace> UnderlyingContainer
    {
        set
        {
            map = value;
        }
    }
    public List<Container2D<GridSpace>> RoomMaps = new List<Container2D<GridSpace>>();
    private MultiMap<Container2D<GridSpace>> roomMapping = new MultiMap<Container2D<GridSpace>>(); // floor space to roommap
    public System.Random Random;
    public HashSet<WorldObject> WorldObjects = new HashSet<WorldObject>();

    public Level()
    {
    }

    public void LoadRoomMaps(LevelLayout layout)
    {
        foreach (LayoutObject<GenSpace> room in layout.Rooms)
        {
            var roomMap = new MultiMap<GridSpace>();
            RoomMaps.Add(roomMap);
            foreach (Value2D<GenSpace> floor in room.Grids)
            {
                int x = floor.x + room.ShiftP.x;
                int y = floor.y = room.ShiftP.y;
                roomMap[x, y] = map[x, y];
                roomMapping[x, y] = roomMap;
            }
        }
    }

    #region Container2D
    public override GridSpace this[int x, int y]
    {
        get
        {
            if (x < map.Width && y < map.Height)
            {
                GridSpace space = map[x, y];
                if (space == null)
                { // Create empty gridspace
                    space = new GridSpace(this, GridType.NULL, x, y);
                    map[x, y] = space;
                    return space;
                }
                return space;
            }
            return null;
        }
        set
        {
            map[x, y] = value;
        }
    }

    public override Array2D<GridSpace> Array { get { return map.Array; } }

    public override bool TryGetValue(int x, int y, out GridSpace val)
    {
        if (map.InRange(x, y))
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
        get { return map.Count; }
    }

    public override Bounding Bounding
    {
        get { return map.Bounding; }
    }

    public override bool Contains(int x, int y)
    {
        return map.Contains(x, y);
    }

    public override bool InRange(int x, int y)
    {
        return map.InRange(x, y);
    }

    public override bool DrawAll(DrawAction<GridSpace> call)
    {
        return map.DrawAll(call);
    }

    public override void Clear()
    {
        map.Clear();
    }

    public override Array2DRaw<GridSpace> RawArray(out Point shift)
    {
        return map.RawArray(out shift);
    }

    public override bool Remove(int x, int y)
    {
        return map.Remove(x, y);
    }

    public override void Shift(int x, int y)
    {
        map.Shift(x, y);
    }

    public override IEnumerable<GridSpace> GetEnumerateValues()
    {
        return map.GetEnumerateValues();
    }

    public override IEnumerator<Value2D<GridSpace>> GetEnumerator()
    {
        return map.GetEnumerator();
    }
    #endregion

    public void PlacePlayer()
    {
        BigBoss.Debug.w(Logs.Main, "Placing player in position.");
        Value2D<GridSpace> start;
        RandomPicker<GridSpace> picker;
        this.map.DrawAll(Draw.Walkable<GridSpace>().IfThen(Draw.PickRandom(out picker)));
        if (!picker.Pick(Random, out start))
        {
            throw new ArgumentException("Cannot place player");
        }
        PlacePlayer(start.val);
        BigBoss.Debug.w(Logs.Main, "Placed player on " + start);
    }

    public void PlacePlayer(int x, int y)
    {
        PlacePlayer(this[x, y]);
    }

    public void PlacePlayer(GridSpace space)
    {
        BigBoss.PlayerInfo.transform.position = new Vector3(space.X, 0, space.Y);
        BigBoss.Player.GridSpace = space;
        BigBoss.Player.ForceUpdateTiles(space);
        BigBoss.Levels.Builder.Instantiate(space);
    }
}
