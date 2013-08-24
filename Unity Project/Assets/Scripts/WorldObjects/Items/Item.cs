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
	void Start () 
	{
		RegisterItemToSingleton();
	}
	
	public virtual void RegisterItemToSingleton() //if we decide to make Item.cs structural only, then switch these to abstract
	{
		BigBoss.WorldObjectManager.AddItemToMasterList(this);//registering existence with singleton
        BigBoss.TimeKeeper.RegisterToUpdateList(this);
	}
	
	public virtual void DestroyThisItem()
	{
		BigBoss.WorldObjectManager.RemoveItemFromMasterList(this);//removing existence with singleton
        BigBoss.TimeKeeper.RemoveFromUpdateList(this);
		Destroy (this.gameObject);
	}
    #endregion

    #region Properties of Items

    //Properties
    private string type;
    public string Type
    {
        get { return type; }
        set { this.type = value; }
    }
    public ItemProperties props = new ItemProperties();

    //flags
    public Flags<ItemFlags> itemFlags = new Flags<ItemFlags>();

    //separate classes
    public ItemStats stats = new ItemStats();

    //effects
    protected ItemEffect onEaten = new ItemEffect();
    protected ItemEffect onEquip = new ItemEffect();
    protected ItemEffect onUse = new ItemEffect();

    #endregion

    public Item()
    {
    }

    #region Usage:

    public void onEatenEvent(NPC n)
    {
        if (onEaten != null)
        {
            n.applyEffect(onEaten.prop, onEaten.strength, false, onEaten.turns);
        }
        n.AdjustHunger(this.stats.Nutrition);
    }

    public bool isUsable()
    {
        //do any code to determine usability here
        //like if it's restricted on a turn basis, all that
        return true;
    }

    public void onUseEvent(NPC n)
    {
        if (onUse != null)
        {
            n.applyEffect(onUse.prop, onUse.strength, false, onUse.turns);
        }

        //if usage needs restricted, change that here
    }

    public bool isUnEquippable()
    {
        if (props.BUC == global::BUC.BLESSED || props.BUC == global::BUC.UNCURSED)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void onEquipEvent(NPC n)
    {
        if (onEquip != null)
        {
            n.applyEffect(onEquip.prop, 1, true);
        }
    }

    public void onUnEquipEvent(NPC n)
    {
        if (onEquip != null)
        {
            n.applyEffect(onEquip.prop, -1, true);
        }
    }

    public int getDamage()
    {
        return this.props.Damage.getValue();
    }

    #endregion

    #region Data Management for Item Instances

    public void setData(string itemName) 
    {
        this.setData(BigBoss.WorldObjectManager.getItem(itemName));
    }

    //use this to do a conversion of a base item to instanced item
    public void setData(Item baseItem)
    {
        base.setData(baseItem);
        //classes
        this.stats = baseItem.stats.Copy();
        //properties
        this.Type = baseItem.Type;
        this.props.BUC = baseItem.props.BUC;
        this.props.Damage = baseItem.props.Damage;
        this.props.Material = baseItem.props.Material;
        this.props.EquipType = baseItem.props.EquipType;
        this.props.Size = baseItem.props.Size;
    }

    public override void setNull()
    {
        //Initialize to null stats essentially. Needed/Not needed? Dunno.
        parseXML(new XMLNode());
        IsActive = false;
    }

    public override void parseXML(XMLNode x)
    {
        base.parseXML(x);
        props.parseXML(x);
        this.onEquip.parseXML(XMLNifty.select(x, "OnEquipEffect"));
        this.onUse.parseXML(XMLNifty.select(x, "OnUseEvent"));
        this.onEaten.parseXML(XMLNifty.select(x, "OnEatenEffect"));
        stats.parseXML(x);
    }
    #endregion

    #region Turn Management

    //If there's anything that needs updated... let it go here.
    private int itemPoints = 0;
    private int baseItemPoints = 60;

    public override void UpdateTurn()
    {
        //throw new NotImplementedException();
    }

    public override int CurrentPoints
    {
        get
        {
            return this.itemPoints;
        }
        set
        {
            this.itemPoints = value;
        }
    }

    public override int BasePoints
    {
        get
        {
            return this.baseItemPoints;
        }
        set
        {
            this.baseItemPoints = value;
        }
    }
    public override bool IsActive
    {
        get
        {
            return this.isActive;
        }
        set
        {
            this.isActive = value;
        }
    }
    #endregion
}