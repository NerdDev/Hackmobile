using UnityEngine;
using System.Collections;

public class SpawnNPCs : SpawnModifier {

    public override bool Modify(SpawnSpec spec)
    {
        MultiMap<GridSpace> spawnable = spec.Room.Spawnable();
        Value2D<GridSpace> space = spawnable.Random(spec.Random);
        if (space == null || space.val == null) return false;
        BigBoss.DungeonMaster.SpawnNPC(space.val, spec.Theme.Keywords);
        return true;
    }

    public override bool ShouldSpawn(RoomMap room, params Keywords[] keywords)
    {
        return true;
    }
}
