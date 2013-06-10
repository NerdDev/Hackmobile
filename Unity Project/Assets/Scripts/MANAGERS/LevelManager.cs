using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelManager : MonoBehaviour {

    Level[] levels = new Level[50];

    void Start()
    {
        getLevel(1);
    }

    Level getLevel(int index)
    {
        if (levels[index] == null)
        {
            LevelGenerator gen = new LevelGenerator();
            levels[index] = gen.generate(index);
        }
        return levels[index];
    }
}
