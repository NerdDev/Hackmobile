using System;
using System.Collections.Generic;
using XML;

public class NPCStats
{
    public int MaxHealth { get; set; }
    public int CurrentHealth { get; set; }
    public int MaxPower { get; set; }
    public int CurrentPower { get; set; }
    public int Hunger { get; set; }
    public int Level { get; set; }
    public int XP { get; set; }

    public void setNull()
    {

    }

    public void parseXML(XMLNode x)
    {

    }
}