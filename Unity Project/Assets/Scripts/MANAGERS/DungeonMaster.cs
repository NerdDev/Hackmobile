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

    public GridSpace PickSpawnableLocation(Level l)
    {
        MultiMap<GridSpace> room = Spawnable(l.GetRooms().Random(Probability.SpawnRand));
        return room.RandomValue(Probability.SpawnRand).val;
    }

    public GridSpace PickSpawnableLocation()
    {
        return PickSpawnableLocation(BigBoss.Levels.Level);
    }

    public NPC SpawnNPC(GridSpace g, NPC n)
    {
        if (g == null)
            g = PickSpawnableLocation();
        try
        {
            return BigBoss.Objects.NPCs.Instantiate(n, g);
        }
        catch (ArgumentException)
        {
            throw new ArgumentException("The prefab is null: '" + n.Prefab + "' on NPC " + n.ToString());
        }
    }

    public NPC SpawnNPC(GridSpace g, string npc)
    {
        return SpawnNPC(g, BigBoss.Objects.NPCs.GetPrototype(npc));
    }

    public NPC SpawnNPC(GridSpace g, Percent variety, params Keywords[] keywords)
    {
        if (Probability.SpawnRand.Percent(variety))
            return SpawnNPC(g);
        else
            return SpawnNPC(g, keywords);
    }

    public NPC SpawnNPC(GridSpace g, params Keywords[] keywords)
    {
        return SpawnNPC(g, (ESFlags<Keywords>) keywords);
    }

    public NPC SpawnNPC(GridSpace g, ESFlags<Keywords> keywords)
    {
        LeveledPool<NPC> pool = GetPool(keywords);
        NPC n = pool.Get();
        if (n == null)
        {
            throw new ArgumentException("NPC Pool was empty for keywords: " + keywords);
        }
        return SpawnNPC(g, n);
    }

    protected LeveledPool<NPC> GetPool(ESFlags<Keywords> keywords)
    {
        LeveledPool<NPC> pool;
        bool empty = keywords.Empty;
        if (!npcPools.TryGetValue(keywords, out pool))
        {
            pool = new LeveledPool<NPC>(DefaultLevelCurve);
            npcPools.Add(keywords, pool);
            foreach (NPC n in BigBoss.Objects.NPCs.Prototypes)
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
