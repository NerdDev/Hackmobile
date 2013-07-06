using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/*   As long as this isn't an MMO, this Player class should be able to hold most if not all of player information.
 
 
 */
public class PlayerManager : NPC
{

    #region General Player Info:

    private string playerChosenName;
    public string PlayerChosenName { get { return playerChosenName; } }  //read only - set at char creation

    private string playerTitle;//student, apprentice, grunt, practitioner, etc. etc.
    public string PlayerTitle { get { return playerTitle; } }  //read only - updated via class info

    public GameObject playerAvatar;
    public GameObject PlayerAvatar { get { return playerAvatar; } }//read only global reference to the hero gameobject

    public PlayerProfessions PlayerChosenProfession;

    #endregion

    #region TRANSFORMS AND GRID DATA

    public Vector3 avatarStartLocation;
    public float tileMovementTolerance = .85f;  //radius
    //X,Y coordinate for other scripts to grab:
    private Vector2 playerGridCoordinate;
    public Vector2 PlayerGridCoordinate
    {
        get { return playerGridCoordinate; }
        set { playerGridCoordinate = value; }
    }

    private Vector3 currentOccupiedGridCenterWorldPoint;
    public Vector3 CurrentOccupiedGridCenterWorldPoint 
    { 
        get { return currentOccupiedGridCenterWorldPoint; }
        set { currentOccupiedGridCenterWorldPoint = value; }
    }

    private Vector3 lastOccupiedGridCenterWorldPoint;
    public Vector3 LastOccupiedGridCenterWorldPoint
    {
        get { return lastOccupiedGridCenterWorldPoint; }
        set { lastOccupiedGridCenterWorldPoint = value; }
    }

    #endregion

    #region Player Stats (For all the Player-only statistics)
    public float playerSpeed = 10;  //temporarily hard-coded
    public float PlayerSpeed { get { return playerSpeed; } }

    public float playerRotationSpeed = .05f;  //temporarily hard-coded
    public float PlayerRotationSpeed { get { return playerRotationSpeed; } }
    #endregion

