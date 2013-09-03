using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/*   
 * As long as this isn't an MMO, this Player class should be able to hold most if not all of player information.
 */
public class Player : NPC, IManager
{
    #region General Player Info:

    private string playerChosenName = "Andrew";
    public string PlayerChosenName { get { return playerChosenName; } }  //read only - set at char creation

    private string playerTitle;//student, apprentice, grunt, practitioner, etc. etc.
    public string PlayerTitle { get { return playerTitle; } set { playerTitle = value; } }

    public GameObject playerAvatar;
    public GameObject PlayerAvatar { get { return playerAvatar; } }//read only global reference to the hero gameobject

    public PlayerProfessions PlayerChosenProfession;

    #endregion

    #region Player Stats (For all the Player-only statistics)
    public float playerSpeed = 1.5f;  //temporarily hard-coded
    public float PlayerSpeed { get { return playerSpeed; } }

    public float playerRotationSpeed = .15f;  //temporarily hard-coded
    public float PlayerRotationSpeed { get { return playerRotationSpeed; } }
    #endregion

    #region INVENTORY
    public override void addToInventory(Item item, int count)
    {
        if (inventory.ContainsKey(item))
        {
            inventory[item] += count;
            //GUI Stuff:
			
        }
        else
        {
            inventory.Add(item, count);
            //GUI Stuff:
			BigBoss.Gooey.AddItemToGUIInventory(item,count);
        }
        stats.Encumbrance += item.props.Weight * count;
    }
    #endregion

    #region Physics
    int lastCollisionTime = 0; //unused atm
    void OnCollisionEnter(Collision collision)
    {
        //yes, it's the gspot
        GridSpace gspot = LevelManager.Level[collision.gameObject.transform.position.x.ToInt(),
            collision.gameObject.transform.position.z.ToInt()];
        //I KNEW IT EXISTED
        GridType g = gspot.Type;
        switch (g)
        {
            case GridType.Wall:
                //Debug.Log("You walked into a wall!");
                break;
            case GridType.Door:
                //Debug.Log("You walked through the door!");
                break;
            case GridType.Floor:
                //Debug.Log("You ended up in the floor. Don't ask how.");
                break;
            default:
                //Debug.Log("I'm not sure what you collided with.");
                break;
        }
    }
    #endregion

    public void Initialize()
    {
        //use the internal assignation reference for clarity
        this.playerAvatar = this.gameObject;
        this.setData(BigBoss.WorldObject.getNPC("player"));
        stats.Hunger = 900;
        IsActive = true;
        calcStats();
        this.PlayerTitle = BigBoss.Data.playerProfessions.getTitle(BigBoss.PlayerInfo.PlayerChosenProfession, BigBoss.PlayerInfo.stats.Level);
        anim = playerAvatar.GetComponent<Animator>() as Animator;
        this.Name = "Kurtis";

        GameObject item = new GameObject();
        Item i = item.AddComponent<Item>();
        Item baseI = BigBoss.WorldObject.getItem("sword1");
        i.setData(baseI);
        i.IsActive = true;
        this.addToInventory(i);
        this.equipItem(i);

        GameObject item2 = new GameObject();
        Item i2 = item2.AddComponent<Item>();
        Item food = BigBoss.WorldObject.getItem("spoiled bread");
        i2.setData(food);
        i2.IsActive = true;
        this.addToInventory(i2, 5);
    }

    // Update is called once per frame
    void Update()
    {
        movement();
    }

    #region Actions

    public override void attack(NPC n)
    {
        base.attack(n);
        BigBoss.Time.PassTurn(60);
    }

    #endregion

    #region Movement and Animation

    #region Movement/Animation Properties
    private Animator anim;							// a reference to the animator on the character
    private AnimatorStateInfo currentBaseState;			// a reference to the current state of the animator, used for base layer
    private AnimatorStateInfo layer2CurrentState;	// a reference to the current state of the animator, used for layer 2
    ///static int idleState = Animator.StringToHash("Base Layer.Idle");
    ///static int locoState = Animator.StringToHash("Base Layer.Locomotion");			// these integers are references to our animator's states
    Vector3 currentGridLoc;
    Vector3 currentGridCenterPointWithoffset;

