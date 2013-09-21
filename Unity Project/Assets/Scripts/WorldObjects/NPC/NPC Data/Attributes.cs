using System;
using XML;

public class AttributesData : FieldContainerClass
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

    public int get(Attributes a) 
    {
        switch (a)
        {
            case Attributes.Charisma:
                return Charisma;
            case Attributes.Constitution:
                return Constitution;
            case Attributes.Dexterity:
                return Dexterity;
            case Attributes.Intelligence:
                return Intelligence;
            case Attributes.Strength:
                return Strength;
            case Attributes.Wisdom:
                return Wisdom;
        }
        return 0;
    }

    public bool set(Attributes a, int val)
    {
        switch (a)
        {
            case Attributes.Charisma:
                Charisma = val;
                return true;
            case Attributes.Constitution:
                Constitution = val;
                return true;
            case Attributes.Dexterity:
                Dexterity = val;
                return true;
            case Attributes.Intelligence:
                Intelligence = val;
                return true;
            case Attributes.Strength:
                Strength = val;
                return true;
            case Attributes.Wisdom:
                Wisdom = val;
                return true;
        }
        return false;
    }

    public override void SetParams()
    {
        base.SetParams();
        this.Strength = map.Add<Integer>("strength");
        this.Charisma = map.Add<Integer>("charisma");
        this.Intelligence = map.Add<Integer>("intelligence");
        this.Wisdom = map.Add<Integer>("wisdom");
        this.Dexterity = map.Add<Integer>("dexterity");
        this.Constitution = map.Add<Integer>("constitution");
        this.Difficulty = map.Add<Integer>("difficulty");
        this.Size = map.Add<EnumField<Size>>("size");
    }
}

public enum Attributes
{
    Strength,
    Dexterity,
    Constitution,
    Intelligence,
    Wisdom,
    Charisma
}