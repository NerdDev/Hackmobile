using System;
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
    private int difficulty;
    public int Difficulty
    {
        get { return difficulty; }
        set { this.difficulty = value; }
    }
    private Size size;
    public Size Size
    {
        get { return size; }
        set { this.size = value; }
    }

    public void setNull() {
        this.Strength = 0;
        this.Charisma = 0;
        this.Intelligence = 0;
        this.Wisdom = 0;
        this.Dexterity = 0;
        this.Constitution = 0;
        this.Difficulty = 0;
        this.Size = Size.NONE;
    }

    public void parseXML(XMLNode x)
    {
        this.Strength = x.SelectInt("strength");
        this.Charisma = x.SelectInt("charisma");
        this.Intelligence = x.SelectInt("intelligence");
        this.Wisdom = x.SelectInt("wisdom");
        this.Dexterity = x.SelectInt("dexterity");
        this.Constitution = x.SelectInt("constitution");
        this.Difficulty = x.SelectInt("difficulty");
        this.Size = x.SelectEnum<Size>("size");
    }
}