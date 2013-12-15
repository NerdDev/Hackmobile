using System;
using UnityEngine;
using System.Collections.Generic;

public static class Probability
{
    private static System.Random _rand = new System.Random();
    public static System.Random Rand { get { return _rand; } }
    private static System.Random _spawnRand = new System.Random();
    public static System.Random SpawnRand { get { return _spawnRand; } }

    public static int getRandomInt() 
    {
        return Rand.Next();
    }

    public static int getRandomInt(int maxVal)
    {
        return Rand.Next(maxVal);
    }

    public static double getRandomDouble()
    {
        return Rand.NextDouble();
    }

    #region Dice Calcs
    static Dictionary<string, Dice> dice = new Dictionary<string, Dice>();

    static public Dice getDice(string d)
    {
        if (dice.ContainsKey(d))
        {
            return dice[d];
        }
        else
        {
            Dice die = new Dice(d);
            dice.Add(d, die);
            return die;
        }
    }

    static Dictionary<string, Dice> getDice()
    {
        return dice;
    }
    #endregion
}
