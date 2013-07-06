using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using XML;
/**
 * Notes on usage:
 * 
 * Adding a property to an NPC requires adding it in several places;
 *  - The property itself
 *  - In the XML definition (if applicable)
 *  - In the parseXML (if read from XML)
 *  - In the setNull definition (to define a a null NPC)
 *  - In the setData definition (to define an instance of an NPC)
 *  
 * When adding a property, if the property is only useful or applicable to a base NPC
 *  then store it under the region of base NPC properties, and set it to a null initialization.
 *  Add the definition to convert it to whatever is needed for the instance of an NPC
 *  under the setData method.
 *  
 * If it is only applicable to an instance of an NPC, then do the opposite - add it to
 *  the instance NPC properties, and leave the initialization as null until it is defined
 *  in the setData method.
 *  
 * See equipment and equipped items for an example.
 */
public class NPC : WorldObject
{
    #region BIGBOSSMANAGEMENT
    // Use this for initialization
    void Awake()
    {
        RegisterNPCToSingleton();
    }

    public virtual void RegisterNPCToSingleton()
    {
        BigBoss.NPCManager.AddNPCToMasterList(this);
        BigBoss.TimeKeeper.RegisterToUpdateList(this);
    }

    public virtual void DestroyThisItem()
    {
        BigBoss.NPCManager.RemoveNPCFromMasterList(this);
        BigBoss.TimeKeeper.RemoveFromUpdateList(this);
        Destroy(this.gameObject);
    }
    #endregion

    //TODO:
    //  -- Refactor all public access to get/set functions? (annoying with unity, can't modify then).
    //  -- Refine the NPC's movement functions?

    #region Base NPC Properties
    //Local variables
    public List<NPCItem> baseInventory = null;
    #endregion

    #region NPC Instance Properties
    //Initialized all to null for the base.
    //Converted from base properties upon creation of instance.
    public Dictionary<Item, int> inventory = null;
    List<Item> equippedItems = null;
    Equipment equipment = null;
    #endregion

    #region Generic NPC Properties
    //Properties stored here.
    public GenericFlags<Properties> properties = new GenericFlags<Properties>();
    //When accessing, use the long value of NPC Properties.
    protected NPCEffect[] effects = new NPCEffect[(long)Properties.LAST];

    //All sets of flags
    GenericFlags<NPCFlags> flags = new GenericFlags<NPCFlags>();

    //Enums
    public Race race;
    public Role role;

    //Separate classes
    public AttributesData attributes = new AttributesData();
    public BodyParts bodyparts = new BodyParts();
    public Stats stats = new Stats();
    
    #endregion

    public NPC()
    {
    }

    #region Stats
    public virtual int AdjustHunger(int amount)
    {
        stats.Hunger += amount;
        getHungerLevel(stats.Hunger);
        return stats.Hunger;
    }

    public virtual void AddLevel()
    {
        stats.Level++;
        //do level up stuff here
        calcStats();
    }

    public virtual void AdjustHealth(int amount)
    {
        Debug.Log("Adjusting Health: " + amount);
        if (stats.CurrentHealth + amount > stats.MaxHealth)
        {
            stats.CurrentHealth = stats.MaxHealth;
        }
        else
        {
            stats.CurrentHealth = stats.CurrentHealth + amount;
        }
    }

    public virtual void AdjustMaxHealth(int amount)
    {
        stats.MaxHealth += amount;
    }

    public virtual void AdjustXP(float amount)
    {
        stats.CurrentXP += amount;
        if (stats.CurrentXP > stats.XPToNextLevel)
        {
            AddLevel();
        }
    }

    public virtual void AdjustAttribute(Attributes attr, int amount)
    {
        attributes.set(attr, attributes.get(attr) + amount);
        calcStats();
    }
    #endregion

    #region Stat Calculations
    //Use this for a re-calc on level up or any attribute changes.
    void calcStats()
    {
        stats.MaxEncumbrance = getMaxInventoryWeight();
        stats.XPToNextLevel = calcXPForNextLevel();
    }

