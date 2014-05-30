using UnityEngine;
using System.Collections;

public class RoomSpec 
{
    public LayoutObject<GenSpace> Room { get; protected set; }
    public Container2D<GenSpace> Grids { get { return Room; } }
    public int Depth { get; protected set; }
    public Theme Theme { get; protected set; }
    public RoomModCollection RoomModifiers;
    public System.Random Random { get; protected set; }

    public RoomSpec(LayoutObject<GenSpace> room, int depth, Theme theme, System.Random random)
    {
        Room = room;
        Depth = depth;
        Theme = theme;
        Random = random;
        RoomModifiers = theme.RoomMods;
    }
}
