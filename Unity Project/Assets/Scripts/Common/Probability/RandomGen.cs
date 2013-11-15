using UnityEngine;
using System.Collections;

public class RandomGen {

    protected System.Random rand;
    protected static UnityEngine.Random unityRand;

    public RandomGen()
    {
        rand = new System.Random();
    }

    public RandomGen(int seed)
    {
        SetSeed(seed);
    }

    public void SetSeed(int seed)
    {
        rand = new System.Random(seed);
        UnityEngine.Random.seed = seed;
    }

    public int Next()
    {
        return rand.Next();
    }

    public int Next(int max)
    {
        return rand.Next(max);
    }

    public int Next(int min, int max)
    {
        return rand.Next(min, max);
    }

    public void NextBytes(byte[] buffer)
    {
        rand.NextBytes(buffer);
    }

    public double NextDouble()
    {
        return rand.NextDouble();
    }

    public bool NextBool()
    {
        return rand.Next(2) == 1;
    }

    public bool Percent(Percent percent)
    {
        return rand.Percent(percent);
    }

    public static implicit operator System.Random(RandomGen src)
    {
        return src.rand;
    }
}
