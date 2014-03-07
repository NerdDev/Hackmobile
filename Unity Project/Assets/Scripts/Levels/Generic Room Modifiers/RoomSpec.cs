using UnityEngine;
using System.Collections;

public class RoomSpec 
{
    public LayoutObject Room { get; protected set; }
    public Container2D<GenSpace> Grids { get; protected set; }
    public int Depth { get; protected set; }
    public Theme Theme { get; protected set; }
    public RoomModCollection RoomModifiers;
    public System.Random Random { get; protected set; }

    public RoomSpec(LayoutObject room, int depth, Theme theme, System.Random random)
    {
        Grids = room.Grids;
        Room = room;
        Depth = depth;
        Theme = theme;
        Random = random;
        RoomModifiers = theme.GetRoomMods();
    }

    public void SetTo(int x, int y, GridType type)
    {
        GenSpace space;
        if (!Grids.TryGetValue(x, y, out space))
        {
            space = new GenSpace(type, Theme);
            Grids[x, y] = space;
        }
        else
        {
            space.Type = type;
        }
    }

    public void SetTo(int x, int y, GenSpace space)
    {
        Grids[x, y] = space;
    }

    public void MergeIn(int x, int y, GenDeploy deploy)
    {
        GenSpace space;
        if (!Grids.TryGetValue(x, y, out space))
        {
            space = new GenSpace(GridType.Floor, Theme);
            Grids[x, y] = space;
        }
        space.AddDeploy(deploy);
    }

    public void MergeIn(int x, int y, GenDeploy deploy, GridType type)
    {
        GenSpace space;
        if (!Grids.TryGetValue(x, y, out space))
        {
            space = new GenSpace(type, Theme);
            Grids[x, y] = space;
        }
        else
        {
            space.Type = type;
        }
        space.AddDeploy(deploy);
    }

    public void MergeIn(int x, int y, ThemeElement element)
    {
        MergeIn(x, y, new GenDeploy(element));
    }

    public void MergeIn(int x, int y, ThemeElement element, GridType type)
    {
        MergeIn(x, y, new GenDeploy(element), type);
    }
}
