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
    List<string[]> props = null;
    #endregion

    #region NPC Instance Properties
    //Initialized all to null for the base.
    //Converted from base properties upon creation of instance.
    public Dictionary<Item, int> inventory = null;
    #endregion

    #region Generic NPC Properties
    //Properties stored here.
    public GenericFlags<NPCProperties> properties = new GenericFlags<NPCProperties>();
    //When accessing, use the long value of NPC Properties.
    protected NPCEffect[] effects = new NPCEffect[(long)NPCProperties.LAST];

    //All sets of flags
    Flags<NPCFlags> flags = new Flags<NPCFlags>();

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
    public void applyEffect(NPCProperties e, int priority, bool item)
    {
        if (effects[(long) e] == null)
        {
            NPCEffect eff = new NPCEffect(e, this);

            effects[(long) e] = eff;
            eff.apply(priority, item);
            
        }
        else
        {
            effects[(long) e].apply(priority, item);
        }
    }
    #endregion

    #region Equipment Management

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
        for (int i = 0; i < effects.Length; i++)
        {
            effects[i] = null;
        }
    }

    #region XML Parsing
    public void parseXML(XMLNode x)
    {
        //name is handled in DataManager so we get the GameObject name
        base.parseXML(x);

        this.role = x.SelectEnum<NPCRole>("role");
        this.race = x.SelectEnum<NPCRace>("race");

        //property parsing
        this.props = new List<string[]>();
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
