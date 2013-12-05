using UnityEngine;
using System.Collections;

public class SpawnNPCs : SpawnModifier {

    public override bool Modify(SpawnSpec spec)
    {
        MultiMap<GridSpace> spawnable = spec.Room.Spawnable();
        Point<GridSpace> space = spawnable.RandomValue(spec.Random);
        BigBoss.DungeonMaster.SpawnNPC(space.val, spec.Theme.Keywords);
        return true;
    }

    public override bool ShouldSpawn(RoomMap room, params Keywords[] keywords)
    {
        return true;
    }
}
