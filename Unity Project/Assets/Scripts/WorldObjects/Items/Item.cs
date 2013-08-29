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
    void Start()
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
    protected EffectEvent onEaten = new EffectEvent();
    protected EffectEvent onEquip = new EffectEvent();
    protected EffectEvent onUse = new EffectEvent();

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
            onEaten.activate(n);
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
            onUse.activate(n);
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
            onEquip.activate(n);
        }
    }

    public void onUnEquipEvent(NPC n)
    {
        if (onEquip != null)
        {
            onEquip.activate(n);
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
        this.props = baseItem.props.Copy();
        this.icon = baseItem.icon;
        this.itemFlags = baseItem.itemFlags.Copy();
        this.onEaten = baseItem.onEaten.Copy();
        this.onEquip = baseItem.onEquip.Copy();
        this.onUse = baseItem.onUse.Copy();
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
        onEquip.parseXML(XMLNifty.select(x, "OnEquipEffect"));
        onUse.parseXML(XMLNifty.select(x, "OnUseEvent"));
        onEaten.parseXML(XMLNifty.select(x, "OnEatenEffect"));
        stats.parseXML(x);
        icon = XMLNifty.SelectString(x, "icon");
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