using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class LevelManager : MonoBehaviour, IManager
{
    public Level Level { get; private set; }
    public bool Initialized { get; set; }
    public Theme TestTheme;
    private ProbabilityPool<ThemeSet> themeSets = ProbabilityPool<ThemeSet>.Create();
    public PrefabProbabilityContainer[] ThemeSets;
    public LevelBuilder Builder;
    public int Seed = -1;
    public bool UseInitialTheme;
    public Theme InitialTheme;
    // Number of areas
    public int MinAreas = 1;
    public int MaxAreas = 2;

    [Serializable]
    public class PrefabProbabilityContainer
    {
        public float Multiplier = 1f;
        public ThemeSet Set;
    }

    public void Initialize()
    {
        ArrayExt.Converters[typeof(GridType)] = (b) => { return GridTypeEnum.Convert((GridType)b); };
        ArrayExt.Converters[typeof(GridSpace)] = (b) =>
        {
            GridSpace s = b as GridSpace;
            if (s == null) return GridTypeEnum.Convert(GridType.NULL);
            return GridTypeEnum.Convert(s.Type);
        };
        ArrayExt.Converters[typeof(GenSpace)] = (b) =>
        {
            GenSpace s = b as GenSpace;
            if (s == null) return GridTypeEnum.Convert(GridType.NULL);
            return s.GetChar();
        };
        ArrayExt.Converters[typeof(GridTypeObj)] = (b) =>
        {
            GridTypeObj s = b as GridTypeObj;
            if (s == null) return GridTypeEnum.Convert(GridType.NULL);
            return GridTypeEnum.Convert(s.Type);
        };
        ArrayExt.Converters[typeof(IGridType)] = (b) =>
        {
            IGridType s = b as IGridType;
            if (s == null) return GridTypeEnum.Convert(GridType.NULL);
            return GridTypeEnum.Convert(s.Type);
        };
        ArrayExt.Converters[typeof(IGridSpace)] = (b) =>
        {
            IGridSpace s = b as IGridSpace;
            if (s == null) return GridTypeEnum.Convert(GridType.NULL);
            return s.GetChar();
        };
        if (Seed == -1)
        {
            Seed = Probability.Rand.Next();
        }
        themeSets = ProbabilityPool<ThemeSet>.Create();
        foreach (IInitializable init in this.FindAllDerivedObjects<IInitializable>())
        {
            init.Init();
        }
        foreach (PrefabProbabilityContainer cont in ThemeSets)
        {
            if (cont.Set == null)
            {
                throw new ArgumentException("Prefab has to be not null");
            }
            if (cont.Multiplier <= 0)
            {
                cont.Multiplier = 1f;
            }
            themeSets.Add(cont.Set, cont.Multiplier);
        }
        if (InitialTheme == null || !UseInitialTheme)
        {
            InitialTheme = themeSets.Get(Probability.Rand).GetTheme(Probability.Rand);
        }
        LevelBuilder.Initialize();
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGenMain))
        {
            BigBoss.Debug.w(Logs.LevelGenMain, "Random seed int: " + Seed);
        }
        #endregion
    }

    void Start()
    {
    }

    public void SetFirstLevel()
    {
        Level level = GenerateLevel(Seed, InitialTheme, 0);
        SetCurLevel(level);
    }

    public void SetCurLevel(Level level)
    {
        if (level.Equals(Level)) return;
        Destroy(Level);
        #region DEBUG
        if (BigBoss.Debug.logging())
        {
            level.ToLog(Logs.Main, "Setting level to");
        }
        #endregion
        DeployLevel(level);
        BigBoss.DungeonMaster.PopulateLevel(Level);
        Level.PlacePlayer();
    }

    protected void DeployLevel(Level level)
    {
        Deploy(level);
        Level = level;
        AstarPath.active.Scan();
    }

    void Destroy(Level level)
    {
        if (level == null) return;
        foreach (GridSpace val in level.GetEnumerateValues())
        {
            val.WrapObjects(false);
            val.DestroyGridSpace();
        }
    }

    void Deploy(Level level)
    {
        BigBoss.Debug.w(Logs.LevelGenMain, "Deploying " + level);
        foreach (GridSpace space in level.GetEnumerateValues())
        {
            space.WrapObjects(true);
        }
        BigBoss.Debug.w(Logs.LevelGenMain, "Deployed " + level);
    }

    public Level GenerateLevel(int seed, Theme startingTheme, int depth)
    {
        themeSets.Freshen();
        LevelGenerator gen = new LevelGenerator();
        gen.DebugTheme = startingTheme;
        gen.ThemeSetOptions = themeSets;
        gen.Depth = depth;
        gen.Rand = new System.Random(seed);
        LevelLayout layout = gen.Generate();
        Level level = GenerateFromLayout(layout, gen.Rand);
        return level;
    }

    Level GenerateFromLayout(LevelLayout layout, System.Random rand)
    {
        LevelGenerator.ConfirmEdges(layout);
        Level level = new Level()
        {
            RoomsByTheme = layout.RoomsByTheme
        };
        MultiMap<GridSpace> spaces = Builder.GeneratePrototypes(level, layout);
        level.CopyUsing(layout, spaces);
        level.Random = rand;
        return level;
    }

    public void LoadTestLevel(LevelLayout layout, System.Random rand)
    {
        layout.Random = rand;
        Level level = GenerateFromLayout(layout, rand);
        DeployLevel(level);
    }
}

