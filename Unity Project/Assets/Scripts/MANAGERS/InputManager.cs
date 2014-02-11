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
        zoomCam = Camera.main;
        EasyTouch.On_DoubleTap += EasyTouch_On_DoubleTap;
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
                touchZoom();
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
        }
        if (touchMovement)
        {
            touchMove();
        }
        if (joystickRight.JoystickValue.y != 0 || joystickRight.JoystickValue.x != 0)
        {
            Rotation_Camera.Rotate(joystickRight.JoystickValue.x, joystickRight.JoystickValue.y);
        }
    }

    void EasyTouch_On_DoubleTap(Gesture gesture)
    {
        Rotation_Camera.Reset();
    }

    public void touchMove()
    {
        float forwardTransVector = Vector2.Distance(joystickLeft.JoystickAxis, Vector2.zero) * Time.deltaTime;
        Vector3 tar = new Vector3(joystickLeft.JoystickAxis.x, 0, joystickLeft.JoystickAxis.y); //making a fake "target" vec3 using vec2 inputs - y and z swaps intended
        Vector3 lookVectorPreAdjusted = tar - Vector3.zero;
        BigBoss.Player.MovePlayer(new Vector3(0, 0, forwardTransVector));
        Quaternion lookRotFinal = Quaternion.LookRotation(lookVectorPreAdjusted); //calc'ing our look vector
        BigBoss.PlayerInfo.transform.rotation = lookRotFinal;
        BigBoss.PlayerInfo.transform.Rotate(Vector3.up, Rotation_Camera.xDeg, Space.Self);
    }

    #region Zoom
    public int speed = 4;
    public Camera zoomCam;
    public float Zoom_MINSCALE = 2.0F;
    public float Zoom_MAXSCALE = 5.0F;
    public float Zoom_minPinchSpeed = 5.0F;
    public float Zoom_varianceInDistances = 5.0F;
    private float touchDelta = 0.0F;
    private Vector2 prevDist = new Vector2(0, 0);
    private Vector2 curDist = new Vector2(0, 0);
    private float speedTouch0 = 0.0F;
    private float speedTouch1 = 0.0F;

    public void touchZoom()
    {
        if (Input.touchCount == 2 && Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(1).phase == TouchPhase.Moved)
        {
            curDist = Input.GetTouch(0).position - Input.GetTouch(1).position; //current distance between finger touches
            prevDist = ((Input.GetTouch(0).position - Input.GetTouch(0).deltaPosition)
                - (Input.GetTouch(1).position - Input.GetTouch(1).deltaPosition)); //difference in previous locations using delta positions
            touchDelta = curDist.magnitude - prevDist.magnitude;
            speedTouch0 = Input.GetTouch(0).deltaPosition.magnitude / Input.GetTouch(0).deltaTime;
            speedTouch1 = Input.GetTouch(1).deltaPosition.magnitude / Input.GetTouch(1).deltaTime;

            if ((touchDelta + Zoom_varianceInDistances <= 1) && (speedTouch0 > Zoom_minPinchSpeed) && (speedTouch1 > Zoom_minPinchSpeed))
            {
                Rotation_Camera.zoom(touchDelta * speed);
                //zoomCam.fieldOfView = Mathf.Clamp(zoomCam.fieldOfView + (1 * speed), 15, 90);
            }
            if ((touchDelta + Zoom_varianceInDistances > 1) && (speedTouch0 > Zoom_minPinchSpeed) && (speedTouch1 > Zoom_minPinchSpeed))
            {
                Rotation_Camera.zoom(-touchDelta * speed);
                //zoomCam.fieldOfView = Mathf.Clamp(zoomCam.fieldOfView - (1 * speed), 15, 90);
            }
        }
    }
    #endregion

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