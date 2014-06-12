using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;

public class SpawnNPCs : SpawnMod
{
    public double SpawnRatio = .01d;
    public double Variance = .25d;

    public override bool Modify(SpawnSpec spec)
    {
        double numToSpawn = spec.Spawnable.Count * SpawnRatio;
        double var = numToSpawn * Variance;
        if (spec.Random.NextBool())
        {
            var *= -1;
        }
        numToSpawn += var;
        int toSpawn = (int)Math.Round(numToSpawn);
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.Spawning))
        {
            BigBoss.Debug.printHeader(Logs.Spawning, "Spawn NPCs");
            BigBoss.Debug.w(Logs.Spawning, "Number of spawnable spaces: " + spec.Spawnable.Count);
            BigBoss.Debug.w(Logs.Spawning, "Spawn Ratio: " + SpawnRatio);
            BigBoss.Debug.w(Logs.Spawning, "Num To Spawn: " + (spec.Spawnable.Count * SpawnRatio));
            BigBoss.Debug.w(Logs.Spawning, "Variance: " + Variance + " -> " + var);
            BigBoss.Debug.w(Logs.Spawning, "After Variance: " + numToSpawn);
            BigBoss.Debug.w(Logs.Spawning, "Final: " + toSpawn);
        }
        #endregion
        List<Value2D<GridSpace>> list = new List<Value2D<GridSpace>>(toSpawn);
        for (int i = 0; i < toSpawn; i++)
        {
            Value2D<GridSpace> space;
            if (!spec.Spawnable.GetRandom(spec.Random, out space)) return false;
            NPC npc;
            if (!BigBoss.DungeonMaster.SpawnNPC(space.val, out npc, space.val.Theme.Keywords))
            {
                #region DEBUG
                if (BigBoss.Debug.logging())
                {
                    BigBoss.Debug.w(Logs.Main, "Failed to spawn NPC with keywords: " + space.val.Theme.Keywords);
                    BigBoss.Debug.w(Logs.Spawning, "Failed to spawn NPC with keywords: " + space.val.Theme.Keywords);
                }
                #endregion
            }
            else
            {
                list.Add(space);
            }
            #region DEBUG
            if (BigBoss.Debug.logging(Logs.Spawning))
            {
                BigBoss.Debug.w(Logs.Spawning, "Spawned: " + npc + " at " + space);
            }
            #endregion
        }
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.Spawning))
        {
            MultiMap<GridSpace> tmp = new MultiMap<GridSpace>();
            foreach (var v in list)
            {
                tmp[v] = v.val;
            }
            spec.Container.ToLog(BigBoss.Debug.Get(Logs.Spawning), tmp, null, '*', "Spawn Points");
            BigBoss.Debug.printFooter(Logs.Spawning, "Spawn NPCs");
        }
        #endregion
        return true;
    }

    public override bool ShouldSpawn(SpawnSpec spec)
    {
        return true;
    }
}
