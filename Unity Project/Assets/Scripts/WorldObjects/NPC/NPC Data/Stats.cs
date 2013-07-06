using System;
using System.Collections.Generic;
using XML;

public class Stats
{
    public int MaxHealth { get; set; }
    public int CurrentHealth { get; set; }
    public int MaxPower { get; set; }
    public int CurrentPower { get; set; }
    public int Hunger { get; set; }
    public float Encumbrance { get; set; }
    public float MaxEncumbrance { get; set; }
    public int Level { get; set; }
    public float CurrentXP { get; set; }
    public float XPToNextLevel { get; set; }
    public HungerLevel HungerLevel { get; set; }
    public EncumbranceLevel EncumbranceLevel { get; set; }

    public void setNull()
    {

    }

    public void parseXML(XMLNode x)
    {

    }
}