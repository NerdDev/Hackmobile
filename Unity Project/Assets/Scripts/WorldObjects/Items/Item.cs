using UnityEngine;
using System.Collections;
using XML;

public class Item : WorldObject
{
    #region BIGBOSSMANAGEMENT
    //consider some abtract/virtual methods and variables here for cleanliness
	//ex:  register my existence and key information to the item master singleton so i'm not lost in the game world
	
	// Use this for initialization
	void Awake () 
	{
		RegisterItemToSingleton();
	}
	
	public virtual void RegisterItemToSingleton() //if we decide to make Item.cs structural only, then switch these to abstract
	{
		BigBoss.ItemMaster.AddItemToMasterList(this);//registering existence with singleton
	}
	
	public virtual void DestroyThisItem()
	{
		BigBoss.ItemMaster.RemoveItemFromMasterList(this);//removing existence with singleton
		Destroy (this.gameObject);
	}
    #endregion

    #region Properties of Items

    private string type;
    public string Type
    {
        get { return type; }
        set { this.type = value; }
    }
    private string name;
    public string Name
    {
        get { return name; }
        set { this.name = value; }
    }
    private BUC buc;
    public BUC BUC
    {
        get { return buc; }
        set { this.buc = value; }
    }
    private int cost;
    public int Cost
    {
        get { return cost; }
        set { this.cost = value; }
    }
    private string damage;
    public Dice Damage
    {
        get { return BigBoss.DataManager.getDice(damage); }
        set { this.damage = value.diceName; }
    }
    private string mat;
    public MaterialType Material
    {
        get { return BigBoss.DataManager.getMaterial(mat); }
        set { this.mat = value.Name; }
    }
    private int weight;
    public int Weight
    {
        get { return weight; }
        set { this.weight = value; }
    }

    #endregion

    public Item()
    {
    }

    #region Data Management for Item Instances
    public void setData(string itemName) 
    {
        this.setData(BigBoss.DataManager.getItem(itemName));
    }

    public void setData(Item baseItem)
    {
        base.setData(baseItem);
        this.Name = baseItem.Name;
        this.Type = baseItem.Type;
        this.Weight = baseItem.Weight;
        this.Cost = baseItem.Cost;
        this.BUC = baseItem.BUC;
        this.Damage = baseItem.Damage;
        this.Material = baseItem.Material;
        this.Model = baseItem.Model;
        this.ModelTexture = baseItem.ModelTexture;
    }

    public void setNull()
    {
        //Initialize to null stats essentially. Needed/Not needed? Dunno.
        Type = "";
        Name = "";
        BUC = BUC.CURSED;
        Cost = 0;
        damage = "d1";
        mat = "null";
        Weight = 0;
        Model = "";
        ModelTexture = "";
    }
    #endregion
}