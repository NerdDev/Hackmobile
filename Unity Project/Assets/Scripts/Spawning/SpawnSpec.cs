using UnityEngine;
using System.Collections;

public class SpawnSpec {

    public System.Random Random { get; protected set; }
    public Theme Theme { get; protected set; }
    public RoomMap Room { get; protected set; }

    public SpawnSpec(System.Random rand, Theme theme, RoomMap room)
    {
        Random = rand;
        Theme = theme;
        Room = room;
    }
}
