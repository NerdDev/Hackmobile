using UnityEngine;
using System.Collections;
using System .Collections.Generic;


public class ItemMaster : MonoBehaviour {
	
	//potions, stethoscope, materials, quest items, etc.
	
	
	//This may be changed depending on how we utilize objects in the game world
	//globally accessible list of types for iteration:
	public List<Item> TotalItemsInExistence = new List<Item>(); 
		
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		//Debug.Log(TotalItemsInExistence.Count);   //for quick debugging only
	}
	
	
	public void AddItemToMasterList(Item theItemScript)
	{
	
		TotalItemsInExistence.Add (theItemScript);
	}
	
	public void RemoveItemFromMasterList(Item theItemScript)
	{
	
		TotalItemsInExistence.Remove(theItemScript);//look up this generic method and see if we leave a memory leak
	}

    public Item CreateItem(Vector3 location, string itemName)
	{
        GameObject go = new GameObject();
		go.transform.position = location;
        Item item = go.AddComponent<Item>();
        item.setData(itemName);
        MeshFilter mf = go.AddComponent<MeshFilter>();
        mf.mesh = (Resources.Load(item.Model) as GameObject).GetComponent<MeshFilter>().mesh;
        MeshRenderer mr = go.AddComponent<MeshRenderer>();
        Debug.Log(item.ModelTexture);
        mr.material = Resources.Load(item.ModelTexture) as Material;
		return item;
	}
}
