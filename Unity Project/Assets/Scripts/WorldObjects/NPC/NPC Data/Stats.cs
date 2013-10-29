using System;
using System.Collections.Generic;
using XML;

public class Stats : IXmlParsable
{
    public int MaxHealth { get; set; }
    public int CurrentHealth { get; set; }
    public int MaxPower { get; set; }
    public int CurrentPower { get; set; }
    public float Hunger { get; set; }
    public float Encumbrance { get; set; } //needs calc'd on NPC
    public float MaxEncumbrance { get; set; }
    public int Level { get; set; }
    public float CurrentXP { get; set; }
    public float XPToNextLevel { get; set; } //needs calc'd on NPC
    public float hungerRate { get; set; }
    public HungerLevel HungerLevel { get; set; }
    public EncumbranceLevel EncumbranceLevel { get; set; }

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

    private void initialize()
    {
        this.CurrentHealth = this.MaxHealth;
        this.CurrentPower = this.MaxPower;
        this.Encumbrance = 0;
        this.CurrentXP = 0;
        this.Hunger = 500f;
        this.hungerRate = .2f;
    }
}
