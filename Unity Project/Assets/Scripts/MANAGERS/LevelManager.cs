using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {
	
	public Theme Theme;
    public LevelBuilder Builder;
    public static GameObject[,] Blocks { get; private set; }
    public static GridArray Array { get; private set; }
    private static LevelGenerator gen;

    void Start()
    {
        RoomModifier.RegisterModifiers();
        gen = new LevelGenerator();
        LevelLayout layout;
        layout = gen.GenerateLayout(0);
//        layout = GenerateLevels(30);
        Array = layout.GetArray();
		Blocks = Builder.Build(Array, Theme);

        JustinTest();
        JoseTest();
    }

    LevelLayout GenerateLevels(int num)
    {
        LevelLayout last = null;
        for (int i = 0; i < num; i++)
        {
            last = gen.GenerateLayout(i);
        }
        return last;
    }

    void TestModifier(RoomModifier mod, int seed)
    {
        System.Random rand = new System.Random(seed);
        Room room = new Room();
        SquareRoom square = new SquareRoom();
        square.Modify(room, rand);
        mod.Modify(room, rand);
        DebugManager.w(DebugManager.Logs.LevelGenMain, "Testing Room Modifier: " + mod + " with seed: " + seed);
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