    #region INVENTORY
    //Inventory Array - Will have to confirm typing when NGUI integration is set up...
    public List<GameObject> PlayerInventory = new List<GameObject>();
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
    void Start()
    {
        playerTitle = getPlayerTitle();
        stats.MaxEncumbrance = getMaxInventoryWeight();
        stats.Hunger = 900;

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
    void Update()
    {
        movement();
    }

    #region Movement

    private void movement()
    {
        Vector3 lookVectorToOccupiedTile = CurrentOccupiedGridCenterWorldPoint - playerAvatar.transform.position;  //relocate this into InputMgr
        Debug.DrawLine(playerAvatar.transform.position + Vector3.up, CurrentOccupiedGridCenterWorldPoint, Color.green);

        //If distance is greater than 1.3 (var), pass turn
        if (lookVectorToOccupiedTile.sqrMagnitude > tileMovementTolerance)  //saving overhead for Vec3.Distance()
        {
            UpdateCurrentTileVectors();
            BigBoss.TimeKeeper.PassTurn(60);
        }

        //Moving toward closest center point if player isn't moving with input:
        resetToGrid(lookVectorToOccupiedTile);

        Debug.DrawRay(CurrentOccupiedGridCenterWorldPoint, Vector3.up, Color.yellow);
    }

    private void resetToGrid(Vector3 lookVectorToOccupiedTile)
    {
        if (BigBoss.PlayerInput.isMovementKeyPressed == false && Input.GetMouseButton(1) == false)
        {
            if (checkPosition(playerAvatar.transform.position, currentOccupiedGridCenterWorldPoint))
            {
                //"Drift our player towards tile center point:
                playerAvatar.transform.Translate(lookVectorToOccupiedTile.normalized * Time.deltaTime);
            }
            else
            {
                this.playerAvatar.transform.position = CurrentOccupiedGridCenterWorldPoint;
            }
        }
    }

    public void MovePlayer(Vector3 heading)
    {
        //THE INCOMING HEADING VECTOR3 DOES NOT HAVE TO BE PRENORMALIZED TO BE PASSED IN - MAKE SURE TO NORMALIZE ANY HEADING CALC'S IN THE TRANS FUNCTION
        //Translation toward a precalculated heading:
        playerAvatar.transform.Translate(Vector3.forward * playerSpeed * Time.deltaTime, Space.Self);
        //Lerping rotation so we don't get jitter:
        Quaternion toRot = Quaternion.LookRotation(heading);//does this need to be normalized?
        playerAvatar.transform.rotation = Quaternion.Slerp(playerAvatar.transform.rotation, toRot, playerRotationSpeed);
    }

    void UpdateCurrentTileVectors()
    {
        PlayerGridCoordinate = new Vector2(Mathf.Round(playerAvatar.transform.position.x), Mathf.Round(playerAvatar.transform.position.z));
        LastOccupiedGridCenterWorldPoint = CurrentOccupiedGridCenterWorldPoint;
        CurrentOccupiedGridCenterWorldPoint = new Vector3(PlayerGridCoordinate.x, -.5f, PlayerGridCoordinate.y);
        GridSpace grid = LevelManager.blocks[Convert.ToInt32(PlayerGridCoordinate.x), Convert.ToInt32(PlayerGridCoordinate.y)].GetComponent<GridSpace>();
        if (grid.hasNPC())
        {
            playerAvatar.transform.position = LastOccupiedGridCenterWorldPoint;
        }
        applyTileEffect();
    }

    private void applyTileEffect()
    {
        GridType grid = LevelManager.array[Convert.ToInt32(playerGridCoordinate.x), Convert.ToInt32(playerGridCoordinate.y)];
        Debug.Log(grid);
        switch (grid)
        {
            case GridType.Wall:
                applyEffect(Properties.POISONED, 3, false);
                break;
            default:
                break;
        }
    }

    private float variance = .08f;
    private bool checkPosition(Vector3 playPos, Vector3 curPos)
    {
        if (Math.Abs(playPos.x - curPos.x) > variance ||
            Math.Abs(playPos.z - curPos.z) > variance)
        {
            return true;
        }
        return false;
    }

    //BRAD WHAT DOES THIS DO?!
    void FixedUpdate()
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
    #endregion

    #region Calculation Methods (Hunger, Stats, etc)

    private string getPlayerTitle()
    {
        //Change this to a generic chosen profession later
        playerTitle = BigBoss.DataManager.playerProfessions.getTitle(PlayerProfessions.Archaeologist, this.stats.Level);
        string finalTitle = playerChosenName + ", " + playerTitle;// + " of " + playerTitleCombatArea;
        return finalTitle;
    }

    #endregion

    #region Effects

    public override void applyEffect(Properties e, int priority, bool isItem)
    {
        base.applyEffect(e, priority, isItem);
        BigBoss.Gooey.CreateTextPop(playerAvatar.transform.position, e.ToString(), Color.green);
        //update gui
    }

    #endregion

    #region Adjust Player Stats/Attr's/Data

    public override int AdjustHunger(int amount)
    {
        base.AdjustHunger(amount);

        //Update GUI here

        return stats.Hunger;
    }

    public override void AddLevel()
    {
        base.AddLevel();

        //Update GUI here
    }

    public override void AdjustHealth(int amount)
    {
        base.AdjustHealth(amount);

        //Do all GUI updates here
        BigBoss.Gooey.UpdateHealthBar();
    }

    public override void AdjustMaxHealth(int amount)
    {
        base.AdjustMaxHealth(amount);

        //GUI updates
    }

    public override void AdjustXP(float amount)
    {
        base.AdjustXP(amount);

        //GUI updates
        BigBoss.Gooey.UpdateXPBar();
    }

    public override void AdjustAttribute(Attributes attr, int amount)
    {
        base.AdjustAttribute(attr, amount);
        //update gui:
    }

    #endregion

    #region SETFUCTIONS   //THESE SHOULD BE USED IN GAME ONLY FOR VERY SPECIFIC SITUATIONS AND/OR EVENTS

    public int SetHealth(int newHealthAmount)
    {

        Debug.Log("SetHealth() called.  Target health should be " + newHealthAmount);

        stats.CurrentHealth = newHealthAmount;
        BigBoss.Gooey.UpdateHealthBar();
        Debug.Log("SetHealth() successfully completed - " + stats.CurrentHealth + " is current health.");
        return stats.CurrentHealth;
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

    #region Time Management

    public override void UpdateTurn()
    {
        base.UpdateTurn();
        BigBoss.TimeKeeper.numTilesCrossed++;
        BigBoss.Gooey.UpdateTilesCrossedLabel();
    }

    #endregion
}
