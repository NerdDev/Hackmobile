using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DungeonMaster : MonoBehaviour, IManager {

    public void Initialize()
    {
    }

    public void PopulateLevel(Level l)
    {
        if (!l.Populated)
        {
            ForcePopulateLevel(l);
        }
    }

    void ForcePopulateLevel(Level l)
    {
        l.Populated = true;
        foreach (MultiMap<GridSpace> room in l.GetRooms())
        {
            MultiMap<GridSpace> spawn = OnlySpawnable(room);
            Value2D<GridSpace> space = spawn.RandomValue(Probability.SpawnRand);
            SpawnCreature("skeleton_knight", space.x, space.y);
        }
    }

    public MultiMap<GridSpace> OnlySpawnable(MultiMap<GridSpace> map)
    {
        MultiMap<GridSpace> ret = new MultiMap<GridSpace>();
        foreach (Value2D<GridSpace> space in map)
        {
            if (space.val.Spawnable)
                ret.Put(space);
        }
        return ret;
    }

    void PickStartLocation(Level l)
    {
    }

    public void SpawnCreature(string npc, int x, int y)
    {
        BigBoss.Debug.w(DebugManager.Logs.Main, "Spawning");
        NPC n = BigBoss.WorldObject.getNPC(npc);
        GameObject gameObject = Instantiate(Resources.Load(n.Prefab), new Vector3(x, -.5f, y), Quaternion.identity) as GameObject;
        NPC newNPC = gameObject.AddComponent<NPC>();
        newNPC.setData(n);
        newNPC.IsActive = true;
        newNPC.init();
    }
}
