using UnityEngine;
using System.Collections;

public class SpawnSpec {

    public System.Random Random { get; protected set; }
    public MultiMap<GridSpace> Room { get; protected set; }

    public SpawnSpec(System.Random rand, MultiMap<GridSpace> room)
    {
        Random = rand;
        Room = room;
    }
}
