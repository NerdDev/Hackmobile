using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using XML;

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
    #endregion

    #region Generic NPC Properties
    //Properties stored here.
    public Dictionary<Enum, Effect> effects = new Dictionary<Enum, Effect>();

    //All sets of flags
    Flags flags = new Flags(NPCFlags.NONE);

    //Enums
    public NPCRace race;
    public NPCRole role;

    //Separate classes
    public NPCStats stats = new NPCStats();
    public NPCBodyParts bodyparts = new NPCBodyParts();
    public NPCEquipment equipment = new NPCEquipment();
    
    #endregion

    public NPC()
    {
    }

    #region Effects
    public void applyEffect(Enum e, bool positive, Priority p)
    {
        if (!effects.ContainsKey(e))
        {
            effects.Add(e, new Effect());
        }
        if (positive)
        {
            effects[e].apply(p);
        }
        else
        {
            effects[e].remove(p);
        }
    }
    #endregion

    #region Get/Set of Properties and Flags
    public bool getProperty(Enum e)
    {
        if (effects.ContainsKey(e))
        {
            return effects[e].On;
        }
        else
        {
            return false;
        }
    }

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
        this.equipment = npc.equipment.Copy();
        //flags
        this.flags = npc.flags.Copy();
        //enums
        this.race = npc.race;
        this.role = npc.role;
        //lists
        this.effects = npc.effects.Copy();

        //inventory
        //TODO: needs conversion from baseInventory into actual inventory
    }

    public override void setNull()
    {
        base.setNull();
        this.stats.setNull();
        this.bodyparts.setNull();
        this.equipment.setNull();
        this.flags.setNull();
        this.race = NPCRace.NONE;
        this.role = NPCRole.NONE;
        this.effects.Clear();
    }

    #region XML Parsing
    public void parseXML(XMLNode x)
    {
        //name is handled in DataManager so we get the GameObject name
        base.parseXML(x);

        this.role = x.SelectEnum<NPCRole>("role");
        this.race = x.SelectEnum<NPCRace>("race");

        //property parsing
        string temp = x.SelectString("properties");
        string[] split = temp.Split(',');
        for (int i = 0; i < split.Length; i++)
        {
            //NPCProperties pr = (NPCProperties) Enum.Parse(typeof(NPCProperties), split[i], true);
            this.effects.Add(
                Nifty.StringToEnum<NPCProperties>(split[i].Trim()),
                new Effect(true));
        }

        //resistance parsing
        temp = x.SelectString("resistances");
        split = temp.Split(',');
        for (int i = 0; i < split.Length; i++)
        {
            //NPCProperties pr = (NPCProperties) Enum.Parse(typeof(NPCProperties), split[i], true);
            this.effects.Add(
                Nifty.StringToEnum<NPCResistances>(split[i].Trim()), 
                new Effect(true));
        }

        //flag parsing
        temp = x.SelectString("flags");
        split = temp.Split(',');
        for (int i = 0; i < split.Length; i++)
        {
            //NPCFlags fl = (NPCFlags) Enum.Parse(typeof(NPCFlags), split[i], true);
            this.flags[Nifty.StringToEnum<NPCFlags>(split[i].Trim())] = true;
        }

        //body part data
        this.bodyparts.parseXML(x.select("bodyparts"));

        //stats
        this.stats.parseXML(x.select("stats"));

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
