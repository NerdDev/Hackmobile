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
        Value2D<GridSpace> stairsDown = CreateStairs("StairsDown", PrimitiveType.Sphere);
        Value2D<GridSpace> stairsUp = CreateStairs("StairsUp", PrimitiveType.Cylinder);

        //Place Player
        if (up)
        {
            PlacePlayer(stairsDown);
        }
        else
        {
            PlacePlayer(stairsUp);
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
        BigBoss.WorldObject.ClearNPCs();
    }

    public Value2D<GridSpace> CreateStairs(string name, PrimitiveType prim)
    {
        Value2D<GridSpace> locStairs = BigBoss.DungeonMaster.PickStartLocation(LevelManager.Level);
        GameObject stairs = GameObject.CreatePrimitive(prim);
        stairs.name = name;
        stairs.transform.position = new Vector3(locStairs.x, 0f, locStairs.y);
        locStairs.val.Block.layer = 13;
        locStairs.val.Block.collider.isTrigger = false;
        locStairs.val.Block.name = name;
        this.stairs.Add(stairs);
        return locStairs;
    }

    public void PlacePlayer(Value2D<GridSpace> stairsUp)
    {
        IEnumerable<Value2D<GridSpace>> grids = LevelManager.Level.getSurroundingSpaces(stairsUp.x, stairsUp.y);
        //List<Value2D<GridSpace>> gridList = grids.ToList();
        Value2D<GridSpace> grid = grids.First(g => g.val.Type == GridType.Floor);
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

    public Value2D<GridSpace> PickStartLocation(Level l)
    {
        MultiMap<GridSpace> room = Spawnable(l.GetRooms().Random(Probability.SpawnRand));
        return room.RandomValue(Probability.SpawnRand);
    }

    public void SpawnCreature(Point p, NPC n)
    {
        try
        {
            GameObject gameObject = Instantiate(Resources.Load(n.Prefab), new Vector3(p.x, -.5f, p.y), Quaternion.identity) as GameObject;
            NPC newNPC = gameObject.AddComponent<NPC>();
            newNPC.setData(n);
            newNPC.IsActive = true;
            newNPC.init();
        }
        catch (ArgumentException arg)
        {
            //continue
        }
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
