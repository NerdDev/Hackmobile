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
	
	public Potion CreatePotion(Vector3 location,Potion.Size size)
	{
		//Instantiation:
		GameObject go = Instantiate(BigBoss.Prefabs.potion)as GameObject;
		//Relocating to desired place:
		go.transform.position = location;
		//Capturing reference to script:
		Potion thePotionScript = (Potion)go.GetComponent(typeof(Potion));
		thePotionScript.potionSize = size;
		//This part is a bit ghetto but we'll come back to it:
		if (thePotionScript.potionSize == Potion.Size.Small)
		{
			thePotionScript.restoreHealthAmount = 25;	
		}
		//returning a reference to the instance of this exact potion
		return thePotionScript;
	}
}