    float calcXPForNextLevel()
    {
        //do calc here
        return (100 + ((Mathf.Pow(stats.Level, 3f) / 2)));
    }

    protected void getHungerLevel(int hunger)
    {
        HungerLevel prior = stats.HungerLevel;
        Color col = Color.white;

        if (hunger < 50)
        {
            stats.HungerLevel = HungerLevel.Faint;
            col = Color.red;
            BigBoss.Gooey.UpdateHungerText(col);
        }
        else if (hunger < 130)
        {
            stats.HungerLevel = HungerLevel.Starving;
            col = Color.yellow;
            BigBoss.Gooey.UpdateHungerText(col);
        }
        else if (hunger < 500)
        {
            stats.HungerLevel = HungerLevel.Hungry;
            col = Color.yellow;
            BigBoss.Gooey.UpdateHungerText(col);
        }
        else if (hunger < 800)
        {
            stats.HungerLevel = HungerLevel.Satiated;
            col = Color.blue;
            BigBoss.Gooey.UpdateHungerText(col);
        }
        else if (hunger < 1000)
        {
            stats.HungerLevel = HungerLevel.Stuffed;
            col = Color.yellow;
            BigBoss.Gooey.UpdateHungerText(col);
        }

        if (prior != stats.HungerLevel)
        {
            UpdateHungerLevel(col);
        }
    }

    protected void UpdateHungerLevel(Color guiCol)
    {
        BigBoss.Gooey.CreateTextPop(this.gameObject.transform.position + Vector3.up * .75f, stats.HungerLevel.ToString() + "!", guiCol);
    }
    #endregion

    #region Movement

    public bool moveNPC(Vector3 location)
    {
        this.gameObject.transform.localPosition = location;
        return true;
    }

    #endregion

    #region Effects
    /**
     * Does nothing if the NONE property is applied.
     */
    public virtual void applyEffect(Properties e, int priority, bool isItem)
    {
        Debug.Log("Applying Effect: " + e);
        if (e != Properties.NONE)
        {
            if (effects[(long)e] == null)
            {
                NPCEffect eff = new NPCEffect(e, this);
                if (this.properties[Properties.NONE])
                {
                    this.properties[Properties.NONE] = false;
                }

                effects[(long)e] = eff;
                eff.apply(priority, isItem);

            }
            else
            {
                effects[(long)e].apply(priority, isItem);
            }
        }
        if (this.IsNotAFreaking<PlayerManager>()) {
            BigBoss.Gooey.CreateTextPop(this.gameObject.transform.position, "Poisoned!", Color.green);
        }
    }

    /**
     * This entirely removes the effect from the NPC. If applied again later, it'll create a new one.
     */
    public void removeEffect(Properties e)
    {
        if (effects[(long)e] != null)
        {
            effects[(long)e] = null;
        }
    }
    #endregion

    #region Equipment Management

    public bool equipItem(Item i)
    {
        if (equipment.equipItem(i))
        {
            equippedItems.Add(i);
            return true;
        }
        return false;
    }

    public bool removeItem(Item i)
    {
        if (equipment.removeItem(i))
        {
            return true;
        }
        return false;
    }

    public List<Item> getEquippedItems()
    {
        return equippedItems;
    }

    #endregion

    #region Inventory
    public void addToInventory(Item item)
    {
        this.addToInventory(item, 1);
    }

    public void addToInventory(Item item, int count)
    {
        if (inventory.ContainsKey(item))
        {
            inventory[item] += count;
        }
        else
        {
            inventory.Add(item, count);
        }
        stats.Encumbrance += item.Weight * count;
    }

    public void removeFromInventory(Item item)
    {
        removeFromInventory(item, 1);

    }

    public void removeFromInventory(Item item, int count)
    {
        if (inventory.ContainsKey(item))
        {
            if (inventory[item] <= count)
            {
                inventory.Remove(item);
            }
            else
            {
                inventory[item] -= count;
            }
            stats.Encumbrance -= item.Weight * count;
        }
        else
        {
            //do nothing, the item isn't there
        }
    }

