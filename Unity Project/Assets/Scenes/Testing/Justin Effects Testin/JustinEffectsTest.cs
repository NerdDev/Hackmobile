using UnityEngine;
using System.Collections;

public class JustinEffectsTest : MonoBehaviour {

	// Use this for initialization
    void Start()
    {
        BigBoss.Levels.SetCurLevel(0);
        BigBoss.DungeonMaster.PopulateLevel(BigBoss.Levels.Level);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
