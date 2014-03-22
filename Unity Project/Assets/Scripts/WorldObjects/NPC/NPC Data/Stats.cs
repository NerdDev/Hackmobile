using System;
using System.Collections.Generic;
using XML;

public class Stats : IXmlParsable
{
    public int MaxHealth = 50;
    public int CurrentHealth = 100;
    public int MaxPower;
    public int CurrentPower;
    public float Hunger;
    public float MaxEncumbrance;
    public ushort Level;
    public float CurrentXP;
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
        Level = x.SelectUShort("level");
        initialize();
    }

    internal void initialize()
    {
        this.CurrentHealth = this.MaxHealth;
        this.CurrentPower = this.MaxPower;
        this.CurrentXP = 0;
        this.Hunger = 500f;
        this.hungerRate = .2f;
    }
}
