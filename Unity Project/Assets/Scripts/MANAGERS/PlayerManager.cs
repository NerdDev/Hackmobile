using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*   As long as this isn't an MMO, this Player class should be able to hold most if not all of player information.
 
 
 */
public class PlayerManager : MonoBehaviour {
	
	
	//Name/title info:
	private string playerChosenName;
	public string PlayerChosenName{get{ return playerChosenName;}}  //read only - set at char creation
	
	private string playerTitle;//student, apprentice, grunt, practitioner, etc. etc.
	public string PlayerTitle{get{ return playerTitle;}}  //read only - updated via class info
	
	private string playerTitleCombatArea;//wizardry, combat, bushido, medicine, chemistry, sightseeing, etc
	public string PlayerTitleCombatArea{get{ return playerTitleCombatArea;}}  //read only - updated via class info
	
	public GameObject playerAvatar;
	public GameObject PlayerAvatar{get{return playerAvatar;}}//read only global reference to the hero gameobject
	
	#region HERO STAT VARS   //only health and attribute stats pertaining to hero go here
		//Attributes here are read-only for GUI access every frame - modify through functions below
	private int playerMaxHealth = 100;  //temporarily hard-coded
	public int PlayerMaxHealth{get{ return playerMaxHealth;}}  //read only - change through Adjust()
	
	private int playerCurrentHealth = 50;
	public int PlayerCurrentHealth{get{ return playerCurrentHealth;}} //read only Player Health - change through Adjust()
	
	private int playerMaxWillpower = 100;  //temporarily hard-coded
	public int PlayerMaxWillpower{get{ return playerMaxWillpower;}}  //read only - change through Adjust()
	
	private int playerCurrentWillpower = 50;
	public int PlayerCurrentWillpower{get{ return playerCurrentWillpower;}} //read only Player Health - change through Adjust()
	
	private int playerCurrentXPForThisLevel = 0;  //temporarily hard-coded
	public int PlayerCurrentXPForThisLevel{get{ return playerCurrentXPForThisLevel;}}  //read only - change through Adjust()
	
	private int playerLifeTimeXP = 0;  //temporarily hard-coded
	public int PlayerLifeTimeXP{get{ return playerLifeTimeXP;}}
	
	private int playerCurrentLevel = 1;
	public int PlayerCurrentLevel{get{ return playerCurrentLevel;}} //read only Player Health - change through Adjust()
	
	private int playerStrength = 10;//temporarily hard-coded
	public int PlayerStrength{get{ return playerMaxHealth;}}  //read only - change through Adjust()
	
	private int playerConstitution = 10;//temporarily hard-coded
	public int PlayerConstitution{get{ return playerConstitution;}}
	
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
	
	public enum PlayerProfessions //are we keeping all of these?  Rename/rework a couple maybe?
	{
		Archaeologist,
		Barbarian,
		Caveman,
		Healer,
		Knight,
		Monk,
		Priest,
		Ranger,
		Rogue,
		Samurai,
		Tourist,
		Valkyrie,
		Wizard
	}
	public PlayerProfessions PlayerChosenProfession;
	
	public enum HungerLevel 
	{
		Stuffed,
		Satiated,
		Hungry,
		Starving,
		Faint
		
	}
	public HungerLevel CurrentHungerLevel;
	
	
	public enum EncumbranceEnum  //speed and to-hit mods will be affected.  Nethack "exercises" strength and "abuses" dex/const based on this enum
	{
		Unencumbered,
		Burdened,
		Stressed,
		Strained,
		Overtaxed,
		Overloaded
		
	}
	public EncumbranceEnum CurrentPlayerEncumbrance;
	
	public enum BodyEventLocations    //This enum is for event use - monster hitting - traps, etc.  Equip locations are below
	{
		Head,
		LeftArm,
		RightArm,
		LeftHand,
		RightHand,
		Chest,
		Waist,
		LeftLeg,
		RightLeg,
		LeftKnee,
		RightKnee,
		LeftFoot,
		RightFoot
		
		
	}
	public enum BodyEquipLocations    //WIP - body inventory equipping purposes
	{
		Head,
		Neck,
		Arms,
		Hands,
		Chest,
		Waist,
		Legs,
		Feet,
		Ring1,
		Ring2,
		Ring3,
		Shield,
		Weapon,
		Spellbook
		
	}
	#endregion
	// Use this for initialization
	void Start () 
	{
		DecidePlayerTitle();
		DecidePlayerInventoryMaxWeight();
		DecideHungerLevel();
		
		
		//Debug.Log(bb.gameObject);//working
		//AdjustPlayerHealth(gameObject,0);
	}
	
	// Update is called once per frame
	void Update ()
	{
		
	}
		
