using UnityEngine;
using System;
using System.Collections;
using XML;

public class Item : WorldObject, PassesTurns
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
        BigBoss.TimeKeeper.RegisterToUpdateList(this);
	}
	
	public virtual void DestroyThisItem()
	{
		BigBoss.ItemMaster.RemoveItemFromMasterList(this);//removing existence with singleton
        BigBoss.TimeKeeper.RemoveFromUpdateList(this);
		Destroy (this.gameObject);
	}
    #endregion

    #region Generic Properties of Items

    //Properties
    private string type;
    public string Type
    {
        get { return type; }
        set { this.type = value; }
    }
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
    public double Weight
    {
        get
        {
            return (Size * Material.Density) / 1000;
        }
    }

    //These map to existing values upon a dictionary stored in ItemMaster
    private string damage;
    public Dice Damage
    {
        get { return BigBoss.DataManager.getDice(damage); }
        set { this.damage = value.diceName; }
    }
    private string mat;
    public MaterialType Material
    {
        get { return BigBoss.ItemMaster.getMaterial(mat); }
        set { this.mat = value.Name; }
    }

    private EquipTypes equipType;
    public EquipTypes EquipType
    {
        get { return equipType; }
        set { this.equipType = value; }
    }

    //flags
    public Flags<ItemFlags> itemFlags = new Flags<ItemFlags>();

    //separate classes
    public ItemStats stats = new ItemStats();

    #endregion

    #region Instanced Properties of Items
    //none atm
    #endregion

    #region Base Properties of Items
    //none atm
    #endregion

    public Item()
    {
    }

    #region Data Management for Item Instances
    public void setData(string itemName) 
    {
        this.setData(BigBoss.ItemMaster.getItem(itemName));
    }

    //use this to do a conversion of a base item to instanced item
    public void setData(Item baseItem)
    {
        base.setData(baseItem);
        //classes
        this.stats = baseItem.stats.Copy();
        //properties
        this.Type = baseItem.Type;
        this.BUC = baseItem.BUC;
        this.Damage = baseItem.Damage;
        this.Material = baseItem.Material;
        this.EquipType = baseItem.EquipType;
        this.Size = baseItem.Size;
    }

    public override void setNull()
    {
        //Initialize to null stats essentially. Needed/Not needed? Dunno.
        base.setNull();
        //properties
        type = "null";
        BUC = BUC.CURSED;
        damage = "d1";
        mat = "null";
        EquipType = EquipTypes.LAST;
        //classes
        stats.setNull();
    }

    #region XML Parsing
    public void parseXML(XMLNode x)
    {
        base.parseXML(x);
        this.Damage = BigBoss.DataManager.getDice(x.SelectString("damage"));
        this.mat = x.SelectString("material");
        stats.parseXML(x.select("stats"));
    }
    #endregion

    #endregion

    #region Turn Management

    //If there's anything that needs updated... let it go here.
    private int currentPoints;
    private int basePoints;

    public void UpdateTurn()
    {
        throw new NotImplementedException();
    }

    public int CurrentPoints
    {
        get
        {
            return currentPoints;
        }
        set
        {
            currentPoints = value;
        }
    }

    public int BasePoints
    {
        get
        {
            return basePoints;
        }
        set
        {
            basePoints = value;
        }
    }
    #endregion
}