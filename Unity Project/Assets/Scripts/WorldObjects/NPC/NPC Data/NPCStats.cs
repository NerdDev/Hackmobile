﻿using System;
using XML;

public class NPCStats
{
    //store stat data here
    private int strength;
    public int Strength
    {
        get { return strength; }
        set { this.strength = value; }
    }
    private int charisma;
    public int Charisma
    {
        get { return charisma; }
        set { this.charisma = value; }
    }
    private int intelligence;
    public int Intelligence
    {
        get { return intelligence; }
        set { this.intelligence = value; }
    }
    private int wisdom;
    public int Wisdom
    {
        get { return wisdom; }
        set { this.wisdom = value; }
    }
    private int dexterity;
    public int Dexterity
    {
        get { return dexterity; }
        set { this.dexterity = value; }
    }
    private int constitution;
    public int Constitution
    {
        get { return constitution; }
        set { this.constitution = value; }
    }

    public void setData(NPCStats ns) {
        this.Strength = ns.Strength;
        this.Charisma = ns.Charisma;
        this.Intelligence = ns.Intelligence;
        this.Wisdom = ns.Wisdom;
        this.Dexterity = ns.Dexterity;
        this.Constitution = ns.Constitution;
    }

    public void parseXML(XMLNode x)
    {
        this.Strength = Nifty.StringToInt(x.select("strength").getText());
        this.Charisma = Nifty.StringToInt(x.select("charisma").getText());
        this.Intelligence = Nifty.StringToInt(x.select("intelligence").getText());
        this.Wisdom = Nifty.StringToInt(x.select("wisdom").getText());
        this.Dexterity = Nifty.StringToInt(x.select("dexterity").getText());
        this.Constitution = Nifty.StringToInt(x.select("constitution").getText());
    }
}