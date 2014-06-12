using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public struct Damage : IXmlParsable
{
    public int min;
    public int max;
    public int median;

    public Damage(int min, int max, int median)
    {
        this.min = min;
        this.max = max;
        this.median = median;
    }

    public Damage(int min, int max)
    {
        this.min = min;
        this.max = max;
        median = 0;
    }

    public Damage(int median)
    {
        this.min = 0;
        this.max = 0;
        this.median = median;
    }

    public void SetMin(int min)
    {
        this.min = min;
    }

    public void SetMax(int max)
    {
        this.max = max;
    }

    public int GetDamage()
    {
        //damage calc?
        if (min == 0 && max == 1 && median != 0)
        {
            return median;
        }
        else
        {
            return (min + (int)(UnityEngine.Random.value * (max - min)));
        }
    }

    public void ParseXML(XMLNode x)
    {
        min = x.SelectInt("min");
        max = x.SelectInt("max", 1);
    }

    public override string ToString()
    {
        return min + " - " + max;
    }

    public int GetHash()
    {
        int hash = 3;
        hash += min * 5;
        hash += max * 19;
        return hash;
    }
}
