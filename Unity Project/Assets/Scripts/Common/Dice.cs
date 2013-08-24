using System;
using System.Collections.Generic;

public class Dice
{
    public int faces;
    public int numDice;
    public string diceName;
    private Random rand = new Random();

    public Dice(int numFaces, int dice)
    {
        this.faces = numFaces;
        this.numDice = dice;
        diceName = numDice + "d" + faces;
    }

    public Dice(string s)
    {
        if (s.Contains("d")) 
        {
            string[] split = s.Split('d');
            if (split.Length == 2)
            {
                if (split[0] != "")
                {
                    int.TryParse(split[0], out numDice);
                    int.TryParse(split[1], out faces);
                }
                else
                {
                    int.TryParse(split[1], out faces);
                    numDice = 1;
                }
                diceName = s;
            }
            else
            {
                numDice = 1;
                faces = 1;
                diceName = "1d1";
            }
        }
        else
        {
            numDice = 1;
            faces = 1;
            diceName = "1d1";
        }
    }

    public int getFaces()
    {
        return faces;
    }

    public int getNumDice()
    {
        return numDice;
    }

    public int getValue()
    {
        int ret = 0;
        for (int i = 0; i < numDice; i++)
        {
            ret += rand.Next(1, (faces + 1));
        }
        return ret;
    }
}
