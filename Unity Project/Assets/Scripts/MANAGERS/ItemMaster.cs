using UnityEngine;
using System.Collections;
using System .Collections.Generic;
using System.Linq;
using System;


public class ItemMaster : MonoBehaviour {
	
	//potions, stethoscope, materials, quest items, etc.
	
	
	//This may be changed depending on how we utilize objects in the game world
	//globally accessible list of types for iteration:
	public List<Item> TotalItemsInExistence = new List<Item>();
    MaterialType nullMaterial { get; set; }
    Item nullItem { get; set; }

    Dictionary<string, Item> baseItems = new Dictionary<string, Item>();
    Dictionary<string, MaterialType> materials = new Dictionary<string, MaterialType>();
		
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Debug.Log(TotalItemsInExistence.Count);   //for quick debugging only
	}

    private void initializeNullData()
    {
        //Null material
        nullMaterial = new MaterialType();
        nullMaterial.setNull();

        //Null item
        GameObject nullGOItem = new GameObject("nullItem");
        nullItem = nullGOItem.AddComponent<Item>();
        nullItem.setNull();
    }
	
	public void AddItemToMasterList(Item theItemScript)
	{
	
		TotalItemsInExistence.Add (theItemScript);
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
        Debug.Log(item.Model);
        MeshFilter mf = go.AddComponent<MeshFilter>();
        mf.mesh = (Resources.Load(item.Model) as GameObject).GetComponent<MeshFilter>().mesh;
        MeshRenderer mr = go.AddComponent<MeshRenderer>();
        Debug.Log(item.ModelTexture);
        mr.material = Resources.Load(item.ModelTexture) as Material;
		return item;
	}

    public Item CreateRandomItem(Vector3 location)
    {
        Item i = (Item) Nifty.RandomValue(baseItems);
        return CreateItem(location, i.Name);
    }
}
