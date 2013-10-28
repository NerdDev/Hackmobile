using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class LevelManager : MonoBehaviour, IManager {
	
    // Internal
	public Theme Theme;
    public LevelBuilder Builder;
    public int Seed = -1;
    private const int MaxLevels = 100;
    private static Level[] Levels;

    // Public Access
    public Level Level { get; private set; }
    public int CurLevelDepth { get; private set; }

    public int CurLevel;

    public void Initialize()
    {
        Builder.Theme = Theme;
        RoomModifier.RegisterModifiers();
        Levels = new Level[MaxLevels];
    }
    
    void Start()
    {
    }

    public void SetCurLevel(int num)
    {
        Destroy(Level);
        Level = GetLevel(num);
        CurLevelDepth = num;
        Deploy(Level);
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

    public Level GetLevel(int num)
    {
        if (Levels[num] == null)
        {
            GenerateLevel(num);
        }
        return Levels[num];
    }

    void Destroy(Level level)
    {
        if (level == null) return;
        foreach (GridSpace space in level.Iterate())
        {
            space.SetActive(false);
        }
    }

    void Deploy(Level level)
    {
        foreach(Value2D<GridSpace> space in level)
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

    void GenerateLevel(int num)
    {
        Theme theme = GetTheme();
        LevelGenerator gen = new LevelGenerator(theme, num);
        Levels[num] = new Level(gen.GenerateLayout(Seed), theme);
    }

    Theme GetTheme()
    {
        return Theme;
    }
}
