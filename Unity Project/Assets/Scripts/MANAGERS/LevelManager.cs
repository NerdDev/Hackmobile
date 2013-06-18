using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {

    void Start()
    {
        DebugManager.levelGenDFSsteps = true;
        LevelGenerator gen = new LevelGenerator();
		float startTime = Time.realtimeSinceStartup;
        gen.generateLayout(1);
		Debug.Log ("Generate Level took: " + (Time.realtimeSinceStartup - startTime));
        gen.generateLayout(0, 665911697, 1733302797);
    }

}
