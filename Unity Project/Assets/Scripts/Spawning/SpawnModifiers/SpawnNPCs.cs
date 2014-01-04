using UnityEngine;
using System.Collections;
using System;

public class SpawnNPCs : SpawnModifier {

    public override bool Modify(SpawnSpec spec)
    {
        MultiMap<GridSpace> spawnable = new MultiMap<GridSpace>();
        spec.Room.DrawAll(Draw.If<GridSpace>((g) => g.Spawnable).And(Draw.AddTo(spawnable)));
        Value2D<GridSpace> space = spawnable.Random(spec.Random);
        if (space == null || space.val == null) return false;
        BigBoss.DungeonMaster.SpawnNPC(space.val, spec.Theme.Keywords);
        return true;
    }

    public override bool ShouldSpawn(SpawnSpec spec)
    {
        return true;
    }
}
