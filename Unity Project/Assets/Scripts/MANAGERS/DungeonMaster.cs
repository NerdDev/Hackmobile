using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DungeonMaster : MonoBehaviour, IManager {

    public void Initialize()
    {
        SpawnModifier.RegisterModifiers();
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
        foreach (RoomMap room in l.GetRooms())
        {
            SpawnModifier mod = SpawnModifier.GetMod();
            mod.Modify(room, Probability.SpawnRand);
        }
    }

    public static MultiMap<GridSpace> Spawnable(MultiMap<GridSpace> map)
    {
        MultiMap<GridSpace> ret = new MultiMap<GridSpace>();
        foreach (Value2D<GridSpace> space in map)
        {
            if (space.val.Spawnable)
                ret.Put(space);
        }
        return ret;
    }

    public Value2D<GridSpace> PickStartLocation(Level l)
    {
        MultiMap<GridSpace> room = Spawnable(l.GetRooms().Random(Probability.SpawnRand));
        return room.RandomValue(Probability.SpawnRand);
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
