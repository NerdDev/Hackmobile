using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/*   
 * As long as this isn't an MMO, this Player class should be able to hold most if not all of player information.
 */
public class Player : NPC
{
    // Disable unassigned warnings
#pragma warning disable 414, 219
    #region General Player Info:
    private string playerTitle;//student, apprentice, grunt, practitioner, etc. etc.
    public string PlayerTitle { get { return playerTitle; } set { playerTitle = value; } }
    public PlayerProfessions PlayerChosenProfession;
    //Stored in the stats class
    //public int level = 10; // Just for testing.  'Prolly change this to 1 later.
    public int Level { get { return Stats.Level; } }
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
        base.addToInventory(item, count);
        BigBoss.Gooey.RegenInventoryGUI();
    }

    public override void removeFromInventory(Item item, int count)
    {
        base.removeFromInventory(item, count);
        BigBoss.Gooey.RegenInventoryGUI();
    }
    #endregion

    #region Physics
    int lastCollisionTime = 0; //unused atm

    void OnCollisionEnter(Collision collision)
    {
        //yes, it's the gspot
        GridSpace gspot = BigBoss.Levels.Level[collision.gameObject.transform.position.x.ToInt(),
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

    public override void OnClick()
    {
        BigBoss.Gooey.displayGUI = !BigBoss.Gooey.displayGUI;
        BigBoss.Gooey.RegenInventoryGUI();
    }
    #endregion

    public override void Init()
    {
        PlayerStats.Load(this, Role.ARCHAELOGIST, Race.HUMAN);
        calcStats();
    }

    // Update is called once per frame
    public override void Update()
    {
        movement();
        if (verticalMoving)
        {
            verticalMovement();
        }
    }

    #region Actions

    public override void attack(NPC n)
    {
        base.attack(n);
        BigBoss.Time.PassTurn(60);
    }

    public override void eatItem(Item i)
    {
        base.eatItem(i);
        CreateTextPop(this.Name + " eats the " + i.Name + ".");
        BigBoss.Time.PassTurn(60);
    }

    public override void useItem(Item i)
    {
        base.useItem(i);
        CreateTextPop(this.Name + " uses the " + i.Name + ".");
        BigBoss.Time.PassTurn(60);
    }

    public override bool equipItem(Item i)
    {
        if (base.equipItem(i))
        {
            CreateTextPop("Item " + i.Name + " equipped.");
            return true;
        }
        CreateTextPop("Item not equipped.");
        return false;
    }

    public override bool unequipItem(Item i)
    {
        if (base.unequipItem(i))
        {
            CreateTextPop("Item " + i.Name + " uneqipped.");
            return true;
        }
        return false;
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

    public float tileMovementTolerance = .85f;  //radius

    float timePassed = 0;
    float timeVar = 1.5f;
    bool timeSet;
    bool isMoving;
    #endregion

    private void movement()
    {
        Vector3 lookVectorToOccupiedTile = CurrentOccupiedGridCenterWorldPoint - GO.transform.position;
        Debug.DrawLine(GO.transform.position + Vector3.up, CurrentOccupiedGridCenterWorldPoint, Color.green);

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
        if (!BigBoss.PlayerInput.mouseMovement && !BigBoss.PlayerInput.touchMovement)
        {
            if (!timeSet)
            {
                timePassed = UnityEngine.Time.time + timeVar;
                timeSet = true;
            }
            if (UnityEngine.Time.time > timePassed)
            {
                if (!checkPosition(GO.transform.position, CurrentOccupiedGridCenterWorldPoint))
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
        GO.transform.position = CurrentOccupiedGridCenterWorldPoint;
    }

    public void MovePlayer(Vector3 heading)
    {
        MovePlayer(heading, playerSpeed, playerRotationSpeed);
    }

    protected override bool UpdateCurrentTileVectors()
    {
        GridCoordinate = new Vector2(GO.transform.position.x.Round(), GO.transform.position.z.Round());
        GridSpace newGridSpace = BigBoss.Levels.Level[GridCoordinate.x.ToInt(), GridCoordinate.y.ToInt()];
        if (!newGridSpace.IsBlocked())
        {
            if (GridSpace != null && GridSpace != null)
            {
                GridSpace.Remove(this);
            }
            newGridSpace.Put(this);
            CurrentOccupiedGridCenterWorldPoint = new Vector3(GridCoordinate.x, -.5f + verticalOffset, GridCoordinate.y);
            GridSpace = newGridSpace;
            BigBoss.Gooey.CheckChestDistance();
            return true;
        }
        else
        {
            return false;
        }
    }

    private void MovePlayer(Vector3 heading, float playerSpeed, float playerRotationSpeed)
    {
        GO.transform.Translate(Vector3.forward * playerSpeed * Time.deltaTime, Space.Self);
        Quaternion toRot = Quaternion.LookRotation(heading);
        GO.transform.rotation = Quaternion.Slerp(GO.transform.rotation, toRot, playerRotationSpeed);
    }

    //BRAD WHAT DOES THIS DO?!
    float v;
    public override void FixedUpdate()
    {
        if (anim == null)
        {
            try
            {
                anim = GO.GetComponent<Animator>() as Animator;
            }
            catch (Exception)
            {
            }
            return;
        }
        if (isMoving)
        {
            if (v < playerSpeed)
            {
                v += .01f;
            }
            else
            {
                v = playerSpeed;
            }
        }
        else
        {
            v = 0;
        }
        anim.SetFloat("runSpeed", v);							// set our animator's float parameter 'Speed' equal to the vertical input axis
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
        playerTitle = BigBoss.Objects.PlayerProfessions.getTitle(PlayerProfessions.Archaeologist, this.Stats.Level);
        string finalTitle = Name + ", " + playerTitle;// + " of " + playerTitleCombatArea;
        return finalTitle;
    }

    public override float AdjustHunger(float amount)
    {
        base.AdjustHunger(amount);

        //Update GUI here

        return Stats.Hunger;
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

    #region Time Management

    public override void UpdateTurn()
    {
        base.UpdateTurn();
        BigBoss.Time.numTilesCrossed++;
        BigBoss.Gooey.UpdateTilesCrossedLabel();
    }

    #endregion

#pragma warning restore 414, 219
}
