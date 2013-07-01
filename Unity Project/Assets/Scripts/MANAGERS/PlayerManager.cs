using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*   As long as this isn't an MMO, this Player class should be able to hold most if not all of player information.
 
 
 */
public class PlayerManager : MonoBehaviour {
	
	
	//General Player Info:
	private string playerChosenName;
	public string PlayerChosenName{get{ return playerChosenName;}}  //read only - set at char creation
	
	private string playerTitle;//student, apprentice, grunt, practitioner, etc. etc.
	public string PlayerTitle{get{ return playerTitle;}}  //read only - updated via class info
	
	public GameObject playerAvatar;
	public GameObject PlayerAvatar{get{return playerAvatar;}}//read only global reference to the hero gameobject
	#region TRANSFORMS AND GRID DATA
	
	public Vector3 avatarStartLocation;
	public float tileMovementTolerance = 1f;  //radius
	//X,Y coordinate for other scripts to grab:
	private Vector2 playerGridCoordinate;
	public Vector2 PlayerGridCoordinate{get{return playerGridCoordinate;}}
	
	private Vector3 currentOccupiedGridCenterWorldPoint;
	public Vector3 CurrentOccupiedGridCenterWorldPoint{get{return currentOccupiedGridCenterWorldPoint;}}
	#endregion
	
	
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
	
	public int playerHunger = 1000;  //temporarily hard-coded
	public int PlayerHunger{get{ return playerHunger;}}  //read only - change through Adjust()
	
	private int playerHungerMax = 1000;  //temporarily hard-coded
	public int PlayerHungerMax{get{ return playerHungerMax;}}  //read only - change through Adjust()
	
	private int playerEncumbrance = 0;  //temporarily hard-coded
	public int PlayerEncumbrance{get{ return playerEncumbrance;}}
	
	private int playerEncumbranceMax = 10;  //temporarily hard-coded
	public int PlayerEncumbranceMax{get{ return playerEncumbranceMax;}}
	
	public float playerSpeed = 10;  //temporarily hard-coded
	public float PlayerSpeed{get{ return playerSpeed;}}
	
	public float playerRotationSpeed = .05f;  //temporarily hard-coded
	public float PlayerRotationSpeed{get{ return playerRotationSpeed;}}
	#endregion
	
	#region INVENTORY
	private float playerInventoryWeight = 0;  //temporarily hard-coded
	public float PlayerInventoryWeight{get{ return playerInventoryWeight;}}  //read only - change through Adjust()
	
	private float playerInventoryWeightMax = 0;  //temporarily hard-coded
	public float PlayerInventoryWeightMax{get{ return  playerInventoryWeightMax;}}
	//Inventory Array - Will have to confirm typing when NGUI integration is set up...
	public List<GameObject> PlayerInventory = new List<GameObject>();
	#endregion

    #region INVENTORY MANAGEMENT

    Dictionary<Item, int> inventory = new Dictionary<Item, int>();

    public void addToInventory(Item item)
    {
        if (inventory.ContainsKey(item))
        {
            inventory[item] += 1;
        }
        else
        {
            inventory.Add(item, 1);
        }
    }

    public void removeFromInventory(Item item)
    {
        if (inventory.ContainsKey(item))
        {
            if (inventory[item] <= 1)
            {
                inventory.Remove(item);
            }
            else
            {
                inventory[item] -= 1;
            }
        }
        else
        {
            //do nothing, the item isn't there
        }
    }

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
	
	public enum PlayerAttributes    //WIP - body inventory equipping purposes
	{
		Strength,
		Dexterity,
		Constitution,
		Intelligence,
		Wisdom,
		Charisma
		
	}
	
	#endregion
	
	#region ANIMATION

	private Animator anim;							// a reference to the animator on the character
	private AnimatorStateInfo currentBaseState;			// a reference to the current state of the animator, used for base layer
	private AnimatorStateInfo layer2CurrentState;	// a reference to the current state of the animator, used for layer 2
	
	static int idleState = Animator.StringToHash("Base Layer.Idle");	
	static int locoState = Animator.StringToHash("Base Layer.Locomotion");			// these integers are references to our animator's states
	
	Vector3 currentGridLoc;
	Vector3 currentGridCenterPointWithoffset;
	#endregion
	
