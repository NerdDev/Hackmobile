using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class DungeonMaster : MonoBehaviour, IManager {

    public bool Initialized { get; set; }
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

    public void PlacePlayer(Level l, GridSpace stairsUp)
    {
        Value2D<GridSpace> grid;
        l.Array.GetPointAround(stairsUp.X, stairsUp.Y, false, (arr, x, y) =>
            {
                return arr[x, y].Type == GridType.Floor;
            }, out grid);
        BigBoss.PlayerInfo.transform.position = new Vector3(grid.x, 0, grid.y);
    }

    void ForcePopulateLevel(Level l)
    {
        l.Populated = true;
        foreach (Container2D<GridType> room in l.RoomMaps)
        {
            MultiMap<GridSpace> roomMap = new MultiMap<GridSpace>();
            room.DrawAll((arr, x, y) =>
                {
                    roomMap[x, y] = l[x, y];
                    return true;
                });
            SpawnSpec spec = new SpawnSpec(Probability.SpawnRand, l.Theme, roomMap);
            SpawnModifier mod = SpawnModifier.GetMod();
            mod.Modify(spec);
        }
    }

    public static MultiMap<GridSpace> Spawnable<T>(Level l, IEnumerable<Value2D<T>> points)
    {
        return Spawnable(l, points.Cast<Point>());
    }

    public static MultiMap<GridSpace> Spawnable(Level l, IEnumerable<Point> points)
    {
        MultiMap<GridSpace> ret = new MultiMap<GridSpace>();
        foreach (Point p in points)
        {
            GridSpace s = l[p];
            if (s.Spawnable)
                ret[p] = s;
        }
        return ret;
    }

    public bool PickSpawnableLocation(Level l, out Value2D<GridSpace> pick)
    {
        MultiMap<GridSpace> room = Spawnable(l, l.RoomMaps.Random(Probability.SpawnRand));
        return room.Random(Probability.SpawnRand, out pick);
    }

    public bool PickSpawnableLocation(out Value2D<GridSpace> pick)
    {
        return PickSpawnableLocation(BigBoss.Levels.Level, out pick);
    }

    public NPC SpawnNPC(GridSpace g, NPC n)
    {
        try
        {
            return BigBoss.Objects.NPCs.InstantiateAndWrap(n, g);
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
        NPC n;
        if (!pool.Get(Probability.SpawnRand, out n, BigBoss.Player.Level))
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
                if (!empty && n.Keywords.Contains(keywords) // NPC has keywords
                    || (empty && !n.Flags[NPCFlags.NO_RANDOM_SPAWN])) // If keywords empty
                {
                    pool.Add(n);
                }
            }
        }
        return pool;
    }

    protected double DefaultLevelCurve(ushort gravity, ushort entry)
    {
        return 1d;
    }
}
