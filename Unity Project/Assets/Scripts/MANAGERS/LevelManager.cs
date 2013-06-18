using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {

    void Start()
    {
        DebugManager.levelGenDFSsteps = true;
        LevelGenerator gen = new LevelGenerator();
        gen.generateLayout(1);
        gen.generateLayout(0, 665911697, 1733302797);
    }

}
