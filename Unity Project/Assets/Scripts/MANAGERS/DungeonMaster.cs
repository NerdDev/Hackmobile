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

    public Value2D<GridSpace> PickSpawnableLocation(Level l)
    {
        MultiMap<GridSpace> room = Spawnable(l.GetRooms().Random(Probability.SpawnRand));
        return room.RandomValue(Probability.SpawnRand);
    }

    public Value2D<GridSpace> PickSpawnableLocation()
    {
        return PickSpawnableLocation(BigBoss.Levels.Level);
    }

    public NPC SpawnNPC(Point p, NPC n)
    {
        if (p == null)
            p = PickSpawnableLocation();
        try
        {
            GameObject gameObject = Instantiate(Resources.Load(n.Prefab), new Vector3(p.x, -.5f, p.y), Quaternion.identity) as GameObject;
            WOInstance instance = gameObject.AddComponent<WOInstance>();
            NPC newNPC = instance.SetTo(n);
            newNPC.IsActive = true;
            newNPC.Init();
            return newNPC;
        }
        catch (ArgumentException)
        {
            throw new ArgumentException("The prefab is null: '" + n.Prefab + "' on NPC " + n.ToString());
        }
    }

    public NPC SpawnNPC(Point p, string npc)
    {
        return SpawnNPC(p, BigBoss.WorldObject.getNPC(npc));
    }

    public NPC SpawnNPC(Point p, Percent variety, params Keywords[] keywords)
    {
        if (Probability.SpawnRand.Percent(variety))
            return SpawnNPC(p);
        else
            return SpawnNPC(p, keywords);
    }

    public NPC SpawnNPC(Point p, params Keywords[] keywords)
    {
        return SpawnNPC(p, (ESFlags<Keywords>) keywords);
    }

    public NPC SpawnNPC(Point p, ESFlags<Keywords> keywords)
    {
        LeveledPool<NPC> pool = GetPool(keywords);
        NPC n = pool.Get();
        if (n == null)
        {
            throw new ArgumentException("NPC Pool was empty for keywords: " + keywords);
        }
        return SpawnNPC(p, n);
    }

    protected LeveledPool<NPC> GetPool(ESFlags<Keywords> keywords)
    {
        LeveledPool<NPC> pool;
        bool empty = keywords.Empty;
        if (!npcPools.TryGetValue(keywords, out pool))
        {
            pool = new LeveledPool<NPC>(DefaultLevelCurve);
            npcPools.Add(keywords, pool);
            foreach (NPC n in BigBoss.WorldObject.getNPCs().Values)
            {
                if (!empty && n.keywords.Contains(keywords) // NPC has keywords
                    || (empty && !n.flags[NPCFlags.NO_RANDOM_SPAWN])) // If keywords empty
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