	// Use this for initialization
	void Start () 
	{
		DecidePlayerTitle();
		DecidePlayerInventoryMaxWeight();
		DecideHungerLevel();
		anim = playerAvatar.GetComponent<Animator>() as Animator;
		
	
		//Example of LevelLayout Surrounding() call:
		//if grid type is wall:
		//if (GridType.Wall == LevelManager.array[x,y])
		//{
		//	foo;
		//}
		
		
		//Placing our hero object at designated start location:
		playerAvatar.transform.position = avatarStartLocation;
		UpdateCurrentTileVectors();
	}
	
	// Update is called once per frame
	void Update ()
	{
		
		Vector3 lookVectorToOccupiedTile = currentOccupiedGridCenterWorldPoint - playerAvatar.transform.position;  //relocate this into InputMgr
		//Debug.Log("Player current (rounded) grid loc: " + playerGridCoordinate);
		Debug.DrawLine (playerAvatar.transform.position + Vector3.up, currentOccupiedGridCenterWorldPoint,Color.green);
		
		
		//If distance is greater than 1.3 (var), pass turn
		if (lookVectorToOccupiedTile.sqrMagnitude > tileMovementTolerance)  //saving overhead for Vec3.Distance()
		{
			
			UpdateCurrentTileVectors();
			//Wrap this up into pass turn
			BigBoss.TimeKeeper.numTilesCrossed++;
			BigBoss.Gooey.UpdateTilesCrossedLabel();
			AdjustXP(10);
			AdjustPlayerHealth(1);
			
			AdjustHungerPoints(-20);
			
		}

		//Moving toward closest center point if player isn't moving with input:
//		if (BigBoss.PlayerInput.isMovementKeyPressed == false && Input.GetMouseButton(1) == false)  //i.e. no player driven movement
//		{
//			//"Drift our player towards tile center point:
//			playerAvatar.transform.Translate(lookVectorToOccupiedTile.normalized*Time.deltaTime);
//		}
		
		Debug.DrawRay(currentOccupiedGridCenterWorldPoint,Vector3.up,Color.yellow);
	}

	void UpdateCurrentTileVectors ()
	{
		playerGridCoordinate = new Vector2(Mathf.Round(playerAvatar.transform.position.x),Mathf.Round(playerAvatar.transform.position.z));
		currentOccupiedGridCenterWorldPoint = new Vector3(playerGridCoordinate.x,-.5f,playerGridCoordinate.y);
	}
		
	void FixedUpdate ()
	{
		//float h = Input.GetAxis("Horizontal");				// setup h variable as our horizontal input axis
		//float v = Input.GetAxis("Vertical");				// setup v variables as our vertical input axis
		float v = 0;
		if (Input.GetMouseButton(1))
		{
			v = 1;
		}
		//Debug.Log("V: " + v);
		anim.SetFloat("Speed", v);							// set our animator's float parameter 'Speed' equal to the vertical input axis				
		//anim.SetFloat("Direction", h); 						// set our animator's float parameter 'Direction' equal to the horizontal input axis		
		//anim.speed = animSpeed;								// set the speed of our animator to the public variable 'animSpeed'
		currentBaseState = anim.GetCurrentAnimatorStateInfo(0);	// set our currentState variable to the current state of the Base Layer (0) of animation
		
//		if(anim.layerCount ==2)		
//			layer2CurrentState = anim.GetCurrentAnimatorStateInfo(1);	// set our layer2CurrentState variable to the current state of the second Layer (1) of animation
		
	}

	//THESE DIRECTIONAL FUNCTIONS FLAGGED FOR DELETION
//	public void PlayerMoveForward()   
//	{
//	
//		playerAvatar.transform.Translate(Vector3.forward*playerSpeed*Time.deltaTime );
//		
//	}
//	public void PlayerMoveBackward()
//	{
//	
//		playerAvatar.transform.Translate(Vector3.back*playerSpeed*Time.deltaTime );
//		
//	}
//	public void PlayerMoveRight()
//	{
//	
//		playerAvatar.transform.Translate(Vector3.forward*(playerSpeed*.8f)*Time.deltaTime );
//		playerAvatar.transform.Rotate(Vector3.up * PlayerSpeed*25f * Time.deltaTime,Space.Self);
//		
//	}
//	public void PlayerMoveLeft()
//	{
//	
//		playerAvatar.transform.Translate(Vector3.forward*(playerSpeed*.8f)*Time.deltaTime );
//		playerAvatar.transform.Rotate((Vector3.up * PlayerSpeed*-25f * Time.deltaTime),Space.Self);		
//	}
	
