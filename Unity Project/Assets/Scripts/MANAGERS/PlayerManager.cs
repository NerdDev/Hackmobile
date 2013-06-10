using UnityEngine;
using System.Collections;

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
	
	private int playerStrength = 10;//temporarily hard-coded
	public int PlayerStrength{get{ return playerMaxHealth;}}  //read only - change through Adjust()
	
	private int playerDexterity = 10;   //temporarily hard-coded
	public int PlayerDexterity{get{ return playerMaxHealth;}}  //read only - change through Adjust()
	
	private int playerIntelligence = 8;//temporarily hard-coded
	public int PlayerIntelligence{get{ return playerMaxHealth;}}  //read only - change through Adjust()
	
	private int playerWisdom = 6;  //temporarily hard-coded
	public int PlayerWisdom{get{ return playerMaxHealth;}}  //read only - change through Adjust()
	
	#endregion
	
	
	#region ENUMERATIONS OF ALL SORTS
	
	public enum PlayerProfessions 
	{
		Warrior,
		Mage,
		Ranger
		
	}
	public PlayerProfessions PlayerChosenProfession;
	
	#endregion
	// Use this for initialization
	void Start () 
	{
		
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
	
}
