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

    public int CurLevel;

    public void Initialize()
    {
        RoomModifier.RegisterModifiers();
        Builder.Theme = Theme;
        _levels = new Level[_maxLevels];
        ArrayExt.Converters.Add(typeof(GridType), (b) => { return GridTypeEnum.Convert((GridType)b); });
        if (Seed == -1)
            Seed = Probability.Rand.Next();
        System.Random rand = new System.Random(Seed);
        for (int i = 0; i < _maxLevels; i++)
            _levelSeeds[i] = rand.Next();
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
        CurLevelDepth = depth;
        Deploy(level);
        Level = level;
    }

    public void SetCurLevel(bool up)
    {
        if (up && CurLevelDepth > 0)
        {
            SetCurLevel(CurLevelDepth - 1);
        }
        else if (up)
        {
            //do nothing, these stairs do not go anywhere
            CurLevel = CurLevelDepth;
            return;
        }
        else
        {
            SetCurLevel(CurLevelDepth + 1);
        }
        CurLevel = CurLevelDepth;
        BigBoss.DungeonMaster.PopulateLevel(Level, up);
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
        foreach (GridSpace space in level.Iterate())
        {
            foreach (WorldObject wo in space.GetBlockingObjects())
            {
                if (wo.IsNotAFreaking<Player>())
                    wo.Destroy();
            }
            foreach (WorldObject wo in space.GetFreeObjects())
            {
                if (wo.IsNotAFreaking<Player>())
                    wo.Destroy();
            }
            space.SetActive(false);
        }
    }

    void Deploy(Level level)
    {
        foreach (Value2D<GridSpace> space in level)
        {
            if (space != null)
            {
                if (space.val.Block == null)
                    Builder.Build(space);
                else
                    space.val.SetActive(true);
            }
        }
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
        if (depth != 0)
            if (_levels[depth - 1] != null)
                gen.UpStairs = _levels[depth - 1].DownStairs;
        else
            gen.UpStairs.Set = true;
        if (_levels[depth + 1] != null)
            gen.DownStairs = _levels[depth + 1].UpStairs;
        gen.UpStairs.SelectedUp = true;
        gen.DownStairs.SelectedUp = false;
        _levels[depth] = new Level(gen.Generate(), gen.Theme);
    }

    Theme GetTheme()
    {
        return Theme;
    }
}
