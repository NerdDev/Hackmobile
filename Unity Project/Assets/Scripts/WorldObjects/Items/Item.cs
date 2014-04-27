using System;

public class Item : Affectable, PassesTurns, IXmlParsable
{
    #region Properties of Items
    //Properties
    public override string Prefab { get { return base.Prefab; } set { base.Prefab = "Items/" + value; } }
    public string Type;
    public string Icon;
    public ItemProperties props = new ItemProperties();

    public bool OnGround { get; set; }

    //flags
    public Flags<ItemFlags> itemFlags = new Flags<ItemFlags>();

    //separate classes
    public ItemStats stats = new ItemStats();

    //effects
    public Spell onEaten = new Spell();
    public Spell onEquip = new Spell();
    public Spell onUse = new Spell();
    public Spell onHit = new Spell();

    //Count
    private int _count; //uneditable except from inside this class
    internal int Count { get { return _count; } }
    #endregion

    public Item()
    {
        _count = 1;
    }

    public Item(int count)
    {
        _count = count;
    }

    public void ModifyItem(Inventory container, Action<Item> mod)
    {
        container.ModifyItem(this, mod);
    }

    public int GetHash()
    {
        int hash = 17;
        hash += Name.GetHashCode() * 5;
        hash += Type.GetHashCode() * 3;
        hash += Icon.GetHashCode() * 5;
        hash += Prefab.GetHashCode() * 7;
        hash += props.GetHash() * 11;
        hash += itemFlags.GetHash() * 13;
        hash += stats.GetHash() * 17;
        hash += onEquip.GetHash() * 3;
        hash += onUse.GetHash() * 5;
        hash += onEaten.GetHash() * 7;
        hash += onHit.GetHash() * 9;
        return hash;
    }

    #region Usage:

    public void onEatenEvent(NPC n)
    {
        if (onEaten != null)
        {
            onEaten.Activate(n, this);
        }
        n.removeFromInventory(this);
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

    public void onHitEvent(NPC n)
    {
        if (onHit != null)
        {
            onHit.Activate(n);
        }
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

    private int getDamage()
    {
        return this.stats.damage.GetDamage();
    }

    public bool Damage(NPC n)
    {
        onHitEvent(n);
        return n.damage(getDamage());
    }

    #endregion

    #region Data Management for Item Instances
    public override void ParseXML(XMLNode x)
    {
        base.ParseXML(x);
        Type = x.Key;
        onEquip = x.Select<Spell>("OnEquipEffect");
        onUse = x.Select<Spell>("OnUseEffect");
        onEaten = x.Select<Spell>("OnEatenEffect");
        onHit = x.Select<Spell>("OnHitEffect");
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

    internal Item GetForTransfer()
    {
        Item newItem;
        if (Count > 0)
        {
            newItem = this.Copy();
            newItem._count = 1;
        }
        else  { newItem = null; }
        return newItem;
    }
    #endregion

    #region Turn Management

    public override void UpdateTurn()
    {
        //throw new NotImplementedException();
    }
    #endregion
}
