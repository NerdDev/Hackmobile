using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class WorldObjectManager : MonoBehaviour, IManager {

    //This may be changed depending on how we utilize objects in the game world
    //globally accessible list of types for iteration:
    public List<Item> TotalItemsInExistence = new List<Item>();

    Dictionary<string, NPC> baseNPCs = new Dictionary<string, NPC>();
    Dictionary<string, Item> baseItems = new Dictionary<string, Item>();
    Dictionary<string, List<Item>> itemCategories = new Dictionary<string, List<Item>>();
    Dictionary<string, MaterialType> materials = new Dictionary<string, MaterialType>();

    public void Initialize()
    {
        BigBoss.Debug.w(Logs.Main, "Starting World Object Manager");
        //Initialize Data Manager by asking for it
        DataManager dm = BigBoss.Data;
    }

    void Start()
    {
    }

	public List<NPC> totalNumberOfNPCs = new List<NPC>();

    public void AddNPCToMasterList(NPC npc)
    {
        totalNumberOfNPCs.Add(npc);
    }

    public void RemoveNPCFromMasterList(NPC npc)
    {
        totalNumberOfNPCs.Remove(npc);//look up this generic method and see if we leave a memory leak
    }

    public NPC getNPC(string npcName)
    {
        NPC n;
        if (getNPCs().TryGetValue(npcName, out n))
        {
            return n;
        }
        else
        {
            return new NPC();
        }
    }

    public Dictionary<string, NPC> getNPCs()
    {
        return baseNPCs;
    }

    public List<NPC> getExistingNPCs(params Keywords[] kw)
    {
        List<NPC> list = new List<NPC>();
        foreach (NPC n in totalNumberOfNPCs)
        {
            if (n.keywords.getAnd(kw))
            {
                list.Add(n);
            }
        }
        return list;
    }

    public void AddItemToMasterList(Item theItemScript)
    {
        TotalItemsInExistence.Add(theItemScript);
    }

    public void RemoveItemFromMasterList(Item theItemScript)
    {
        TotalItemsInExistence.Remove(theItemScript);//look up this generic method and see if we leave a memory leak
    }

    public Item getItem(string itemName)
    {
        Item i;
        if (getItems().TryGetValue(itemName, out i))
        {
            return i;
        }
        else
        {
            return new Item();
        }
    }

    public MaterialType getMaterial(string mat)
    {
        MaterialType m;
        if (getMaterials().TryGetValue(mat, out m))
        {
            return m;
        }
        else
        {
            return new MaterialType();
        }
    }

    public Dictionary<string, Item> getItems()
    {
        return baseItems;
    }

    public Dictionary<string, List<Item>> getCategories()
    {
        return itemCategories;
    }

    public Dictionary<string, MaterialType> getMaterials()
    {
        return materials;
    }

    public Item CreateItem(string itemName)
    {
        GameObject item = new GameObject();
        WOInstance instance = item.AddComponent<WOInstance>();
        Item i = instance.SetTo(BigBoss.WorldObject.getItem(itemName));
        i.IsActive = true;
        return i;
    }

    public Item CreateRandomItem()
    {
        Item i = (Item)Nifty.RandomValue(baseItems);
        return CreateItem(i.Name);
    }
}
