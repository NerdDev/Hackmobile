using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {
	
	public Theme theme;
    public LevelBuilder builder;
	
    void Start()
    {
        LevelGenerator gen = new LevelGenerator();
        LevelLayout layout = gen.generateLayout(0, 665911697, 1733302797);
		builder.Build(layout, theme);

        JustinTest();
        JoseTest();
    }

    void JustinTest()
    {
        Room room = new Room(0);
        room.generate();
        PillarMod pillar = new PillarMod();
        pillar.Modify(room, new System.Random());
        DebugManager.w(DebugManager.Logs.LevelGenMain, "Testing Room Modifier: " + pillar);
        room.toLog(DebugManager.Logs.LevelGenMain);
    }

    void JoseTest()
    {
        Room room = new Room(0);
        room.generate();
        HiddenRoomMod hidden = new HiddenRoomMod();
        hidden.Modify(room, new System.Random());
        DebugManager.w(DebugManager.Logs.LevelGenMain, "Testing Room Modifier: " + hidden);
        room.toLog(DebugManager.Logs.LevelGenMain);
    }
}
