using System;

public static class Probability
{
    static Random rand = new Random();

    public Probability()
    {
    }

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
}