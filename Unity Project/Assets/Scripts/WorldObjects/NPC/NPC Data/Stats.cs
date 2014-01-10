using System;
using System.Collections.Generic;
using XML;

public class Stats : IXmlParsable
{
    public int MaxHealth;
    public int CurrentHealth;
    public int MaxPower;
    public int CurrentPower;
    public float Hunger;
    public float Encumbrance; //needs calc'd on NPC
    public float MaxEncumbrance;
    public int Level;
    public float CurrentXP;
    public float XPToNextLevel; //needs calc'd on NPC
    public float hungerRate;
    public HungerLevel HungerLevel;
    public EncumbranceLevel EncumbranceLevel;

    public Stats()
    {
    }

    public void ParseXML(XMLNode x)
    {
        MaxHealth = x.SelectInt("maxhealth");
        MaxPower = x.SelectInt("maxpower");
        Level = x.SelectInt("level");
        initialize();
    }

    internal void initialize()
    {
        this.CurrentHealth = this.MaxHealth;
        this.CurrentPower = this.MaxPower;
        this.Encumbrance = 0;
        this.CurrentXP = 0;
        this.Hunger = 500f;
        this.hungerRate = .2f;
    }
}
