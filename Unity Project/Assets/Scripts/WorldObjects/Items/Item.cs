using UnityEngine;
using System;
using System.Collections;
using XML;
using System.Collections.Generic;

public class Item : Affectable, PassesTurns, IXmlParsable
{
    #region Properties of Items
    //Properties
    public override string Prefab { get { return Prefab; } set { base.Prefab = value + "Items/"; } }
    public string Type { get; set; }
    public string Icon { get; set; }
    public ItemProperties props = new ItemProperties();

    public bool OnGround { get; set; }

    //flags
    public Flags<ItemFlags> itemFlags = new Flags<ItemFlags>();

    //separate classes
    public ItemStats stats = new ItemStats();

    //effects
    protected Spell onEaten = new Spell();
    protected Spell onEquip = new Spell();
    protected Spell onUse = new Spell();

    //Count
    private uint _count; //uneditable except from inside this class
    internal uint Count { get { return _count; } }
    #endregion

    public Item()
    {
        _count = 1;
    }

    public Item(int count)
    {
        _count = (uint) count;
    }

    #region Usage:

    public void onEatenEvent(NPC n)
    {
        if (onEaten != null)
        {
            onEaten.Activate(this, n);
        }
        n.removeFromInventory(this);
        RemoveItem();
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
        Type = x.Key;
        props = x.Select<ItemProperties>("properties");
        onEquip = x.Select<Spell>("OnEquipEffect");
        onUse = x.Select<Spell>("OnUseEffect");
        onEaten = x.Select<Spell>("OnEatenEffect");
        stats = x.Select<ItemStats>("stats");
        Icon = x.SelectString("icon");
    }

    internal bool RemoveItem()
    {
        _count--;
        if (_count <= 0)
        {
            this.Destroy();
            return true;
        }
        return false;
    }

    internal void Add()
    {
        _count++;
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
