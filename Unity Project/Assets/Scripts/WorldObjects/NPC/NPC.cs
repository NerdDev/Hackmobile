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
public class NPC : WorldObject, PassesTurns
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

    #region Base NPC Properties
    //Local variables
    public List<NPCItem> baseInventory = null;
    #endregion

    #region NPC Instance Properties
    //Initialized all to null for the base.
    //Converted from base properties upon creation of instance.
    public Dictionary<Item, int> inventory = null;
    List<Item> equippedItems = null;
    NPCEquipment equipment = null;
    #endregion

    #region Generic NPC Properties
    //Properties stored here.
    public GenericFlags<NPCProperties> properties = new GenericFlags<NPCProperties>();
    //When accessing, use the long value of NPC Properties.
    protected NPCEffect[] effects = new NPCEffect[(long)NPCProperties.LAST];

    //All sets of flags
    GenericFlags<NPCFlags> flags = new GenericFlags<NPCFlags>();

    //Enums
    public NPCRace race;
    public NPCRole role;

    //Separate classes
    public AttributesData attributes = new AttributesData();
    public NPCBodyParts bodyparts = new NPCBodyParts();
    public NPCStats stats = new NPCStats();
    
    #endregion

    #region Misc Properties for storage
    int encumbranceLevel = 0;
    int XPForNextLevel = 0;
    #endregion

    public NPC()
    {
    }

    #region Stats
    public virtual int AdjustHunger(int amount)
    {
        stats.Hunger += amount;
        return stats.Hunger;
    }

    public virtual void AddLevel()
    {
        stats.Level++;
    }

    public virtual void AdjustHealth(int amount)
    {
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

    public virtual void AdjustXP(int amount)
    {
        stats.XP += amount;
        BigBoss.Gooey.UpdateXPBar();
    }

    public virtual void AdjustAttribute(Attributes attr, int amount)
    {
        switch (attr)
        {
            case Attributes.Charisma:
                {
                    attributes.Charisma += amount;
                    break;
                }
            case Attributes.Constitution:
                {
                    attributes.Constitution += amount;
                    break;
                }
            case Attributes.Dexterity:
                {
                    attributes.Dexterity += amount;
                    break;
                }
            case Attributes.Intelligence:
                {
                    attributes.Intelligence += amount;
                    break;
                }
            case Attributes.Strength:
                {
                    attributes.Strength += amount;
                    break;
                }
            case Attributes.Wisdom:
                {
                    attributes.Wisdom += amount;
                    break;
                }
            default:
                break;
        }
    }
    #endregion

    #region Stat Calculations
    int calculateEncumbrance()
    {
        //calculate encumbrance from stats here
        return 400;
    }

    int calcXPForNextLevel()
    {
        //do calc here
        return 10;
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
    public void applyEffect(NPCProperties e, int priority, bool isItem)
    {
        if (e != NPCProperties.NONE)
        {
            if (effects[(long)e] == null)
            {
                NPCEffect eff = new NPCEffect(e, this);
                if (this.properties[NPCProperties.NONE])
                {
                    this.properties[NPCProperties.NONE] = false;
                }

                effects[(long)e] = eff;
                eff.apply(priority, isItem);

            }
            else
            {
                effects[(long)e].apply(priority, isItem);
            }
        }
    }

    /**
     * This entirely removes the effect from the NPC. If applied again later, it'll create a new one.
     */
    public void removeEffect(NPCProperties e)
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
        }
        else
        {
            //do nothing, the item isn't there
        }
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
        equipment = new NPCEquipment(this.bodyparts);
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
        this.race = NPCRace.NONE;
        this.role = NPCRole.NONE;
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

        this.role = x.SelectEnum<NPCRole>("role");
        this.race = x.SelectEnum<NPCRace>("race");

        //property parsing
        List<XMLNode> xprops = x.select("properties").selectList("property");
        foreach (XMLNode xnode in xprops)
        {
            NPCProperties np = Nifty.StringToEnum<NPCProperties>(xnode.SelectString("name"));
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

    private int basePoints;
    private int currentPoints;

    public void UpdateTurn()
    {
        //this does nothing right now!
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