	public void PlayerMoveForward()
	{
	
		gameObject.transform.Translate(new Vector3(0,0,1f));  //THIS IS TEMPORARYYYYYYYYYYYYYYYY
		
	}
	
	
	
	
	#region ALL THE DECISION MAKING/UPDATING/COMMUNICATING TO OTHER SCRIPTS.  ALSO CONTAINED IN ANY ADJUST() CALLS.
	private void DecideHungerLevel()//depending on other's needs I can 
	{
		//For coloring the text:
		Color col;
		//Trickle if statement to set enum, color, and update GUI:
		if (PlayerHunger < 50)
		{
			CurrentHungerLevel = HungerLevel.Faint;
			col = Color.red;
			BigBoss.Gooey.UpdateHungerText(col);
		}
		else if (PlayerHunger < 130)
		{
			CurrentHungerLevel = HungerLevel.Starving;
			col = Color.magenta;
			BigBoss.Gooey.UpdateHungerText(col);
		}
		else if (PlayerHunger < 500)
		{
			CurrentHungerLevel = HungerLevel.Hungry;
			col = Color.cyan;
			BigBoss.Gooey.UpdateHungerText(col);
		}
		else if (PlayerHunger < 800)
		{
			CurrentHungerLevel = HungerLevel.Satiated;
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
	
	private float DecideXPToNextLevel()//UNDER CONSTRUCTION FOR DEBUG ONLY
	{
	
		//NOT FINAL NOT FINAL NOT FINAL
		float xpToNext = 100 + ((Mathf.Pow(playerCurrentLevel,3f)/2));
		Debug.Log("XP To Next Level() Calc'd : " + xpToNext);
		return xpToNext;
	}
	
	private bool DecidePlayerInventoryMaxWeight()//Should only be cal'c when weight changes or attribute on player is affected
	{
		
		playerInventoryWeightMax = (25 * (playerStrength + playerConstitution) + 50);
		//This a good spot to add in buffs that would be outside the normal calc?
		
		Debug.Log("Max weight calculated: " + playerInventoryWeightMax);
		return true;
	}
	private string DecidePlayerTitle()
	{
		
		//Called in order to figure out what the player's title should be given all variables (titles taken verbatim from nethack:
		switch (PlayerChosenProfession) 
		{
			case PlayerProfessions.Archaeologist://I WANT A TREASURE HUNTER/TOMB RAIDER/ RELIC HUNTER
			{
				//Trickle if check for level:
				if (playerCurrentLevel < 2)
				{
					playerTitle = "Digger";	
				}
				
				else if (playerCurrentLevel < 9)
				{
					playerTitle = "Investigator";
				}
				else if (playerCurrentLevel < 13)
				{
					playerTitle = "Exhumer";
				}
				else if (playerCurrentLevel < 17)
				{
					playerTitle = "Excavator";
				}
				else if (playerCurrentLevel < 21)
				{
					playerTitle = "Spelunker";
				}
				else if (playerCurrentLevel < 25)
				{
					playerTitle = "Speleologist";
				}
				else if (playerCurrentLevel < 29)
				{
					playerTitle = "Collector";
				}
				else if (playerCurrentLevel < 30)
				{
					playerTitle = "Curator";
				}
				break;
			}//end case
			case PlayerProfessions.Barbarian:
			{
			
				if (playerCurrentLevel < 2)
				{
					playerTitle = "Plunderer";
				}
				else if (playerCurrentLevel < 5)
				{
					playerTitle = "Pillager";
				}
				else if (playerCurrentLevel < 9)
				{
					playerTitle = "Bandit";
				}
				else if (playerCurrentLevel < 13)
				{
					playerTitle = "Brigand";
				}
				else if (playerCurrentLevel < 17)
				{
					playerTitle = "Raider";
				}
				else if (playerCurrentLevel < 21)
				{
					playerTitle = "Reaver";
				}
				else if (playerCurrentLevel < 25)
				{
					playerTitle = "Slayer";
				}
				else if (playerCurrentLevel < 29)
				{
					playerTitle = "Chieftain";
				}
				else if (playerCurrentLevel < 30)
				{
					playerTitle = "Conqueror";
				}	
			break;
			}
			case PlayerProfessions.Caveman:
			{
				if (playerCurrentLevel < 2)
				{
					playerTitle = "Troglodyte";
				}
				else if (playerCurrentLevel < 5)
				{
					playerTitle = "Aborigine";
				}
				else if (playerCurrentLevel < 9)
				{
					playerTitle = "Wanderer";
				}
				else if (playerCurrentLevel < 13)
				{
					playerTitle = "Vagrant";
				}
				else if (playerCurrentLevel < 17)
				{
					playerTitle = "Wayfarer";
				}
				else if (playerCurrentLevel < 21)
				{
					playerTitle = "Roamer";
				}
				else if (playerCurrentLevel < 25)
				{
					playerTitle = "Nomad";
				}
				else if (playerCurrentLevel < 29)
				{
					playerTitle = "Rover";
				}
				else if (playerCurrentLevel < 30)
				{
					playerTitle = "Pioneer";
				}
				break;

			}
			case PlayerProfessions.Healer:
			{
			
				if (playerCurrentLevel < 2)
				{
					playerTitle = "Rhizotomist";
				}
				else if (playerCurrentLevel < 5)
				{
					playerTitle = "Empiric";
				}
				else if (playerCurrentLevel < 9)
				{
					playerTitle = "Embalmer";
				}
				else if (playerCurrentLevel < 13)
				{
					playerTitle = "Medicus Ossium";
				}
				else if (playerCurrentLevel < 17)
				{
					playerTitle = "Herbalist";
				}
				else if (playerCurrentLevel < 21)
				{
					playerTitle = "Magister";
				}
				else if (playerCurrentLevel < 25)
				{
					playerTitle = "Slayer";
				}
				else if (playerCurrentLevel < 29)
				{
					playerTitle = "Physician";
				}
				else if (playerCurrentLevel < 30)
				{
					playerTitle = "Chirurgeon";
				}
			break;
			}
			case PlayerProfessions.Knight:
			{
				if (playerCurrentLevel < 2)
				{
					playerTitle = "Gallant";
				}
				else if (playerCurrentLevel < 5)
				{
					playerTitle = "Esquire";
				}
				else if (playerCurrentLevel < 9)
				{
					playerTitle = "Bachelor";
				}
				else if (playerCurrentLevel < 13)
				{
					playerTitle = "Sergeant";
				}
				else if (playerCurrentLevel < 17)
				{
					playerTitle = "Knight";
				}
				else if (playerCurrentLevel < 21)
				{
					playerTitle = "Banneret";
				}
				else if (playerCurrentLevel < 25)
				{
					playerTitle = "Chevalier";
				}
				else if (playerCurrentLevel < 29)
				{
					playerTitle = "Seignieur";
				}
				else if (playerCurrentLevel < 30)
				{
					playerTitle = "Paladin";
				}
			break;
			}
//			case PlayerProfessions.Monk:
//			{
//			
//			}
//			case PlayerProfessions.Priest:
//			{
//				
//			}
//			case PlayerProfessions.Ranger:
//			{
//			
//			}
//			case PlayerProfessions.Rogue:
//			{
//				
//			}
//			case PlayerProfessions.Samurai:
//			{
//			
//			}
//			case PlayerProfessions.Tourist:
//			{
//				
//			}
//			case PlayerProfessions.Valkyrie:
//			{
//			
//			}
//			case PlayerProfessions.Wizard:
//			{
//				
//			}
			
			
		break;
		}
		
		
		string finalTitle = playerChosenName + ", " + playerTitle;// + " of " + playerTitleCombatArea;
		return finalTitle;
	}  //I'll come back and rework this so it's not retarded
	#endregion
	
	
	#region Adjust Player Stats/Attr's/Data
	public int AdjustHungerPoints(int amount) 
	{
	
		//Increment Hunger, but not less than 0 or over 100:
		playerHunger += (int)Mathf.Clamp (amount,0f,playerHungerMax);
		DecideHungerLevel();
		return playerHunger;
	}
	
	
	
	 public void AdjustLevel()//UNDER CONSTRUCTION FOR DEBUG ONLY
	{
		playerCurrentLevel++;
		DecidePlayerInventoryMaxWeight();//Replace this with the increase stats() method
		Debug.Log("Player gained level " + playerCurrentLevel);
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
	
	public void AdjustXP(GameObject senderObj,int amount)
	{
	
		Debug.Log("IncreasePlayerMaxHealth() called from " + senderObj + ".  Player's max health to be adjusted by " + amount);
		playerMaxHealth += amount;  //come back and install logic/failsafes
		Debug.Log("IncreasePlayerMaxHealth() successfully completed - " + PlayerCurrentHealth + " is current health.");
	}
	//A STRAIGHT UP SETHEALTH() COMMAND I THINK IS HIGHLY RECOMMENDED JUST IN CASE
	#endregion
	
	#region SETFUCTIONS   //THESE SHOULD BE USED IN GAME ONLY FOR VERY SPECIFIC SITUATIONS AND/OR EVENTS
	
	public int SetHealth(GameObject senderObj,int newHealthAmount)
	{
		
		Debug.Log("SetHealth() called from " + senderObj + ".  Target health should be " + newHealthAmount);
		
		playerCurrentHealth = newHealthAmount; 
		BigBoss.Gooey.UpdateHealthBar();
		Debug.Log("SetHealth() successfully completed - " + PlayerCurrentHealth + " is current health.");
		return playerCurrentHealth;
	}
	
	#endregion
}
