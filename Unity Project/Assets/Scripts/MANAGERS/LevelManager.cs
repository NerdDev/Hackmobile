using System;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour, IManager {
	
    // Internal
	public Theme Theme;
    public LevelBuilder Builder;
    public int Seed = -1;
    private const int MaxLevels = 100;
    private static LevelGenerator _gen;
    private static Level[] Levels;

    // Public Access
    public static Level Level { get; private set; }
    public static int CurLevelDepth { get; private set; }

    public void Initialize()
    {
        Builder.Theme = Theme;
        RoomModifier.RegisterModifiers();
        Levels = new Level[MaxLevels];
        _gen = new LevelGenerator();
    }
    
    void Start()
    {
        SetCurLevel(0);

        if (BigBoss.DungeonMaster != null)
            BigBoss.DungeonMaster.PopulateLevel(Level);

        JustinTest();
        JoseTest();
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
        IEnumerator<GridSpace> grids = Level.GetBasicEnumerator();
        while (grids.MoveNext())
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
        Levels[num] = new Level(_gen.GenerateLayout(num, Seed));
    }

    void TestModifier(RoomModifier mod, int seed)
    {
        Room room = new Room();
        SquareRoom square = new SquareRoom();
        square.Modify(room, Probability.LevelRand);
        mod.Modify(room, Probability.LevelRand);
        BigBoss.Debug.w(DebugManager.Logs.LevelGenMain, "Testing Room Modifier: " + mod + " with seed: " + seed);
        room.ToLog(DebugManager.Logs.LevelGenMain);
    }

    void JustinTest()
    {
        System.Random rand = new System.Random();
        TestModifier(new PillarMod(), rand.Next()); 
    }

    void JoseTest()
    {
        System.Random rand = new System.Random();
        TestModifier(new HiddenRoomMod(), rand.Next());
    }
}
