using System;
using System.Collections.Generic;

public static class Probability
{
    static Random rand = new Random();

    public static int getRandomInt() 
    {
        return rand.Next();
    }

    public static int getRandomInt(int maxVal)
    {
        return rand.Next(maxVal);
    }

    public static double getRandomDouble()
    {
        return rand.NextDouble();
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