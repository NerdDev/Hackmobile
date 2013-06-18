using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {

    void Start()
    {
        LevelGenerator gen = new LevelGenerator();
        gen.generateLayout(1);
        gen.generateLayout(0, 73152134, 1017547111);
    }

}
