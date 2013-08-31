using System;
using XML;

public class ItemProperties
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
            if (Material != null && Size != 0) {
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
    private string mat;
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

    internal void parseXML(XML.XMLNode xnode)
    {
        XMLNode x = XMLNifty.select(xnode, "properties");
        damage = XMLNifty.SelectString(x, "damage");
        this.mat = XMLNifty.SelectString(x, "material");
        if (this.mat == null) { this.weight = XMLNifty.SelectInt(x, "weight"); }
        this.EquipType = XMLNifty.SelectEnum<EquipTypes>(x, "equiptype");
    }
}
