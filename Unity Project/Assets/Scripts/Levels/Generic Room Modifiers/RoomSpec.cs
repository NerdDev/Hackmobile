using UnityEngine;
using System.Collections;

public class RoomSpec 
{
    public Container2D<GenSpace> Grids { get; set; }
    public int Depth { get; protected set; }
    public Theme Theme { get; protected set; }
    public RoomModCollection RoomModifiers;
    public System.Random Random { get; protected set; }

    public RoomSpec(LayoutObject room, int depth, Theme theme, System.Random random)
    {
        Grids = room.Grids;
        Depth = depth;
        Theme = theme;
        Random = random;
        RoomModifiers = theme.GetRoomMods();
    }
}
