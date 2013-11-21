using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class DungeonMaster : MonoBehaviour, IManager {

    Dictionary<ESFlags<Keywords>, LeveledPool<NPC>> npcPools = new Dictionary<ESFlags<Keywords>, LeveledPool<NPC>>();

    public List<GameObject> stairs = new List<GameObject>();

    public void Initialize()
    {
        SpawnModifier.RegisterModifiers();
    }

    public void PopulateLevel(Level l, bool up)
    {
        if (!l.Populated)
        {
            //ForcePopulateLevel(l);
        }

        //Place stairs
        ClearPriorLevel();
        GridSpace stairsDown = CreateStairs(l, "StairsDown", PrimitiveType.Sphere);
        GridSpace stairsUp = CreateStairs(l, "StairsUp", PrimitiveType.Cylinder);

        //Place Player
        if (up)
        {
            PlacePlayer(l, stairsDown);
        }
        else
        {
            PlacePlayer(l, stairsUp);
        }
    }

    private void ClearPriorLevel()
    {
        if (stairs.Count > 0)
        {
            foreach (GameObject go in stairs)
            {
                Destroy(go);
            }
        }
        BigBoss.Objects.NPCs.DestroyWrappers();
    }

    public GridSpace CreateStairs(Level l, string name, PrimitiveType prim)
    {
        GridSpace locStairs = BigBoss.DungeonMaster.PickSpawnableLocation(l);
        GameObject stairs = GameObject.CreatePrimitive(prim);
        stairs.name = name;
        stairs.transform.position = new Vector3(locStairs.X, 0f, locStairs.Y);
        locStairs.Block.layer = 13;
        locStairs.Block.collider.isTrigger = false;
        locStairs.Block.name = name;
        this.stairs.Add(stairs);
        return locStairs;
    }

    public void PlacePlayer(Level l, GridSpace stairsUp)
    {
        Value2D<GridSpace> grid = l.Array.GetAround(stairsUp.X, stairsUp.Y, false, (arr, x, y) =>
            {
                return arr[y, x].Type == GridType.Floor;
            });
        BigBoss.PlayerInfo.transform.position = new Vector3(grid.x, -.5f, grid.y);
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
        Value2D<GridSpace> pick = room.RandomValue(Probability.SpawnRand);
        return pick.val;
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
