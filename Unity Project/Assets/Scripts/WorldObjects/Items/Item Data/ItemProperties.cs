using System;
using XML;

public class ItemProperties : IXmlParsable
{
    //Properties
    public BUC BUC { get; set; }
    public int Size { get; set; }
    private string damage = "";
    public Dice Damage
    {
        get { return Probability.getDice(damage); }
        set { this.damage = value.diceName; }
    }
    public EquipType EquipType { get; set; }
    public int NumberOfSlots { get; set; }

    public void ParseXML(XMLNode x)
    {
        damage = x.SelectString("damage");
        EquipType = x.SelectEnum<EquipType>("equiptype");
        NumberOfSlots = x.SelectInt("slots", 1);
    }

    public int GetHash()
    {
        int hash = 11;
        hash += BUC.GetHashCode();
        hash += Size.GetHashCode() * 3;
        hash += damage.GetHashCode() * 7;
        hash += EquipType.GetHashCode() * 11;
        hash += NumberOfSlots.GetHashCode() * 13;
        return hash;
    }
}