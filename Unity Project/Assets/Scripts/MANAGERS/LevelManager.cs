using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {
	
	public Theme theme;
    public LevelBuilder builder;
	
    void Start()
    {
        RoomModifier.RegisterModifiers();
        LevelGenerator gen = new LevelGenerator();
        LevelLayout layout = gen.generateLayout(0, 665911697, 1733302797);
		builder.Build(layout, theme);

        JustinTest();
        JoseTest();
    }

    void TestModifier(RoomModifier mod, int seed)
    {
        System.Random rand = new System.Random(seed);
        Room room = new Room(0);
        room.generate();
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
