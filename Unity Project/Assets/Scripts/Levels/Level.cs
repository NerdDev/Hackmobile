using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Level : LayoutObject<GridSpace>
{
    public bool Populated;
    public System.Random Random;
    public HashSet<WorldObject> WorldObjects = new HashSet<WorldObject>();
    public Dictionary<Theme, List<LayoutObject<GenSpace>>> RoomsByTheme;
    public Container2D<AreaBatchMapper> BatchMapper = new MultiMap<AreaBatchMapper>();

    public Level()
        : base(LayoutObjectType.Layout)
    {
    }

    public override GridSpace this[int x, int y]
    {
        get
        {
            GridSpace space = base[x, y];
            if (space == null)
            { // Create empty gridspace
                space = new GridSpace(this, GridType.NULL, x, y);
                base[x, y] = space;
                return space;
            }
            return space;
        }
        set
        {
            base[x, y] = value;
        }
    }

    public void PlacePlayer()
    {
        BigBoss.Debug.w(Logs.Main, "Placing player in position.");
        Value2D<GridSpace> start;
        if (!BigBoss.DungeonMaster.PickSpawnableLocation(this, out start))
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