    public Vector3 avatarStartLocation;
    public float tileMovementTolerance = .85f;  //radius

    float timePassed = 0;
    float timeVar = 1.5f;
    bool timeSet;
    bool isMoving;
    private float playervariance = .08f;
    #endregion

    private void movement()
    {
        Vector3 lookVectorToOccupiedTile = CurrentOccupiedGridCenterWorldPoint - playerAvatar.transform.position;  //relocate this into InputMgr
        Debug.DrawLine(playerAvatar.transform.position + Vector3.up, CurrentOccupiedGridCenterWorldPoint, Color.green);

        //If distance is greater than 1.3 (var), pass turn
        if (lookVectorToOccupiedTile.sqrMagnitude > tileMovementTolerance)  //saving overhead for Vec3.Distance()
        {
            if (UpdateCurrentTileVectors())
            {
                BigBoss.Time.PassTurn(60);
            }
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
                if (!checkPosition(playerAvatar.transform.position, CurrentOccupiedGridCenterWorldPoint))
                {
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

    protected override bool UpdateCurrentTileVectors()
    {
        GridCoordinate = new Vector2(this.gameObject.transform.position.x.Round(), this.gameObject.transform.position.z.Round());
        newGridSpace = new Value2D<GridSpace>(GridCoordinate.x.ToInt(), GridCoordinate.y.ToInt());
        GridSpace newGrid = LevelManager.Level[newGridSpace.x, newGridSpace.y];
        if (!newGrid.IsBlocked())
        {
            if (gridSpace != null && gridSpace.val != null)
            {
                gridSpace.val.Remove(this);
            }
            newGrid.Put(this);
            CurrentOccupiedGridCenterWorldPoint = new Vector3(GridCoordinate.x, -.5f, GridCoordinate.y);
            newGridSpace.val = newGrid;
            gridSpace = newGridSpace;
            return true;
        }
        else
        {
            return false;
        }
    }

    private void MovePlayer(Vector3 heading, float playerSpeed, float playerRotationSpeed)
    {
        gameObject.transform.Translate(Vector3.forward * playerSpeed * Time.deltaTime, Space.Self);
        Quaternion toRot = Quaternion.LookRotation(heading);
        playerAvatar.transform.rotation = Quaternion.Slerp(playerAvatar.transform.rotation, toRot, playerRotationSpeed);
    }

    private bool checkPosition(Vector3 playPos, Vector3 curPos)
    {
        if (Math.Abs(playPos.x - curPos.x) > playervariance ||
            Math.Abs(playPos.z - curPos.z) > playervariance)
        {
            return false;
        }
        return true;
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
        anim.SetFloat("speed", v);							// set our animator's float parameter 'Speed' equal to the vertical input axis				
        //anim.SetFloat("Direction", h); 						// set our animator's float parameter 'Direction' equal to the horizontal input axis		
        //anim.speed = animSpeed;								// set the speed of our animator to the public variable 'animSpeed'
        currentBaseState = anim.GetCurrentAnimatorStateInfo(0);	// set our currentState variable to the current state of the Base Layer (0) of animation

        //		if(anim.layerCount ==2)		
        //			layer2CurrentState = anim.GetCurrentAnimatorStateInfo(1);	// set our layer2CurrentState variable to the current state of the second Layer (1) of animation

    }

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
    #endregion

    #region Stats
    private string getPlayerTitle()
    {
        //Change this to a generic chosen profession later
        playerTitle = BigBoss.Data.playerProfessions.getTitle(PlayerProfessions.Archaeologist, this.stats.Level);
        string finalTitle = playerChosenName + ", " + playerTitle;// + " of " + playerTitleCombatArea;
        return finalTitle;
    }

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

    #region Effects

    public override void applyEffect(Properties e, int priority = 1, bool isItem = false, int turnsToProcess = -1)
    {
        base.applyEffect(e, priority, isItem, turnsToProcess);
        BigBoss.Gooey.CreateTextPop(playerAvatar.transform.position, e.ToString(), Color.green);
        //update gui
    }

    #endregion

    #region Time Management

    public override void UpdateTurn()
    {
        base.UpdateTurn();
        BigBoss.Time.numTilesCrossed++;
        BigBoss.Gooey.UpdateTilesCrossedLabel();
    }

    #endregion
}
