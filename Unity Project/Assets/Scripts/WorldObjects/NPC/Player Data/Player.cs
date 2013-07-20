using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/*   
 * As long as this isn't an MMO, this Player class should be able to hold most if not all of player information.
 */
public class Player : NPC
{

    #region General Player Info:

    private string playerChosenName = "Kurtis";
    public string PlayerChosenName { get { return playerChosenName; } }  //read only - set at char creation

    private string playerTitle;//student, apprentice, grunt, practitioner, etc. etc.
    public string PlayerTitle { get { return playerTitle; } set { playerTitle = value; } }

    public GameObject playerAvatar;
    public GameObject PlayerAvatar { get { return playerAvatar; } }//read only global reference to the hero gameobject

    public PlayerProfessions PlayerChosenProfession;

    #endregion

    #region TRANSFORMS AND GRID DATA

    public Vector3 avatarStartLocation;
    public float tileMovementTolerance = .85f;  //radius

    #endregion

    #region Player Stats (For all the Player-only statistics)
    public float playerSpeed = 1.5f;  //temporarily hard-coded
    public float PlayerSpeed { get { return playerSpeed; } }

    public float playerRotationSpeed = .15f;  //temporarily hard-coded
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

    //static int idleState = Animator.StringToHash("Base Layer.Idle");
    //static int locoState = Animator.StringToHash("Base Layer.Locomotion");			// these integers are references to our animator's states

    Vector3 currentGridLoc;
    Vector3 currentGridCenterPointWithoffset;

    #endregion

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision!");
        if (collision.gameObject.GetComponent<NPC>() != null)
        {
            Debug.Log("You walked into an NPC!");
            Debug.Log("Attacking!");
            attack(collision.gameObject.GetComponent<NPC>());
        }
        else
        {
            GridType g = LevelManager.Array[Convert.ToInt32(collision.gameObject.transform.position.x), Convert.ToInt32(collision.gameObject.transform.position.z)];
            switch (g)
            {
                case GridType.Wall:
                    Debug.Log("You walked into a wall!");
                    break;
                case GridType.Door:
                    Debug.Log("You walked through the door!");
                    break;
                case GridType.Floor:
                    Debug.Log("You ended up in the floor. Don't ask how.");
                    break;
                default:
                    Debug.Log("I'm not sure what you collided with.");
                    break;
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        //use the internal assignation reference for clarity
        this.playerAvatar = this.gameObject;
        this.setData(BigBoss.WorldObjectManager.getNPC("player"));
        stats.Hunger = 900;
        IsActive = true;
        calcStats();

        anim = playerAvatar.GetComponent<Animator>() as Animator;

        //Example of LevelLayout Surrounding() call:
        //if grid type is wall:
        //if (GridType.Wall == LevelManager.array[x,y])
        //{
        //	foo;
        //}

        //test scene

        GameObject beholder = Instantiate(BigBoss.Prefabs.Beholder, new Vector3(22f, -.5f, 35f), Quaternion.identity) as GameObject;
        NPC beholderNPC = beholder.GetComponent<NPC>();
        beholderNPC.setData(BigBoss.WorldObjectManager.getNPC("beholder"));
        beholderNPC.IsActive = true;

        GameObject orc = Instantiate(BigBoss.Prefabs.Orc, new Vector3(12f, -.5f, 44f), Quaternion.identity) as GameObject;
        NPC orcNPC = orc.GetComponent<NPC>();
        orcNPC.setData(BigBoss.WorldObjectManager.getNPC("orc"));
        orcNPC.IsActive = true;

        GameObject skeleMage = Instantiate(BigBoss.Prefabs.SkeletonMage, new Vector3(22f, -.5f, 42f), Quaternion.identity) as GameObject;
        NPC skeleMageNPC = skeleMage.GetComponent<NPC>();
        skeleMageNPC.setData(BigBoss.WorldObjectManager.getNPC("skeleton"));
        skeleMageNPC.IsActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        movement();
    }

    #region Combat



    #endregion

    #region Movement

    float timePassed = 0;
    float timeVar = 1.5f;
    bool timeSet;
    bool isMoving;

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
            if (!timeSet)
            {
                timePassed = UnityEngine.Time.time + timeVar;
                timeSet = true;
            }
            if (UnityEngine.Time.time > timePassed)
            {
                if (checkPosition(playerAvatar.transform.position, CurrentOccupiedGridCenterWorldPoint))
                {
                    //VECTORS ARE RETARDED

                    //THESE VECTORS WOULD GO AWAY FROM THE PLAYER

                    //WHY

                    //WHY OH WHY

                    //GRID POINT - POSITION SHOULD NOT SEND THE PLAYER AWAY

                    //sigh

                    //vectorToGrid = currentOccupiedGridCenterWorldPoint - playerAvatar.transform.position;
                    //vectorToGrid = Vector3.Lerp(playerAvatar.transform.position, currentOccupiedGridCenterWorldPoint, 1f);
                    //playerAvatar.transform.Translate(vectorToGrid.normalized * Time.deltaTime);

                    //so I just did this. and it rotates the player. and it's annoying.
                    MovePlayer(lookVectorToOccupiedTile.normalized * 2 * Time.deltaTime, .75f, .25f);
                    isMoving = true;
                }
                else
                {
                    resetPosition();
                    isMoving = false;
                }
            }
            else
            {
                isMoving = false;
            }
        }
        else
        {
            isMoving = true;
            timeSet = false;
        }
    }

    private void resetPosition()
    {
        this.playerAvatar.transform.position = CurrentOccupiedGridCenterWorldPoint;
    }

    public void MovePlayer(Vector3 heading)
    {
        MovePlayer(heading, playerSpeed, playerRotationSpeed);
    }

    private void MovePlayer(Vector3 heading, float playerSpeed, float playerRotationSpeed)
    {
        //THE INCOMING HEADING VECTOR3 DOES NOT HAVE TO BE PRENORMALIZED TO BE PASSED IN - MAKE SURE TO NORMALIZE ANY HEADING CALC'S IN THE TRANS FUNCTION
        //Translation toward a precalculated heading:
        gameObject.transform.Translate(Vector3.forward * playerSpeed * Time.deltaTime, Space.Self);
        //Lerping rotation so we don't get jitter:
        Quaternion toRot = Quaternion.LookRotation(heading);//does this need to be normalized?
        playerAvatar.transform.rotation = Quaternion.Slerp(playerAvatar.transform.rotation, toRot, playerRotationSpeed);
    }

    private void applyTileEffect()
    {
        GridType grid = LevelManager.Array[Convert.ToInt32(GridCoordinate.x), Convert.ToInt32(GridCoordinate.y)];
        switch (grid)
        {
            case GridType.Wall:
                applyEffect(Properties.POISONED, 3, false);
                break;
            default:
                break;
        }
    }

    private float playervariance = .08f;
    private bool checkPosition(Vector3 playPos, Vector3 curPos)
    {
        if (Math.Abs(playPos.x - curPos.x) > playervariance ||
            Math.Abs(playPos.z - curPos.z) > playervariance)
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
        //if (Input.GetMouseButton(1))
        if (isMoving)
        {
            v = playerSpeed;
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

    public override void applyEffect(Properties e, int priority, bool isItem, int turnsToProcess)
    {
        base.applyEffect(e, priority, isItem, turnsToProcess);
        //BigBoss.Gooey.CreateTextPop(playerAvatar.transform.position, e.ToString(), Color.green);
        //update gui
    }

    #endregion

    #region Adjust Player Stats/Attr's/Data

    public override float AdjustHunger(float amount)
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
