using System;
using System.Collections.Generic;
using UnityEngine;

public class GridSpace : MonoBehaviour
{
    public List<Item> items = new List<Item>();
    public NPC npc = null;
    public Point coords;
    System.Random rand = new System.Random();

    void Start()
    {
        if (rand.Next(100) > 95)
        {
            //this.setNPC(((GameObject)Instantiate(BigBoss.Prefabs.enemy1, this.transform.position, Quaternion.identity)).GetComponent<NPC>());
            //this.getNPC().setData(BigBoss.WorldObjectManager.getNPC("newt"));
            //this.getNPC().IsActive = true;
            //this.getNPC().gameObject.transform.parent = npcHolder.transform;
        }
    }

    public bool hasNPC()
    {
        if (npc == null)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    //Call after hasNPC()
    public NPC getNPC()
    {
        return npc;
    }

    public void setNPC(NPC n)
    {
        this.npc = n;
    }
}