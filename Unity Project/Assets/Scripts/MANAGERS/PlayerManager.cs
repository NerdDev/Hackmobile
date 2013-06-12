using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*   As long as this isn't an MMO, this Player class should be able to hold most if not all of player information.
 
 
 */
public class PlayerManager : MonoBehaviour {
	
	private GameObject playerAvatar;
	public GameObject PlayerAvatar{get{return playerAvatar;}}//read only global reference to the hero gameobject
	
	#region HERO STAT VARS   //only health and attribute stats pertaining to hero go here
		//Attributes here are read-only for GUI access every frame - modify through functions below
	private int playerMaxHealth = 100;  //temporarily hard-coded
	public int PlayerMaxHealth{get{ return playerMaxHealth;}}  //read only - change through Adjust()
	
	private int playerCurrentHealth = 50;
	public int PlayerCurrentHealth{get{ return playerCurrentHealth;}} //read only Player Health - change through Adjust()
	
	private int playerCurrentXP = 0;  //temporarily hard-coded
	public int PlayerCurrentXP{get{ return playerCurrentXP;}}  //read only - change through Adjust()
	
	private int playerCurrentLevel = 1;
	public int PlayerCurrentLevel{get{ return playerCurrentLevel;}} //read only Player Health - change through Adjust()
	
	private int playerStrength = 10;//temporarily hard-coded
	public int PlayerStrength{get{ return playerMaxHealth;}}  //read only - change through Adjust()
	
	private int playerDexterity = 10;   //temporarily hard-coded
	public int PlayerDexterity{get{ return playerMaxHealth;}}  //read only - change through Adjust()
	
	private int playerIntelligence = 8;//temporarily hard-coded
	public int PlayerIntelligence{get{ return playerMaxHealth;}}  //read only - change through Adjust()
	
	private int playerWisdom = 6;  //temporarily hard-coded
	public int PlayerWisdom{get{ return playerMaxHealth;}}  //read only - change through Adjust()
	
	private int playerHunger = 0;  //temporarily hard-coded
	public int PlayerHunger{get{ return playerHunger;}}  //read only - change through Adjust()
	
	private int playerHungerMax = 1000;  //temporarily hard-coded
	public int PlayerHungerMax{get{ return playerHungerMax;}}  //read only - change through Adjust()
	
	private int playerEncumbrance = 0;  //temporarily hard-coded
	public int PlayerEncumbrance{get{ return playerEncumbrance;}}
	
	private int playerEncumbranceMax = 10;  //temporarily hard-coded
	public int PlayerEncumbranceMax{get{ return playerEncumbranceMax;}}
	#endregion
	
	#region INVENTORY
	private float playerInventoryWeight = 0;  //temporarily hard-coded
	public float PlayerInventoryWeight{get{ return playerInventoryWeight;}}  //read only - change through Adjust()
	
	private float playerInventoryWeightMax = 0;  //temporarily hard-coded
	public float PlayerInventoryWeightMax{get{ return  playerInventoryWeightMax;}}
	//Inventory Array - Will have to confirm typing when NGUI integration is set up...
	public List<GameObject> PlayerInventory = new List<GameObject>();
	#endregion
	
	
	#region ENUMERATIONS OF ALL SORTS
	
	public enum PlayerProfessions 
	{
		Warrior,
		Mage,
		Ranger,
		Samurai,
		
	}
	public PlayerProfessions PlayerChosenProfession;
	
	public enum HungerLevel 
	{
		Starving,
		Hungry,
		Satisfied,
		Full,
		Stuffed
		
	}
	public HungerLevel CurrentHungerLevel;
	#endregion
	// Use this for initialization
	void Start () 
	{
		CalculatePlayerInventoryMaxWeight();
		DecideHungerLevel();
		
		//AdjustPlayerHealth(gameObject,0);
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
	
	
	//Adjustment of health and attributes should follow the health examples.
	public void AdjustPlayerHealth(GameObject senderObj,int amount)//if this is our only health function, consider adding a sender component
	{
	
		Debug.Log("IncreasePlayerHealth() called from " + senderObj + ".  Player's health to be adjusted by a raw amount of " + amount);
		//Can't raise health over maximum:
		int difference = PlayerMaxHealth - PlayerCurrentHealth;
		amount = (int)Mathf.Clamp(amount,0f,(float)difference);
		playerCurrentHealth += (int)amount; 
		BigBoss.Gooey.UpdateHealthBar();
		
		Debug.Log("IncreasePlayerHealth() successfully completed - " + PlayerCurrentHealth + " is current health.");
	}
	
	public void AdjustPlayerMaxHealth(GameObject senderObj,int amount)//if this is our only health function, consider adding a sender component
	{
	
		Debug.Log("IncreasePlayerMaxHealth() called from " + senderObj + ".  Player's max health to be adjusted by " + amount);
		playerMaxHealth += amount;  //come back and install logic/failsafes
		Debug.Log("IncreasePlayerMaxHealth() successfully completed - " + PlayerCurrentHealth + " is current health.");
	}
	
	//A STRAIGHT UP SETHEALTH() COMMAND I THINK IS HIGHLY RECOMMENDED JUST IN CASE
	
	
	
	public void DecideHungerLevel()
	{
		//For coloring the text:
		Color col;
		//Trickle if statement to set enum, color, and update GUI:
		if (PlayerHunger < 150)
		{
			CurrentHungerLevel = HungerLevel.Starving;
			col = Color.red;
			BigBoss.Gooey.UpdateHungerText(col);
		}
		else if (PlayerHunger < 350)
		{
			CurrentHungerLevel = HungerLevel.Hungry;
			col = Color.magenta;
			BigBoss.Gooey.UpdateHungerText(col);
		}
		else if (PlayerHunger < 700)
		{
			CurrentHungerLevel = HungerLevel.Satisfied;
			col = Color.cyan;
			BigBoss.Gooey.UpdateHungerText(col);
		}
		else if (PlayerHunger < 900)
		{
			CurrentHungerLevel = HungerLevel.Full;
			col = Color.blue;
			BigBoss.Gooey.UpdateHungerText(col);
		}
		else if (PlayerHunger < 1000)
		{
			CurrentHungerLevel = HungerLevel.Stuffed;
			col = Color.yellow;
			BigBoss.Gooey.UpdateHungerText(col);
		}
		
		
	}
	
	public void AdjustHungerPoints(int amount)
	{
	
		//Increment Hunger, but not less than 0 or over 100:
		playerHunger += (int)Mathf.Clamp (amount,0f,100f);
		DecideHungerLevel();
	}
	
	public float GetXPToNextLevel()//UNDER CONSTRUCTION FOR DEBUG ONLY
	{
	
		//NOT FINAL NOT FINAL NOT FINAL
		int level = PlayerCurrentLevel;
		float xpToNext = level * (100+level);
		Debug.Log("XP To Next Level() Calc'd : " + xpToNext);
		return xpToNext;
	}
	
	 public void GainLevel()//UNDER CONSTRUCTION FOR DEBUG ONLY
	{
		playerCurrentLevel++;
		CalculatePlayerInventoryMaxWeight();//Replace this with the increase stats() method
		Debug.Log("Player gained level " + playerCurrentLevel);
	}

	public void CalculatePlayerInventoryMaxWeight()//Should only be cal'c when weight changes or attribute on player is affected
	{
		
		playerInventoryWeightMax = 25 * playerStrength/3;
		Debug.Log("Max weight calculated: " + playerInventoryWeightMax);
	}
}
