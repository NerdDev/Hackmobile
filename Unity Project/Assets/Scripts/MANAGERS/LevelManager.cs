using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {
	
	public Theme theme;
    public LevelBuilder builder;
    static GameObject[,] blocks_;
    static public GameObject[,] blocks { get { return blocks_; } private set { blocks_ = value; } }
    static GridArray array_;
    static public GridArray array { get { return array_; } private set { array_ = value; } }
	
    void Start()
    {
        RoomModifier.RegisterModifiers();
        LevelGenerator gen = new LevelGenerator();
        LevelLayout layout;
        layout = gen.GenerateLayout(0, 665911697, 1733302797);
        layout = gen.GenerateLayout(1);
        array = layout.GetArray();
		blocks = builder.Build(array, theme);

        JustinTest();
        JoseTest();
    }

    void TestModifier(RoomModifier mod, int seed)
    {
        System.Random rand = new System.Random(seed);
        Room room = new Room(0);
        SquareRoom square = new SquareRoom();
        square.Modify(room, rand);
        mod.Modify(room, rand);
        DebugManager.w(DebugManager.Logs.LevelGenMain, "Testing Room Modifier: " + mod + " with seed: " + seed);
        room.toLog(DebugManager.Logs.LevelGenMain);
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
