using System;
using UnityEngine;
using System.Collections.Generic;

public static class Probability
{
    private static RandomGen _rand = new RandomGen();
    public static RandomGen Rand { get { return _rand; } set { _rand = value; } }
    private static RandomGen _levelRand = new RandomGen();
    public static RandomGen LevelRand { get { return _levelRand; } set { _levelRand = value; } }
    private static RandomGen _spawnRand = new RandomGen();
    public static RandomGen SpawnRand { get { return _levelRand; } set { _levelRand = value; } }

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
