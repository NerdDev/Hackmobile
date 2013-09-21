using UnityEngine;
using System.Collections;

public class RoomSpec {

    public Room Room { get; protected set; }
    public int Depth { get; protected set; }
    public Theme Theme { get; protected set; }
    public RandomGen Random { get; protected set; }

    public RoomSpec(Room room, int depth, Theme theme, RandomGen random)
    {
        Room = room;
        Depth = depth;
        Theme = theme;
        Random = random;
    }
}
