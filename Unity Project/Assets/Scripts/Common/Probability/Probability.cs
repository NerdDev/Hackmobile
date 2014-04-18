using System;
using UnityEngine;
using System.Collections.Generic;

public static class Probability
{
    private static System.Random _rand = new System.Random();
    public static System.Random Rand { get { return _rand; } }
    private static System.Random _spawnRand = new System.Random();
    public static System.Random SpawnRand { get { return _spawnRand; } }
}
