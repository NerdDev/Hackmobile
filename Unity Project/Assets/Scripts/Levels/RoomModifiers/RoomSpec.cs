using UnityEngine;
using System.Collections;

public class RoomSpec {

    public LayoutObjectLeaf Room { get; protected set; }
    public Container2D<GridType> Array { get; protected set; }
    public int Depth { get; protected set; }
    public Theme Theme { get; protected set; }
    public System.Random Random { get; protected set; }

    public RoomSpec(LayoutObjectLeaf room, int depth, Theme theme, System.Random random)
    {
        Room = room;
        Depth = depth;
        Theme = theme;
        Random = random;
    }
}
