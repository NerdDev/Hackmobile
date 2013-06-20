using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using XML;

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
    }

    public virtual void DestroyThisItem()
    {
        BigBoss.NPCManager.RemoveNPCFromMasterList(this);
        Destroy(this.gameObject);
    }
    #endregion

    //TODO: Refactor base inventory and/or inventory to somewhere else?
    //  -- do an NPCInstance() class which translates baseInventory into actual inventory?
    //  -- Refactor all public access to get/set functions? (annoying with unity, can't modify then).

    #region NPC Properties
    //Properties
    //currently none, outsourced to their own classes

    //Local variables
    public List<NPCItem> baseInventory = new List<NPCItem>();
    List<GameObject> inventory;

    //All sets of flags
    public Flags flags = new Flags(NPCFlags.NONE);
    public Flags resists = new Flags(NPCResistances.NONE);
    public Flags props = new Flags(NPCProperties.NONE);

    //Enums
    public NPCRace race;
    public NPCRole role;

    //Separate classes
    public NPCStats stats = new NPCStats();
    public NPCBodyParts bodyparts = new NPCBodyParts();
    #endregion

    public NPC()
    {
    }

    #region NPC Management for instances
    public void setData(string npcName)
    {
        this.setData(BigBoss.NPCManager.getNPC(npcName));
    }

    public void setData(NPC npc)
    {
        base.setData(npc);
        //classes
        this.stats.setData(npc.stats);
        this.bodyparts.setData(npc.bodyparts);
        //flags
        //this.flags.set(npc.flags);
        //this.resists.set(npc.resists);
        //this.props.set(npc.props);
        //enums
        this.race = npc.race;
        this.role = npc.role;

        //inventory
        //TODO: needs conversion from baseInventory into actual inventory
        //this.inventory.AddRange(npc.inventory);
    }

    //TODO: Fill in null data.
    public override void setNull()
    {
        base.setNull();
    }

    #region XML Parsing
    public void parseXML(XMLNode x)
    {
        this.role = (NPCRole)Enum.Parse(typeof(NPCRole), x.select("role").getText(), true);
        this.race = (NPCRace)Enum.Parse(typeof(NPCRace), x.select("race").getText(), true);
        this.Model = x.select("model").getText();
        this.ModelTexture = x.select("modeltexture").getText();

        //property parsing
        string temp = x.select("properties").getText();
        string[] split = temp.Split(',');
        for (int i = 0; i < split.Length; i++)
        {
            //NPCProperties pr = (NPCProperties) Enum.Parse(typeof(NPCProperties), split[i], true);
            this.props[(NPCProperties)Enum.Parse(typeof(NPCProperties), split[i].Trim(), true)] = true;
        }

        //flag parsing
        temp = x.select("flags").getText();
        split = temp.Split(',');
        for (int i = 0; i < split.Length; i++)
        {
            //NPCFlags fl = (NPCFlags) Enum.Parse(typeof(NPCFlags), split[i], true);
            this.flags[(NPCFlags)Enum.Parse(typeof(NPCFlags), split[i].Trim(), true)] = true;
        }

        //body part data
        this.bodyparts.parseXML(x.select("bodyparts"));

        //stats
        this.stats.parseXML(x.select("stats"));

        //inventory
        foreach (XMLNode xnode in x.select("inventory").get())
        {
            Debug.Log(xnode.getKey());
            NPCItem baseItem = new NPCItem();
            baseItem.parseXML(xnode);
            this.baseInventory.Add(baseItem);
        }
    }
    #endregion

    #endregion
}
