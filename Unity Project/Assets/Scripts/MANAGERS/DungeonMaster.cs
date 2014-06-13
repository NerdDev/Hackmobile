using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class DungeonMaster : MonoBehaviour, IManager
{
    public bool Initialized { get; set; }
    Dictionary<GenericFlags<SpawnKeywords>, LeveledPool<NPC>> npcPools = new Dictionary<GenericFlags<SpawnKeywords>, LeveledPool<NPC>>();

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
        // Spawn room spawn mods
        foreach (LayoutObject<GridSpace> room in l.Flatten(LayoutObjectType.Room))
        {
            #region DEBUG
            if (BigBoss.Debug.logging(Logs.Spawning))
            {
                BigBoss.Debug.CreateNewLog(Logs.Spawning, "Spawn Level" + l.Id + "/" + room);
            }
            #endregion
            SpawnSpec spec = new SpawnSpec(l.Random, room);
            #region DEBUG
            if (BigBoss.Debug.logging(Logs.Spawning))
            {
                spec.Container.ToLog(Logs.Spawning, "Area spawning in");
                spec.Spawnable.ToLog(Logs.Spawning, "Spawnable Spaces");
            }
            #endregion
            var roomSpawnMod = room.Theme.SpawnMods.RoomMods.Get(l.Random);
            try
            {
                roomSpawnMod.Modify(spec);
            }
            catch (Exception ex)
            {
                BigBoss.Debug.w(Logs.Spawning, "Error spawning room spawn mod " + ex);
                BigBoss.Debug.w(Logs.Main, "Error spawning room spawn mod " + ex);
            }
        }
        // Spawn area spawn mods
        int numAreas = 0;
        foreach (var pair in l.RoomsByTheme)
        {
            #region DEBUG
            if (BigBoss.Debug.logging(Logs.Spawning))
            {
                BigBoss.Debug.CreateNewLog(Logs.Spawning, "Spawn Level" + l.Id + "/Area Spawn " + numAreas++);
            }
            #endregion
            var theme = pair.Key;
            var rooms = pair.Value;
            int numAreaSpawnMods = l.Random.NextNormalDist(theme.MinAreaMods, theme.MaxAreaMods);
            theme.SpawnMods.AreaMods.BeginTaking();
            for (int i = 0 ; i < numAreaSpawnMods ; i++)
            {
                SpawnMod mod;
                if (!theme.SpawnMods.AreaMods.Get(l.Random, out mod))
                {
                    break;
                }
                MultiMap<GridSpace> cont = new MultiMap<GridSpace>();
                foreach (var room in rooms)
                {
                    room.DrawAll(Draw.AddTo<GenSpace, GridSpace>(cont, l));
                }
                SpawnSpec areaSpec = new SpawnSpec(l.Random, cont);
                #region DEBUG
                if (BigBoss.Debug.logging(Logs.Spawning))
                {
                    areaSpec.Container.ToLog(Logs.Spawning, "Area spawning in");
                    areaSpec.Spawnable.ToLog(Logs.Spawning, "Spawnable Spaces");
                }
                #endregion
                try
                {
                    mod.Modify(areaSpec);
                } 
                catch (Exception ex)
                {
                    BigBoss.Debug.w(Logs.Spawning, "Error spawning area spawn mod " + ex);
                    BigBoss.Debug.w(Logs.Main, "Error spawning area spawn mod " + ex);
                }
            }
            theme.SpawnMods.AreaMods.EndTaking();
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
            {
                ret[p] = s;
            }
        }
        return ret;
    }

    public bool PickSpawnableLocation(Level l, out Value2D<GridSpace> pick)
    {
        RandomPicker<GridSpace> picker;
        l.DrawAll(Draw.Walkable<GridSpace>().IfThen(Draw.PickRandom(out picker)));
        return picker.Pick(l.Random, out pick);
    }

    public bool PickSpawnableLocation(out Value2D<GridSpace> pick)
    {
        return PickSpawnableLocation(BigBoss.Levels.Level, out pick);
    }

    public NPC SpawnNPC(GridSpace g, NPC n)
    {
        NPC ret;
        SpawnNPC(g, n, out ret);
        return ret;
    }

    public bool SpawnNPC(GridSpace g, NPC n, out NPC instance)
    {
        return BigBoss.Objects.NPCs.InstantiateAndWrap(n, g, out instance);
    }

    public NPC SpawnNPC(GridSpace g, string npc)
    {
        NPC ret;
        SpawnNPC(g, npc, out ret);
        return ret;
    }

    public bool SpawnNPC(GridSpace g, string npc, out NPC instance)
    {
        return SpawnNPC(g, BigBoss.Objects.NPCs.GetPrototype(npc), out instance);
    }

    public NPC SpawnNPC(GridSpace g, double varietyPercent, params SpawnKeywords[] keywords)
    {
        if (Probability.SpawnRand.Percent(varietyPercent))
        {
            return SpawnNPC(g);
        }
        else
        {
            return SpawnNPC(g, keywords);
        }
    }

    public NPC SpawnNPC(GridSpace g, params SpawnKeywords[] keywords)
    {
        NPC ret;
        SpawnNPC(g, new GenericFlags<SpawnKeywords>(keywords), out ret);
        return ret;
    }

    public bool SpawnNPC(GridSpace g, out NPC ret, params SpawnKeywords[] keywords)
    {
        return SpawnNPC(g, new GenericFlags<SpawnKeywords>(keywords), out ret);
    }

    public NPC SpawnNPC(GridSpace g, GenericFlags<SpawnKeywords> keywords)
    {
        NPC ret;
        SpawnNPC(g, keywords, out ret);
        return ret;
    }

    public bool SpawnNPC(GridSpace g, GenericFlags<SpawnKeywords> keywords, out NPC ret)
    {
        LeveledPool<NPC> pool = GetPool(keywords);
        NPC n;
        if (!pool.Get(Probability.SpawnRand, out n, BigBoss.Player.Level))
        {
            throw new ArgumentException("NPC Pool was empty for keywords: " + keywords);
        }
        return SpawnNPC(g, n, out ret);
    }

    protected LeveledPool<NPC> GetPool(GenericFlags<SpawnKeywords> keywords)
    {
        LeveledPool<NPC> pool;
        if (!npcPools.TryGetValue(keywords, out pool))
        {
            #region DEBUG
            if (BigBoss.Debug.logging(Logs.NPCs))
            {
                BigBoss.Debug.printHeader(Logs.NPCs, "Get Pool");
                BigBoss.Debug.w(Logs.NPCs, "Keywords:");
                BigBoss.Debug.incrementDepth(Logs.NPCs);
                keywords.ToLog(Logs.NPCs);
                BigBoss.Debug.decrementDepth(Logs.NPCs);
            }
            #endregion
            pool = new LeveledPool<NPC>(DefaultLevelCurve);
            npcPools.Add(keywords, pool);
            foreach (NPC n in BigBoss.Objects.NPCs.Prototypes)
            {
                if (!keywords.Empty && n.SpawnKeywords.Contains(keywords) // NPC has keywords
                    || (keywords.Empty && !n.Flags[NPCFlags.NO_RANDOM_SPAWN])) // If keywords empty
                {
                    pool.Add(n);
                }
            }
            #region DEBUG
            if (BigBoss.Debug.logging(Logs.NPCs))
            {
                pool.ToLog(BigBoss.Debug.Get(Logs.NPCs), "NPCs");
                BigBoss.Debug.printFooter(Logs.NPCs, "Get Pool");
            }
            #endregion
        }
        return pool;
    }

    protected double DefaultLevelCurve(ushort gravity, ushort entry)
    {
        return 1d;
    }
}
