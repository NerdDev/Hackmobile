using UnityEngine;
using System;
using System.Collections;
using XML;
using System.Collections.Generic;

public class Item : Affectable, PassesTurns, IXmlParsable
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
		BigBoss.WorldObject.AddItemToMasterList(this);//registering existence with singleton
        BigBoss.Time.RegisterToUpdateList(this);
	}
	
	public virtual void DestroyThisItem()
	{
		BigBoss.WorldObject.RemoveItemFromMasterList(this);//removing existence with singleton
        BigBoss.Time.RemoveFromUpdateList(this);
        Destroy(this.gameObject);
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
    private string icon;
    public string Icon
    {
        get { return icon; }
        set { this.icon = value; }
    }
    public ItemProperties props = new ItemProperties();

    //flags
    public Flags<ItemFlags> itemFlags = new Flags<ItemFlags>();

    //separate classes
    public ItemStats stats = new ItemStats();

    //effects
    protected Spell onEaten = new Spell();
    protected Spell onEquip = new Spell();
    protected Spell onUse = new Spell();

    #endregion

    public Item()
    {
    }

    #region Usage:

    public void onEatenEvent(NPC n)
    {
        if (onEaten != null)
        {
            Debug.Log("Activating OnEatenEvent()");
            onEaten.Activate(this, n);
        }
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
            onUse.Activate(n);
        }

        //if usage needs restricted, change that here
    }

    public bool isUnEquippable()
    {
        if (props.BUC != BUC.CURSED)
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
            onEquip.Activate(n);
        }
    }

    public void onUnEquipEvent(NPC n)
    {
        if (onEquip != null)
        {
            onEquip.Activate(n);
        }
    }

    public int getDamage()
    {
        return this.props.Damage.getValue();
    }

    #endregion

    #region Data Management for Item Instances
    public override void ParseXML(XMLNode x)
    {
        base.ParseXML(x);
        type = x.Key;
        props = x.Select<ItemProperties>("properties");
        onEquip = x.Select<Spell>("OnEquipEffect");
        onUse = x.Select<Spell>("OnUseEffect");
        onEaten = x.Select<Spell>("OnEatenEffect");
        stats = x.Select<ItemStats>("stats");
        Icon = x.SelectString("icon");
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
