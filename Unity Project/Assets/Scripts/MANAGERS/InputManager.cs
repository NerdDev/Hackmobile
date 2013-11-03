using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class InputManager : MonoBehaviour, IManager
{
    /* This manager class handles all player input, in addition to arbritrary related states - i.e. "Player Typing", or "Phone Upside Down".
     Booleans for key combinations can be a possibility.  If it's related to player interaction with hardware, it should probably go here.
     */

    #region Safety Check Booleans
    public bool allowPlayerInput;
    public bool allowKeyboardInput;
    public bool allowMouseInput;
    public bool isMovementKeyPressed;//mainly for debugging, will convert this bool when the input class is implemented for mobile
    #endregion

    #region Mouse variables:
    public float horizontalMouseSensitivity;
    public float horizontalMouseAxis;//Number spit out by Unity's Input.GetAxis
    public float verticalMouseSensitivity;
    public float verticalMouseAxis;//Number spit out by Unity's Input.GetAxis
    #endregion

    //Screen/Cam space vars:
    public Vector2 centerPointInScreenSpace;

    public void Initialize()
    {
    }

    void Start()
    {
        centerPointInScreenSpace = new Vector2(Screen.width / 2, Screen.height / 2);
        //Debug.Log("Camera Screen space center point calculated: " + centerPointInScreenSpace);

        //SubscribeToEasyTouchMethods();  //feel free to relocate this
    }

    #region Touch Input
    void OnEnable()
    {
        EasyTouch.On_SimpleTap += On_SimpleTap;
    }

    void OnDisable()
    {
        EasyTouch.On_SimpleTap -= On_SimpleTap;
    }

    void On_SimpleTap(Gesture gesture)
    {
        if (gesture.pickObject != null)
        {
            GameObject go = gesture.pickObject;
            if (go.layer == 12)
            {
                Point p = new Point(go.transform.position.x, go.transform.position.z);
                if (this is InputManager && BigBoss.Levels.Level[p.x, p.y].HasObject())
                {
                    List<WorldObject> list = BigBoss.Levels.Level[p.x, p.y].GetBlockingObjects();
                    NPC n = (NPC)list.Find(w => w is NPC);
                    if (n != null && n.IsNotAFreaking<Player>())
                    {
                        GridSpace playerLoc = BigBoss.Player.gridSpace;
                        IEnumerable<GridSpace> grids = BigBoss.Levels.Level.getSurroundingSpaces(p.x, p.y);
                        if (grids.Single(g => g.X == playerLoc.X && g.Y == playerLoc.Y) != null)
                        {
                            BigBoss.Player.attack(n);
                        }
                    }
                }
            }
            else if (go.layer == 13)
            {
                //stairs
                if (go.name.Equals("StairsDown"))
                {
                    BigBoss.Levels.SetCurLevel(false);
                }
                else if (go.name.Equals("StairsUp"))
                {
                    BigBoss.Levels.SetCurLevel(true);
                }
            }
        }
    }
    #endregion

    void Update()
    {

        if (allowPlayerInput)
        {
            //Toggle Debug Mode:
            if (Input.GetKeyDown(KeyCode.BackQuote))
            {
                //BigBoss.GameStateManager.ToggleDebugMode(); 
            }
            if (allowKeyboardInput)
            {
                CheckForKeyboardInput();
            }
            if (allowMouseInput)
            {
                CheckForMouseInput();
            }
        }
    }

    /*
    public void OnTouchStart(Gesture gesture)
    {
        //Debug Block:

        try
        {
            Debug.Log("Object Picked: " + gesture.pickObject.name);
            if (gesture.pickObject == (GameObject)GameObject.Find("PlayerA"))//COME BACK AND OPTIMIZE THIS CHECK - NEED A STATIC REF TO PLAYER OBJECT
            {
                BigBoss.Gooey.PlayerTouched();
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("Event sent, but no pick object: Exception: " + ex.Message);
        }
    }
    */

    #region KEYBOARD

    void CheckForKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BigBoss.Time.TogglePause();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            Item food = null;
            food = BigBoss.Player.inventory.Get("food", "spoiled bread")[0];
            if (food != null)
            {
                BigBoss.Player.eatItem(food);
            }
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log(BigBoss.Player.inventory.Dump());
            Item food = null;
            food = BigBoss.Player.inventory.Get("potion", "health potion")[0];
            if (food != null)
            {
                BigBoss.Player.eatItem(food);
            }
        }
    }

    #endregion

    #region MOUSE

    void CheckForMouseInput()
    {
        horizontalMouseAxis = Input.GetAxis("Mouse X");
        verticalMouseAxis = Input.GetAxis("Mouse Y");
        //mouseLocation = Input.mousePosition;

        if (Input.GetMouseButton(1)) //hold right click
        {
            Vector2 centerScreenPointToMousePosLookVector = (Vector2)Input.mousePosition - centerPointInScreenSpace;
            Vector3 playerConvertedTranslationVector = new Vector3(centerScreenPointToMousePosLookVector.x, 0, centerScreenPointToMousePosLookVector.y);
            Debug.DrawRay(BigBoss.PlayerInfo.transform.position,
                playerConvertedTranslationVector,
                Color.magenta);
            BigBoss.Player.MovePlayer(playerConvertedTranslationVector);

        }
        if (Input.GetMouseButtonDown(0)) //hold left click
        {

        }
    }

    #endregion
}
