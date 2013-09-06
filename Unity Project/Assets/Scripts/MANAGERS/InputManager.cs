using UnityEngine;
using System.Collections;

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

        SubscribeToEasyTouchMethods();  //feel free to relocate this
    }

    public void SubscribeToEasyTouchMethods()
    {
        EasyTouch.On_TouchStart += OnTouchStart;
    }

    public void UnSubscribeToEasyTouchMethods()
    {
        EasyTouch.On_TouchStart -= OnTouchStart;
    }

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

    #region KEYBOARD

    void CheckForKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BigBoss.Time.TogglePause();
        }
        {
            Vector3 place = new Vector3(15f, .5f, 18);
            BigBoss.WorldObject.CreateRandomItem();
            Debug.Log("X");
        }
		
		

        if (Input.GetKeyDown(KeyCode.Z))
        {
            BigBoss.PlayerInfo.playerAvatar.transform.position = BigBoss.PlayerInfo.avatarStartLocation;
        }
        if (Input.GetKeyDown(KeyCode.F1))
        {
            //Item theItem = BigBoss.WorldObjectManager.CreateRandomItem(new Vector3 (0,0,0));
            //Testing out an NGUI texture swap:
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            Item food = null;
            foreach (Item i in BigBoss.PlayerInfo.inventory.Keys)
            {
                if (i.Name.Equals("spoiled bread"))
                {
                    food = i;
                    break;
                }
            }
            if (food != null)
            {
                BigBoss.PlayerInfo.eatItem(food);
            }
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Item food = null;
            foreach (Item i in BigBoss.PlayerInfo.inventory.Keys)
            {
                if (i.Name.Equals("health potion"))
                {
                    food = i;
                    break;
                }
            }
            if (food != null)
            {
                BigBoss.PlayerInfo.eatItem(food);
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
            Debug.DrawRay(BigBoss.PlayerInfo.PlayerAvatar.transform.position,
                playerConvertedTranslationVector,
                Color.magenta);
            BigBoss.PlayerInfo.MovePlayer(playerConvertedTranslationVector);

        }
        if (Input.GetMouseButtonDown(0)) //hold left click
        {

        }
    }

    #endregion
}