    public float getCurrentInventoryWeight()
    {
        float tempWeight = 0;
        foreach (KeyValuePair<Item, int> kvp in inventory)
        {
            tempWeight += kvp.Key.Weight * kvp.Value;
        }

        return tempWeight;
    }

    public float getMaxInventoryWeight()//Should only be calc'd when weight changes or attribute on player is affected
    {
        float invWeightMax;
        //Add formula here
        invWeightMax = (25 * (attributes.Strength + attributes.Constitution) + 50);
        return invWeightMax;
    }
    #endregion

    #region Get/Set of flags
    public bool get(NPCFlags fl)
    {
        return flags[fl];
    }

    public void set(NPCFlags fl, bool on)
    {
        flags[fl] = on;
    }
    #endregion

    #region NPC Management for instances
    //These are probably incomplete at this point, I haven't updated them consistently.
    public void setData(string npcName)
    {
        this.setData(BigBoss.NPCManager.getNPC(npcName));
    }

    public void setData(NPC npc)
    {
        //Anything performing the conversion from base NPC -> instance of NPC goes here.
        base.setData(npc);
        //classes
        this.stats = npc.stats.Copy();
        this.bodyparts = npc.bodyparts.Copy();
        //flags
        this.flags = npc.flags.Copy();
        //enums
        this.race = npc.race;
        this.role = npc.role;
        //lists
        this.effects = npc.effects.Copy();
        //inventory
        inventory = new Dictionary<Item, int>();
        //TODO: needs conversion from baseInventory into actual inventory
        equippedItems = new List<Item>();
        equipment = new Equipment(this.bodyparts);
        stats.MaxEncumbrance = getMaxInventoryWeight();
    }

    public override void setNull()
    {
        base.setNull();
        this.attributes.setNull();
        this.bodyparts.setNull();
        if (equipment != null)
        {
            this.equipment.setNull();
        }  
        this.flags.setNull();
        this.race = Race.NONE;
        this.role = Role.NONE;
        for (int i = 0; i < effects.Length; i++)
        {
            effects[i] = null;
        }
    }

    #region XML Parsing
    public override void parseXML(XMLNode x)
    {
        //name is handled in DataManager so we get the GameObject name
        base.parseXML(x);

        this.role = x.SelectEnum<Role>("role");
        this.race = x.SelectEnum<Race>("race");

        //property parsing
        List<XMLNode> xprops = x.select("properties").selectList("property");
        foreach (XMLNode xnode in xprops)
        {
            Properties np = Nifty.StringToEnum<Properties>(xnode.SelectString("name"));
            NPCEffect eff = new NPCEffect(np, this);
            eff.apply(xnode.SelectInt("val"), false);
            this.effects[(long) np] = new NPCEffect(np, this);
        }

        //flag parsing
        List<XMLNode> xflags = x.select("flags").selectList("flag");
        foreach (XMLNode xnode in xflags)
        {
            NPCFlags np = Nifty.StringToEnum<NPCFlags>(xnode.SelectString("name"));
            this.flags[np] = true;
        }

        //body part data
        this.bodyparts.parseXML(x.select("bodyparts"));

        //stats
        this.attributes.parseXML(x.select("stats"));

        //inventory
        baseInventory = new List<NPCItem>();
        foreach (XMLNode xnode in x.select("inventory").get())
        {
            NPCItem baseItem = new NPCItem();
            baseItem.parseXML(xnode);
            this.baseInventory.Add(baseItem);
        }
    }
    #endregion

    #endregion

    #region Turn Management

    private int npcPoints = 0;
    private int baseNPCPoints = 60;

    public override void UpdateTurn()
    {
        AdjustHunger(-1);
    }

    public override int CurrentPoints
    {
        get
        {
            return this.npcPoints;
        }
        set
        {
            this.npcPoints = value;
        }
    }

    public override int BasePoints
    {
        get
        {
            return this.baseNPCPoints;
        }
        set
        {
            this.baseNPCPoints = value;
        }
    }
    #endregion
}
