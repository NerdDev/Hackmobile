using System;
using XML;

public class ItemProperties : IXmlParsable
{
    //Properties
    private BUC buc;
    public BUC BUC
    {
        get { return buc; }
        set { this.buc = value; }
    }
    private int size;
    public int Size
    {
        get
        {
            return size;
        }
        set
        {
            this.size = value;
        }
    }
    private float weight;
    public float Weight
    {
        get
        {
            if (!Material.Name.Equals("") && Size != 0) {
                return (Size * Material.Density) / 1000;
            } else {
                return weight;
            }
        }
    }

    //These map to existing values upon a dictionary stored in ItemMaster
    private string damage;
    public Dice Damage
    {
        get { return Probability.getDice(damage); }
        set { this.damage = value.diceName; }
    }
    private string mat = "";
    public MaterialType Material
    {
        get { return BigBoss.WorldObject.getMaterial(mat); }
        set
        {
            if (value != null) { this.mat = value.Name; }
            else { this.mat = ""; }
        }
    }

    private EquipTypes equipType;
    public EquipTypes EquipType
    {
        get { return equipType; }
        set { this.equipType = value; }
    }

    public void ParseXML(XMLNode x)
    {
        damage = x.SelectString("damage");
        mat = x.SelectString("material");
        if (this.mat.Equals("")) { this.weight = x.SelectInt("weight"); }
        EquipType = x.SelectEnum<EquipTypes>("equiptype");
    }
}
