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

    public bool Initialized { get; set; }
    #region Safety Check Booleans
    public bool allowPlayerInput;
    public bool allowKeyboardInput;
    public bool allowMouseInput;
    public bool allowTouchInput;
    internal bool isMovementKeyPressed;//mainly for debugging, will convert this bool when the input class is implemented for mobile

    public bool defaultPlayerInput;
    public bool spellInput;
    public bool logInput;
    #endregion

    #region Mouse variables:
    public float horizontalMouseSensitivity;
    public float horizontalMouseAxis;//Number spit out by Unity's Input.GetAxis
    public float verticalMouseSensitivity;
    public float verticalMouseAxis;//Number spit out by Unity's Input.GetAxis
    #endregion

    //Screen/Cam space vars:
    internal Vector2 centerPointInScreenSpace;

    public void Initialize()
    {
    }

    void Start()
    {
        centerPointInScreenSpace = new Vector2(Screen.width / 2, Screen.height / 2);
        EasyTouch.On_DoubleTap += EasyTouch_On_DoubleTap;
#if !UNITY_EDITOR
        allowMouseInput = false;
#endif
    }

    #region Touch Input

    public EasyJoystick joystickLeft;
    public EasyJoystick joystickRight;

    #endregion

    void Update()
    {
        if (allowPlayerInput)
        {
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

    public JoystickCamera Rotation_Camera;
    internal bool touchMovement;

    public void CheckForTouchInput()
    {
        if (EasyTouch.GetTouchCount() >= 5)
        {
            Application.LoadLevel(Application.loadedLevelName);
            BigBoss.Starter.Start();
        }
        if (joystickLeft.JoystickAxis.x != 0 || joystickLeft.JoystickAxis.y != 0)
        {
            touchMove();
        }
        if (joystickRight.JoystickValue.x != 0)
        {
            Rotation_Camera.Rotate(joystickRight.JoystickAxis.x, 0f);
        }
        if (Math.Abs(joystickRight.JoystickValue.y) > 0.4)
        {
            Rotation_Camera.zoom(joystickRight.JoystickAxis.y);
        }
    }

    void EasyTouch_On_DoubleTap(Gesture gesture)
    {
        Rotation_Camera.Reset();
    }

    public void touchMove()
    {
        Vector3 tar = new Vector3(joystickLeft.JoystickAxis.x, 0, joystickLeft.JoystickAxis.y); //making a fake "target" vec3 using vec2 inputs - y and z swaps intended
        Quaternion lookRotFinal = Quaternion.LookRotation(tar); //calc'ing our look vector
        Vector3 euler = lookRotFinal.eulerAngles;
        euler = new Vector3(euler.x, euler.y + Rotation_Camera.xDeg, euler.z);
        BigBoss.PlayerInfo.transform.rotation = Quaternion.Euler(euler);
        BigBoss.Player.MovePlayer(joystickLeft.JoystickAxis);
    }

    #endregion

    #region KEYBOARD

    void CheckForKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            BigBoss.Time.TogglePause();
        }
    }

    #endregion

    #region MOUSE

    public bool mouseMovement;

    void CheckForMouseInput()
    {
        horizontalMouseAxis = Input.GetAxis("Mouse X");
        verticalMouseAxis = Input.GetAxis("Mouse Y");

        if (mouseMovement) //hold right click
        {
            isMovementKeyPressed = true;
            centerPointInScreenSpace = new Vector2(Screen.width / 2, Screen.height / 2);
            Vector2 centerScreenPointToMousePosLookVector = (Vector2)Input.mousePosition - centerPointInScreenSpace;
            Vector3 playerConvertedTranslationVector = new Vector3(centerScreenPointToMousePosLookVector.x, 0, centerScreenPointToMousePosLookVector.y);
            Debug.DrawRay(BigBoss.PlayerInfo.transform.position,
                playerConvertedTranslationVector,
                Color.magenta);
            BigBoss.Player.MovePlayer(new Vector2(1f, 0f));
            Quaternion lookRotFinal = Quaternion.LookRotation(playerConvertedTranslationVector); //calc'ing our look vector
            BigBoss.PlayerInfo.transform.rotation = lookRotFinal;
            BigBoss.PlayerInfo.transform.Rotate(Vector3.up, Rotation_Camera.xDeg, Space.Self);
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