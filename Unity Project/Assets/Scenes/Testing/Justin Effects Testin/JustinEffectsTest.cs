using UnityEngine;
using System.Collections;

public class JustinEffectsTest : MonoBehaviour {

	// Use this for initialization
    void Start()
    {
        BigBoss.Levels.SetCurLevel(0);
        NPC n = BigBoss.DungeonMaster.SpawnNPC(BigBoss.DungeonMaster.PickSpawnableLocation());
        AddHealth effect = new AddHealth() { Strength = 15 };
        n.ApplyEffect(effect);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
