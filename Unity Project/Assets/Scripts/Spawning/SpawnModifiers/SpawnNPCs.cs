using UnityEngine;
using System.Collections;

public class SpawnNPCs : SpawnModifier {

    public override void Modify(RandomGen rand, RoomMap room, params Keywords[] keywords)
    {
        MultiMap<GridSpace> spawnable = room.Spawnable();
        Value2D<GridSpace> space = spawnable.RandomValue(rand);
        BigBoss.DungeonMaster.SpawnCreature(space);
    }

    public override bool ShouldSpawn(RoomMap room, params Keywords[] keywords)
    {
        return true;
    }
}
