using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldObjectManager : MonoBehaviour, IManager {

    //This may be changed depending on how we utilize objects in the game world
    //globally accessible list of types for iteration:
    public List<Item> TotalItemsInExistence = new List<Item>();
    MaterialType nullMaterial { get; set; }
    Item nullItem { get; set; }
    NPC nullNPC { get; set; }

    Dictionary<string, NPC> baseNPCs = new Dictionary<string, NPC>();
    Dictionary<string, Item> baseItems = new Dictionary<string, Item>();
    Dictionary<string, List<Item>> itemCategories = new Dictionary<string, List<Item>>();
    Dictionary<string, MaterialType> materials = new Dictionary<string, MaterialType>();

    public void Initialize()
    {
        BigBoss.Debug.w(DebugManager.Logs.Main, "Starting World Object Manager");
        //Initialize Data Manager
        DataManager dm = BigBoss.Data;
        initializeNullData();
    }

    void Start()
    {
    }

    private void initializeNullData()
    {
        GameObject nullGONPC = new GameObject("nullNPC");
        nullNPC = nullGONPC.AddComponent<NPC>();
        nullNPC.setNull();

        //Null material
        nullMaterial = new MaterialType();
        nullMaterial.setNull();

        //Null item
        GameObject nullGOItem = new GameObject("nullItem");
        nullItem = nullGOItem.AddComponent<Item>();
        nullItem.setNull();
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
        if (getNPCs().ContainsKey(npcName))
        {
            return getNPCs()[npcName];
        }
        else
        {
            return nullNPC;
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
        if (getItems().ContainsKey(itemName))
        {
            return getItems()[itemName];
        }
        else
        {
            return nullItem;
        }
    }

    public MaterialType getMaterial(string mat)
    {
        if (getMaterials().ContainsKey(mat))
        {
            return getMaterials()[mat];
        }
        else
        {
            return nullMaterial;
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
        Item i = item.AddComponent<Item>();
        Item baseI = BigBoss.WorldObjectManager.getItem(itemName);
        i.setData(baseI);
        i.IsActive = true;
        return i;
    }

    public Item CreateRandomItem()
    {
        Item i = (Item)Nifty.RandomValue(baseItems);
        return CreateItem(i.Name);
    }
}
