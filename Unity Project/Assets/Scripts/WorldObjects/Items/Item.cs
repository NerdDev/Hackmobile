using UnityEngine;
using System.Collections;

public class Item : WorldObject {
	
	//consider some abtract/virtual methods and variables here for cleanliness
	//ex:  register my existence and key information to the item master singleton so i'm not lost in the game world
	
	// Use this for initialization
	void Awake () 
	{
		RegisterItemToSingleton();
	}
	
		
	
	
	public virtual void RegisterItemToSingleton() //if we decide to make Item.cs structural only, then switch these to abstract
	{
		BigBoss.ItemMaster.AddItemToMasterList(this);//registering existence with singleton
	}
	
	public virtual void DestroyThisItem()
	{
		
		BigBoss.ItemMaster.RemoveItemFromMasterList(this);//removing existence with singleton
		Destroy (this.gameObject);
	}
	
}
