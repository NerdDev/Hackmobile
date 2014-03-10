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
}
