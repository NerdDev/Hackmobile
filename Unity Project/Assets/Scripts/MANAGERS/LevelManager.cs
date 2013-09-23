using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour, IManager {
	
    // Internal
	public Theme Theme;
    public LevelBuilder Builder;
    public int Seed = -1;
    private const int MaxLevels = 100;
    private static Level[] Levels;

    // Public Access
    public static Level Level { get; private set; }
    public static int CurLevelDepth { get; private set; }

    public void Initialize()
    {
        Builder.Theme = Theme;
        RoomModifier.RegisterModifiers();
        Levels = new Level[MaxLevels];
    }
    
    void Start()
    {
        SetCurLevel(0);

        if (BigBoss.DungeonMaster != null)
            BigBoss.DungeonMaster.PopulateLevel(Level);
    }

    void SetCurLevel(int num)
    {
        Level = GetLevel(num);
        CurLevelDepth = num;
        Deploy(Level);
        // Need to switch out game blocks when level switching is implemented
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
        foreach (GridSpace space in Level.Iterate())
        {
            // Delete block
        }
    }

    void Deploy(Level level)
    {
        foreach(Value2D<GridSpace> space in level)
        {
            if (space != null)
            {
                Builder.Build(space);
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
