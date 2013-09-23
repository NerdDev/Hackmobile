using UnityEngine;
using System.Collections;

public class SpawnNPCs : SpawnModifier {

    public override bool Modify(SpawnSpec spec)
    {
        MultiMap<GridSpace> spawnable = spec.Room.Spawnable();
        Value2D<GridSpace> space = spawnable.RandomValue(spec.Random);
        BigBoss.DungeonMaster.SpawnCreature(space);
        return true;
    }

    public override bool ShouldSpawn(RoomMap room, params Keywords[] keywords)
    {
        return true;
    }
}
