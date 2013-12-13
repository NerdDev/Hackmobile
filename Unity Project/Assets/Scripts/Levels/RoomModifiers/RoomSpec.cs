using UnityEngine;
using System.Collections;

public class RoomSpec {

    public LayoutObjectLeaf Room { get; protected set; }
    public GridType[,] Array { get; protected set; }
    public int Depth { get; protected set; }
    public Theme Theme { get; protected set; }
    public RandomGen Random { get; protected set; }

    public RoomSpec(LayoutObjectLeaf room, int depth, Theme theme, RandomGen random)
    {
        Room = room;
        Array = room.GetArray().GetArr();
        Depth = depth;
        Theme = theme;
        Random = random;
    }
}
