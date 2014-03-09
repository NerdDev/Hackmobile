using System;
using XML;

public class ItemProperties : IXmlParsable
{
    //Properties
    public BUC BUC { get; set; }
    public int Size { get; set; }
    private float weight;
    public float Weight
    {
        get
        {
            if (Material != null && Size != 0) {
                return (Size * Material.Density) / 1000;
            } else {
                return weight;
            }
        }
    }
    private string damage;
    public Dice Damage
    {
        get { return Probability.getDice(damage); }
        set { this.damage = value.diceName; }
    }
    private string mat = "";
    public MaterialType Material
    {
        get {
            return BigBoss.Objects.Materials.GetPrototype(mat); }
        set
        {
            if (value != null) { this.mat = value.Name; }
            else { this.mat = ""; }
        }
    }
    public EquipType EquipType { get; set; }
    public int NumberOfSlots { get; set; }

    public void ParseXML(XMLNode x)
    {
        damage = x.SelectString("damage");
        mat = x.SelectString("material");
        if (this.mat.Equals("")) { this.weight = x.SelectInt("weight"); }
        EquipType = x.SelectEnum<EquipType>("equiptype");
        NumberOfSlots = x.SelectInt("slots", 1);
    }
}
