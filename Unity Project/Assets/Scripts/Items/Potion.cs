using UnityEngine;
using System.Collections;

public class Potion : Item {
	
	public int restoreHealthAmount = 0;
	
	
	public enum Size 
	{
		Small,
		Medium,
		Large		
	}
	public Size potionSize;
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	void OnMouseDown()
	{
	
		DrinkPotion();
	}
	
	public void DrinkPotion ()
	{
		BigBoss.PlayerInfo.AdjustPlayerHealth(gameObject,restoreHealthAmount);
		DestroyThisItem();//inherited from Item.cs
	}
}
