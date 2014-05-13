using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class LevelManager : MonoBehaviour, IManager
{
    public Level Level { get; private set; }
    public bool Initialized { get; set; }
    public Theme TestTheme;
    private ProbabilityPool<IThemeOption> themeSets = ProbabilityPool<IThemeOption>.Create();
    public LevelBuilder Builder;
    public int Seed = -1;
    public bool UseInitialTheme;
    public Theme InitialTheme;

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
        if (InitialTheme == null || !UseInitialTheme)
        {
            InitialTheme = themeSets.Get(Probability.Rand).GetTheme(Probability.Rand);
        }
        LevelBuilder.Initialize();
        foreach (IInitializable init in this.FindAllDerivedObjects<IInitializable>())
        {
            init.Init();
        }
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
        Level = level;
        Level.PlacePlayer(true);
    }

    protected void DeployLevel(Level level)
    {
        Deploy(level);
        Level = level;
        Builder.Combine();
        AstarPath.active.Scan();
    }

    void Destroy(Level level)
    {
        if (level == null) return;
        foreach (GridSpace val in level.GetEnumerateValues())
        {
            val.WrapObjects(false);
            val.SetActive(false);
        }
    }

    void Deploy(Level level)
    {
        BigBoss.Debug.w(Logs.LevelGenMain, "Deploying " + level);
        foreach (GridSpace space in level.GetEnumerateValues())
        {
            space.SetActive(true);
            space.WrapObjects(true);
        }
        BigBoss.Debug.w(Logs.LevelGenMain, "Deployed " + level);
    }

    public Level GenerateLevel(int seed, Theme startingTheme, int depth)
    {
        LevelGenerator gen = new LevelGenerator();
        gen.Theme = startingTheme;
        gen.Depth = depth;
        gen.Rand = new System.Random(seed);
        LevelLayout layout = gen.Generate();
        Level level = GenerateFromLayout(layout, gen.Rand);
        return level;
    }

    Level GenerateFromLayout(LevelLayout layout, System.Random rand)
    {
        LevelGenerator.ConfirmEdges(layout);
        Level level = new Level();
        MultiMap<GridSpace> spaces = Builder.GeneratePrototypes(level, layout);
        level.UnderlyingContainer = spaces;
        level.LoadRoomMaps(layout);
        level.Random = rand;
        level.UpStartPoint = layout.UpStart;
        level.DownStartPoint = layout.DownStart;
        return level;
    }

    public void LoadTestLevel(LevelLayout layout, System.Random rand)
    {
        layout.Random = rand;
        Level level = GenerateFromLayout(layout, rand);
        DeployLevel(level);
    }
}
