using System;
using System.Collections.Generic;
using UnityEngine;

public class GridSpace : MonoBehaviour
{
    public List<Item> items = new List<Item>();
    public NPC npc = null;

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