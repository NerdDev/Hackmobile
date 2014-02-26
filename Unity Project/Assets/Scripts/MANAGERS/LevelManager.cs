using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class LevelManager : MonoBehaviour, IManager
{
    // Internal
    private const int _maxLevels = 100;
    private static Level[] _levels;
    private int[] _levelSeeds = new int[_maxLevels];

    // Public Access
    public Level Level { get; private set; }
    public int CurLevelDepth { get; private set; }
    public bool Initialized { get; set; }
    public Theme Theme;
    public LevelBuilder Builder;
    public int Seed = -1;

    public void Initialize()
    {
        Builder.Theme = Theme;
        _levels = new Level[_maxLevels];
        ArrayExt.Converters[typeof(GridType)] = (b) => { return GridTypeEnum.Convert((GridType)b); };
        ArrayExt.Converters[typeof(GridSpace)] = (b) =>
        {
            GridSpace s = b as GridSpace;
            if (s == null) return GridTypeEnum.Convert(GridType.NULL);
            return GridTypeEnum.Convert(((GridSpace)b).Type);
        };
        if (Seed == -1)
            Seed = Probability.Rand.Next();
        System.Random rand = new System.Random(Seed);
        for (int i = 0; i < _maxLevels; i++)
            _levelSeeds[i] = rand.Next();
        LevelBuilder.Initialize();
        Theme.Init();
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

    protected bool InRange(int depth)
    {
        return depth >= 0 && depth < _maxLevels;
    }

    public void SetCurLevel(int depth)
    {
        if (!InRange(depth)) return;
        Destroy(Level);
        Level level;
        GetLevel(depth, out level);
        level.ToLog(Logs.Main, "Setting level to");
        CurLevelDepth = depth;
        Deploy(level);
        Level = level;
        Level.PlacePlayer(true);
        BigBoss.DungeonMaster.PopulateLevel(Level);
        Builder.Combine();
    }

    public void SetCurLevel(bool up)
    {
        if (up)
        {
            if (CurLevelDepth > 0)
            {
                SetCurLevel(CurLevelDepth - 1);
            }
        }
        else
        {
            SetCurLevel(CurLevelDepth + 1);
        }
    }

    public bool GetLevel(int depth, out Level level)
    {
        if (!InRange(depth))
        {
            level = null;
            return false;
        }
        if (_levels[depth] == null)
        {
            GenerateLevel(depth);
        }
        level = _levels[depth];
        return true;
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

    void GenerateLevels(int num)
    {
        for (int i = 0; i < num; i++)
        {
            GenerateLevel(num);
        }
    }

    void GenerateLevel(int depth)
    {
        LevelGenerator gen = new LevelGenerator();
        gen.Theme = GetTheme();
        gen.Depth = depth;
        gen.Rand = new System.Random(_levelSeeds[depth]);
        Level level = new Level(gen.Generate(), gen.Theme, gen.Rand);
        Builder.GeneratePrototypes(level);
        _levels[depth] = level;
    }

    Theme GetTheme()
    {
        return Theme;
    }
}
