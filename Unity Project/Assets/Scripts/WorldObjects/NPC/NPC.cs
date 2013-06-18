using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
    //Properties
    private string name;
    public string Name
    {
        get { return name; }
        set { this.name = value; }
    }

    //Local variables
    List<GameObject> baseInventory;
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

    public NPC()
    {
    }

    public void setData(NPC npc)
    {
        this.stats.setData(npc.stats);
        this.bodyparts.setData(npc.bodyparts);
        this.flags.set(npc.flags);
        this.resists.set(npc.resists);
        this.props.set(npc.props);
        this.race = npc.race;
        this.role = npc.role;
        this.inventory.AddRange(npc.inventory);
    }
}
