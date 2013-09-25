using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DungeonMaster : MonoBehaviour, IManager
{

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
        Value2D<GridSpace> loc = PickStartLocation(l);
        BigBoss.PlayerInfo.transform.position = new Vector3(loc.x, -.5f, loc.y);
        for (int i = 0; i < 4; i++)
        {
            loc = PickStartLocation(l);
            this.SpawnCreature(new Point(loc.x, loc.y), "skeleton_knight");
        }
        for (int i = 0; i < 4; i++)
        {
            loc = PickStartLocation(l);
            this.SpawnCreature(new Point(loc.x, loc.y), "giant_spider");
        }
    }

    void ForcePopulateLevel(Level l)
    {
        l.Populated = true;
        foreach (RoomMap room in l.GetRooms())
        {
            SpawnSpec spec = new SpawnSpec(Probability.SpawnRand, l.Theme, room);
            SpawnModifier mod = SpawnModifier.GetMod();
            mod.Modify(spec);

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

    public void SpawnCreature(Point p, string npc)
    {
        BigBoss.Debug.w(DebugManager.Logs.Main, "Spawning");
        NPC n = BigBoss.WorldObject.getNPC(npc);
        GameObject gameObject = Instantiate(Resources.Load(n.Prefab), new Vector3(p.x, -.5f, p.y), Quaternion.identity) as GameObject;
        NPC newNPC = gameObject.AddComponent<NPC>();
        newNPC.setData(n);
        newNPC.IsActive = true;
        newNPC.init();
    }

    public void SpawnRandomLeveledCreature(Point p)
    {

    }

    public void SpawnCreature(Point p, Percent variety, params Keywords[] keywords)
    {
        if (Probability.SpawnRand.Percent(variety))
            SpawnRandomLeveledCreature(p);
        else
            SpawnCreature(p, keywords);
    }

    public void SpawnCreature(Point p, params Keywords[] keywords)
    {

    }
}
