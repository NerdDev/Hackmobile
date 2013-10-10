using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class DungeonMaster : MonoBehaviour, IManager {

    Dictionary<ESFlags<Keywords>, LeveledPool<NPC>> npcPools = new Dictionary<ESFlags<Keywords>, LeveledPool<NPC>>();

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

    public void SpawnCreature(Point p, NPC n)
    {
        GameObject gameObject = Instantiate(Resources.Load(n.Prefab), new Vector3(p.x, -.5f, p.y), Quaternion.identity) as GameObject;
        NPC newNPC = gameObject.AddComponent<NPC>();
        newNPC.setData(n);
        newNPC.IsActive = true;
        newNPC.init();
    }

    public void SpawnCreature(Point p, string npc)
    {
        SpawnCreature(p, BigBoss.WorldObject.getNPC(npc));
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
        SpawnCreature(p, (ESFlags<Keywords>) keywords);
    }

    public void SpawnCreature(Point p, ESFlags<Keywords> keywords)
    {
        LeveledPool<NPC> pool = GetPool(keywords);
        NPC n = pool.Get();
        if (n == null)
        {
            throw new ArgumentException("NPC Pool was empty for keywords: " + keywords);
        }
        SpawnCreature(p, n);
    }

    protected LeveledPool<NPC> GetPool(ESFlags<Keywords> keywords)
    {
        LeveledPool<NPC> pool;
        if (!npcPools.TryGetValue(keywords, out pool))
        {
            pool = new LeveledPool<NPC>(DefaultLevelCurve);
            npcPools.Add(keywords, pool);
            foreach (NPC n in BigBoss.WorldObject.getNPCs().Values)
            {
                if (n.keywords.Contains(keywords))
                {
                    pool.Add(n);
                }
            }
        }
        return pool;
    }

    protected float DefaultLevelCurve(int level)
    {
        return 1F;
    }
}
