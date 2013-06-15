using UnityEngine;
using System.Collections;

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

    Attributes currentAttributes;
    Attributes maxAttributes;
    Stats stats;

    public void setData(NPC npc)
    {
        this.currentAttributes = npc.currentAttributes;
        this.maxAttributes = npc.maxAttributes;
        this.stats = npc.stats;
    }
}

struct Attributes
{
    public int strength;
    public int charisma;
    public int wisdom;
    public int dexterity;
    public int intelligence;
    public int constitution;
}

struct Stats
{
    public int baseHealth;
    public int currentHealth;
    public int basePower;
    public int currentPower;
    public int nutrition;
}
