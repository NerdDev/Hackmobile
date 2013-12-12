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
    public bool allowTouchInput;
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

    public EasyJoystick joystickLeft;
    public EasyJoystick joystickRight;

    void OnEnable()
    {
        //EasyTouch.On_SimpleTap += On_SimpleTap;
    }

    void OnDisable()
    {
        //EasyTouch.On_SimpleTap -= On_SimpleTap;
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
                        GridSpace playerLoc = BigBoss.Player.GridSpace;
                        Value2D<GridSpace> grid;
                        if (BigBoss.Levels.Level.Array.GetPointAround(p.x, p.y, true, (arr, x, y) =>
                            {
                                GridSpace g = arr[y, x];
                                return g.X == playerLoc.X && g.Y == playerLoc.Y;
                            }, out grid))
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
        CheckForMouseMovement();
        CheckForTouchMovement();
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
                CheckForMouseMovement();
                CheckForMouseInput();
            }
            if (allowTouchInput)
            {
                CheckForTouchMovement();
                CheckForTouchInput();
            }
        }
    }

    #region TOUCH

    public bool touchMovement;

    public void CheckForTouchInput()
    {
        if (EasyTouch.GetTouchCount() >= 5)
        {
            Application.LoadLevel(Application.loadedLevelName);
        }
        if (touchMovement)//meshes perfectly with smoothing/inertia on the joysticks
        {
            touchMove();
        }
        if (joystickRight.JoystickValue.x != 0 || joystickRight.JoystickValue.y != 0)
        {
            //MoveCamera();
            //Write camera movement script
        }
    }

    public void touchMove()
    {
        float forwardTransVector = Vector2.Distance(joystickLeft.JoystickAxis, Vector2.zero) * Time.deltaTime;
        Vector3 tar = new Vector3(joystickLeft.JoystickAxis.x, 0, joystickLeft.JoystickAxis.y); //making a fake "target" vec3 using vec2 inputs - y and z swaps intended
        Vector3 lookVectorPreAdjusted = tar - Vector3.zero; //works, but as soon as camera rotates the direction wont compensate
        BigBoss.Player.MovePlayer(new Vector3(0, 0, forwardTransVector));
        Quaternion lookRotFinal = Quaternion.LookRotation(lookVectorPreAdjusted); //calc'ing our look vector
        if (lookRotFinal != Quaternion.identity)
        {
            BigBoss.PlayerInfo.transform.localRotation = lookRotFinal;
        }
    }

    #endregion

    #region KEYBOARD

    void CheckForKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BigBoss.Time.TogglePause();
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            Item food;
            if (BigBoss.Player.Inventory.TryGetFirst("food", "spoiled bread", out food))
                BigBoss.Player.eatItem(food);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log(BigBoss.Player.Inventory.Dump());
            Item food;
            if (BigBoss.Player.Inventory.TryGetFirst("potion", "health potion", out food))
                BigBoss.Player.eatItem(food);
        }
    }

    #endregion

    #region MOUSE

    public bool mouseMovement;

    void CheckForMouseInput()
    {
        horizontalMouseAxis = Input.GetAxis("Mouse X");
        verticalMouseAxis = Input.GetAxis("Mouse Y");

        if (Input.GetMouseButton(1)) //hold right click
        {
            isMovementKeyPressed = true;
            Vector2 centerScreenPointToMousePosLookVector = (Vector2)Input.mousePosition - centerPointInScreenSpace;
            Vector3 playerConvertedTranslationVector = new Vector3(centerScreenPointToMousePosLookVector.x, 0, centerScreenPointToMousePosLookVector.y);
            Debug.DrawRay(BigBoss.PlayerInfo.transform.position,
                playerConvertedTranslationVector,
                Color.magenta);
            BigBoss.Player.MovePlayer(playerConvertedTranslationVector);
        }
    }

    #endregion

    #region Movement checks
    void CheckForTouchMovement()
    {
        if (joystickLeft.JoystickValue.x != 0 || joystickLeft.JoystickValue.y != 0)
        {
            touchMovement = true;
        }
        else
        {
            touchMovement = false;
        }
    }

    void CheckForMouseMovement()
    {
        if (Input.GetMouseButton(1))
        {
            mouseMovement = true;
        }
        else
        {
            mouseMovement = false;
        }
    }
    #endregion
}
