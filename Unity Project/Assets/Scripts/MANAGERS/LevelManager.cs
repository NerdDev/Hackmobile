using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {
	
	public Theme Theme;
    public LevelBuilder Builder;
    public static GameObject[,] Blocks { get; private set; }
    public static GridArray Array { get; private set; }

    void Start()
    {
        RoomModifier.RegisterModifiers();
        LevelGenerator gen = new LevelGenerator();
        LevelLayout layout;
        layout = gen.GenerateLayout(0, 1826027463, 870812270);
//        for (int i = 0; i < 30; i++)
//        {
//            DebugManager.w(DebugManager.Logs.LevelGenMain, "Generating " + i);
//            layout = gen.GenerateLayout(i);
//        }
        Array = layout.GetArray();
		Blocks = Builder.Build(Array, Theme);

        JustinTest();
        JoseTest();
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
