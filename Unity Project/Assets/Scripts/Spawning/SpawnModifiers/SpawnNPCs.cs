using UnityEngine;
using System.Collections;

public class SpawnNPCs : SpawnModifier {

    public override void Modify(RoomMap room, RandomGen rand)
    {
        MultiMap<GridSpace> spawnable = room.Spawnable();
        Value2D<GridSpace> space = spawnable.RandomValue(rand);
        BigBoss.DungeonMaster.SpawnCreature("skeleton_knight", space.x, space.y);
    }
}
