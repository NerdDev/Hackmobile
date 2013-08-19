using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WorldObjectManager : MonoBehaviour {

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

    void Start()
    {
        initializeNullData();
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

	public List<NPC> totalNumberOfNPCs;

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

    public void Log()
    {
        DebugManager.CreateNewLog(DebugManager.Logs.NPCs, "NPC Logs");
        foreach (NPC n in baseNPCs.Values)
        {
            //DebugManager.w(DebugManager.Logs.NPCs, System.ObjectDumper.Dump(n));
            List<string> filter = new List<string>();
            filter.Add("Camera");
            filter.Add("Rigidbody");
            filter.Add("GameObject");
            filter.Add("Light");
            filter.Add("Animation");
            filter.Add("ConstantForce");
            filter.Add("Renderer");
            filter.Add("AudioSource");
            filter.Add("GUIText");
            filter.Add("NetworkView");
            filter.Add("GUIElement");
            filter.Add("GUITexture");
            filter.Add("HingeJoint");
            filter.Add("Collider");
            filter.Add("ParticleEmitter");
            filter.Add("ParticleSystem");

            string s = System.ObjDump.Dump(n, filter);
            Debug.Log(s);
            DebugManager.printHeader(DebugManager.Logs.NPCs, "NPC: " + n.Name);
            DebugManager.w(DebugManager.Logs.NPCs, s);
        }
        DebugManager.printFooter(DebugManager.Logs.NPCs);
    }

    public Dictionary<string, NPC> getNPCs()
    {
        return baseNPCs;
    }

	public NPC CreateNPC(Vector3 location, string npcName)
	{
        GameObject go = new GameObject(npcName);
        go.transform.position = location;
        NPC npc = go.AddComponent<NPC>();
        npc.setData(npcName);
        MeshFilter mf = go.AddComponent<MeshFilter>();
        mf.mesh = (Resources.Load(npc.Model) as GameObject).GetComponent<MeshFilter>().mesh;
        MeshRenderer mr = go.AddComponent<MeshRenderer>();
        mr.material = Resources.Load(npc.ModelTexture) as Material;
		return npc;
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

    public Item CreateItem(Vector3 location, string itemName)
    {
        GameObject go = new GameObject(itemName);
        go.transform.position = location;
        Item item = go.AddComponent<Item>();
        item.setData(itemName);
        MeshFilter mf = go.AddComponent<MeshFilter>();
        mf.mesh = (Resources.Load(item.Model) as GameObject).GetComponent<MeshFilter>().mesh;
        MeshRenderer mr = go.AddComponent<MeshRenderer>();
        mr.material = Resources.Load(item.ModelTexture) as Material;
        return item;
    }

    public Item CreateRandomItem(Vector3 location)
    {
        Item i = (Item)Nifty.RandomValue(baseItems);
        return CreateItem(location, i.Name);
    }
}