using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public struct Damage : IXmlParsable
{
    public int min;
    public int max;

    public Damage(int min, int max)
    {
        this.min = min;
        this.max = max;
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
        return (min + (int) (UnityEngine.Random.value * (max - min)));
    }

    public void ParseXML(XMLNode x)
    {
        min = x.SelectInt("min");
        max = x.SelectInt("max", 1);
    }
}