	public void MovePlayer(Vector3 heading)//comes from input manager and phases out the WASD movement
	{
		
		//THE INCOMING HEADING VECTOR3 DOES NOT HAVE TO BE PRENORMALIZED TO BE PASSED IN - MAKE SURE TO NORMALIZE ANY HEADING CALC'S IN THE TRANS FUNCTION
		//Translation toward a precalculated heading:
		playerAvatar.transform.Translate(Vector3.forward * playerSpeed * Time.deltaTime,Space.Self);
		//Lerping rotation so we don't get jitter:
		Quaternion toRot = Quaternion.LookRotation(heading);//does this need to be normalized?
 		playerAvatar.transform.rotation = Quaternion.Slerp(playerAvatar.transform.rotation, toRot, playerRotationSpeed); 
		
	}
	
	
	
	#region ALL THE DECISION MAKING/UPDATING/COMMUNICATING TO OTHER SCRIPTS.  ALSO CONTAINED IN ANY ADJUST() CALLS.
	private void DecideHungerLevel()//depending on other's needs I can 
	{
		
		//Saving initial hunger for later use:
		HungerLevel oldHL = CurrentHungerLevel;
		//For coloring the text:
		Color col = Color.white;//initializing to avoid if error
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
			col = Color.yellow;
			BigBoss.Gooey.UpdateHungerText(col);
		}
		else if (PlayerHunger < 500)
		{
			CurrentHungerLevel = HungerLevel.Hungry;
			col = Color.yellow;
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
		
		if (oldHL != CurrentHungerLevel)
		{
			SendHungerLevelChangedEvent(col);	
		}
	}

