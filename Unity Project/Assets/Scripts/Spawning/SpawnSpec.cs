using UnityEngine;
using System.Collections;

public class SpawnSpec {

    public RandomGen Random { get; protected set; }
    public Theme Theme { get; protected set; }
    public RoomMap Room { get; protected set; }

    public SpawnSpec(RandomGen rand, Theme theme, RoomMap room)
    {
        Random = rand;
        Theme = theme;
        Room = room;
    }
}