	public HungerLevel SendHungerLevelChangedEvent(Color guiCol)
	{
		//Do we want a singleton registry to send out this event to othger managers?
		
		Debug.Log("Event thrown for hunger level changing: New level is: " + CurrentHungerLevel.ToString());
		BigBoss.Gooey.CreateTextPop(playerAvatar.gameObject.transform.position + Vector3.up*.75f,CurrentHungerLevel .ToString() + "!",guiCol);
		return CurrentHungerLevel;
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
		
		playerInventoryWeightMax = (25 * (playerStrength + playerConstitution) + 50);  //rework this function since we're not using level cap
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
			
			
		}
		
		
		string finalTitle = playerChosenName + ", " + playerTitle;// + " of " + playerTitleCombatArea;
		return finalTitle;
	}  //I'll come back and rework this so it's not retarded
	#endregion
	
	
	#region Adjust Player Stats/Attr's/Data
	public int AdjustHungerPoints(int amount) 
	{
	
		playerHunger += amount;
		//Clamp to max for safety:  (in property set?)
		
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
	public void AdjustPlayerHealth(int amount)//if this is our only health function, consider adding a sender component
	{
	
		Debug.Log("IncreasePlayerHealth() called... Player's health to be adjusted by a raw amount of " + amount);
		//Can't raise health over maximum:
		int difference = PlayerMaxHealth - PlayerCurrentHealth;
		amount = (int)Mathf.Clamp(amount,0f,(float)difference);
		playerCurrentHealth += (int)amount; 
		BigBoss.Gooey.UpdateHealthBar();
		
		Debug.Log("IncreasePlayerHealth() successfully completed - " + PlayerCurrentHealth + " is current health.");
	}
	
	public void AdjustPlayerMaxHealth(int amount)//if this is our only health function, consider adding a sender component
	{
	
		Debug.Log("IncreasePlayerMaxHealth() called.  Player's max health to be adjusted by " + amount);
		playerMaxHealth += amount;  //come back and install logic/failsafes
		Debug.Log("IncreasePlayerMaxHealth() successfully completed - " + PlayerCurrentHealth + " is current health.");
	}
	
	public void AdjustXP(int amount)
	{
	
		Debug.Log("IncreasePlayerMaxHealth() called.  Player's max health to be adjusted by " + amount);
		playerCurrentXPForThisLevel += amount;  //come back and install logic/failsafes
		BigBoss.Gooey.UpdateXPBar();
		Debug.Log("IncreasePlayerMaxHealth() successfully completed - " + PlayerCurrentHealth + " is current health.");
	}

	#endregion
	
	#region SETFUCTIONS   //THESE SHOULD BE USED IN GAME ONLY FOR VERY SPECIFIC SITUATIONS AND/OR EVENTS
	
	public int SetHealth(int newHealthAmount)
	{
		
		Debug.Log("SetHealth() called.  Target health should be " + newHealthAmount);
		
		playerCurrentHealth = newHealthAmount; 
		BigBoss.Gooey.UpdateHealthBar();
		Debug.Log("SetHealth() successfully completed - " + PlayerCurrentHealth + " is current health.");
		return playerCurrentHealth;
	}
	
	public void SetAttribute(PlayerAttributes attr, int newValue)
	{
				
		switch (attr) 
		{
//			case PlayerAttributes.Charisma:
//			{
//				
//			return 
//				break;
//			}
			case PlayerAttributes.Constitution:
			{
				playerConstitution = newValue;
				break;
			}
			case PlayerAttributes.Dexterity:
			{
				playerDexterity  = newValue;
				break;
			}
			case PlayerAttributes.Intelligence:
			{
				playerIntelligence = newValue;
				break;
			}
			case PlayerAttributes.Strength:
			{
				playerStrength  = newValue;
				break;
			}
			case PlayerAttributes.Wisdom:
			{
				playerWisdom  = newValue;
				break;
			}
		
		
		default:
		break;
		}
		//update gui:
		
	}
	
	#endregion
	
	
	#region MECANIM EXAMPLE SCRIPT
//	using UnityEngine;
//using System.Collections;
//
//// Require these components when using this script
//[RequireComponent(typeof (Animator))]
//[RequireComponent(typeof (CapsuleCollider))]
//[RequireComponent(typeof (Rigidbody))]
//public class BotControlScript : MonoBehaviour
//{
//	[System.NonSerialized]					
//	public float lookWeight;					// the amount to transition when using head look
//	
//	[System.NonSerialized]
//	public Transform enemy;						// a transform to Lerp the camera to during head look
//	
//	public float animSpeed = 1.5f;				// a public setting for overall animator animation speed
//	public float lookSmoother = 3f;				// a smoothing setting for camera motion
//	public bool useCurves;						// a setting for teaching purposes to show use of curves
//
//	
//	private Animator anim;							// a reference to the animator on the character
//	private AnimatorStateInfo currentBaseState;			// a reference to the current state of the animator, used for base layer
//	private AnimatorStateInfo layer2CurrentState;	// a reference to the current state of the animator, used for layer 2
//	private CapsuleCollider col;					// a reference to the capsule collider of the character
//	
//
//	static int idleState = Animator.StringToHash("Base Layer.Idle");	
//	static int locoState = Animator.StringToHash("Base Layer.Locomotion");			// these integers are references to our animator's states
//	static int jumpState = Animator.StringToHash("Base Layer.Jump");				// and are used to check state for various actions to occur
//	static int jumpDownState = Animator.StringToHash("Base Layer.JumpDown");		// within our FixedUpdate() function below
//	static int fallState = Animator.StringToHash("Base Layer.Fall");
//	static int rollState = Animator.StringToHash("Base Layer.Roll");
//	static int waveState = Animator.StringToHash("Layer2.Wave");
//	
//
//	void Start ()
//	{
//		// initialising reference variables
//		anim = GetComponent<Animator>();					  
//		col = GetComponent<CapsuleCollider>();				
//		enemy = GameObject.Find("Enemy").transform;	
//		if(anim.layerCount ==2)
//			anim.SetLayerWeight(1, 1);
//	}
//	
//	
//	void FixedUpdate ()
//	{
//		float h = Input.GetAxis("Horizontal");				// setup h variable as our horizontal input axis
//		float v = Input.GetAxis("Vertical");				// setup v variables as our vertical input axis
//		anim.SetFloat("Speed", v);							// set our animator's float parameter 'Speed' equal to the vertical input axis				
//		anim.SetFloat("Direction", h); 						// set our animator's float parameter 'Direction' equal to the horizontal input axis		
//		anim.speed = animSpeed;								// set the speed of our animator to the public variable 'animSpeed'
//		anim.SetLookAtWeight(lookWeight);					// set the Look At Weight - amount to use look at IK vs using the head's animation
//		currentBaseState = anim.GetCurrentAnimatorStateInfo(0);	// set our currentState variable to the current state of the Base Layer (0) of animation
//		
//		if(anim.layerCount ==2)		
//			layer2CurrentState = anim.GetCurrentAnimatorStateInfo(1);	// set our layer2CurrentState variable to the current state of the second Layer (1) of animation
//		
//		

//		// IDLE
//		
//		// check if we are at idle, if so, let us Wave!
//		else if (currentBaseState.nameHash == idleState)
//		{
//			if(Input.GetButtonUp("Jump"))
//			{
//				anim.SetBool("Wave", true);
//			}
//		}
//		// if we enter the waving state, reset the bool to let us wave again in future
//		if(layer2CurrentState.nameHash == waveState)
//		{
//			anim.SetBool("Wave", false);
//		}
//	}
//}

	#endregion
}
