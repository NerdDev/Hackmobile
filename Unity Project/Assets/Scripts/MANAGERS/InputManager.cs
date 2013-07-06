using UnityEngine;
using System.Collections;

public class InputManager : MonoBehaviour {

	/* This manager class handles all player input, in addition to arbritrary related states - i.e. "Player Typing", or "Phone Upside Down".
	 Booleans for key combinations can be a possibility.  If it's related to player interaction with hardware, it should probably go here.
	 */
	
	//Safety Checks:
	public bool allowPlayerInput;
	public bool allowKeyboardInput;
	public bool allowMouseInput;
	public bool isMovementKeyPressed;//mainly for debugging, will convert this bool when the input class is implemented for mobile
	//Mouse:
	public float horizontalMouseSensitivity;
	public float horizontalMouseAxis;//Number spit out by Unity's Input.GetAxis
	public float verticalMouseSensitivity;
	public float verticalMouseAxis;//Number spit out by Unity's Input.GetAxis
	
	//Screen/Cam space vars:
	public Vector2 centerPointInScreenSpace;
	
	
	void Start()
	{
		
		centerPointInScreenSpace = new Vector2(Screen.width/2,Screen.height/2);
		Debug.Log("Camera Screen space center point calculated: " + centerPointInScreenSpace);
	}
	void Update()
	{
		
		if(allowPlayerInput)
		{
			
			//Toggle Debug Mode:
				if (Input.GetKeyDown(KeyCode.BackQuote)){BigBoss.GameStateManager.ToggleDebugMode();}
			
			if(allowKeyboardInput)
			{
				CheckForKeyboardInput();
			}
			if(allowMouseInput)
			{
				CheckForMouseInput();
			}
			
			
			
			
			
			
						
		}		
	}//end Update()
	
	void CheckForKeyboardInput ()
	{
			
		isMovementKeyPressed = false;
			//Standard Escape Menu/Pause
				if (Input.GetKeyDown(KeyCode.Escape)){BigBoss.TimeKeeper.TogglePause();}
			
				//if(Input.GetKeyDown(KeyCode.C))
		   //AudioController.PlayMusic("Plains");   //audiomanager in limbo for now
				if (Input.GetKeyDown(KeyCode.X))
		{
			Vector3 place = new Vector3(15f,.5f,18);
			BigBoss.ItemMaster.CreateRandomItem(place);
			Debug.Log("X");
		}
//		if (Input.GetKey(KeyCode.W))
//		{
//			BigBoss.PlayerInfo.PlayerMoveForward();
//			isMovementKeyPressed = true;
//		}
//		if (Input.GetKey(KeyCode.S))
//		{
//			BigBoss.PlayerInfo.PlayerMoveBackward();
//			isMovementKeyPressed = true;
//		}
//		if (Input.GetKey(KeyCode.A))
//		{
//			BigBoss.PlayerInfo.PlayerMoveLeft();
//			isMovementKeyPressed = true;
//		}
//		if (Input.GetKey(KeyCode.D))
//		{
//			
//			BigBoss.PlayerInfo.PlayerMoveRight();
//			isMovementKeyPressed = true;
//		}
		
		#region WASD
		//For GUI debugging:
//		if (Input.GetKeyDown(KeyCode.W))
//		{
//			BigBoss.Gooey.keyWSprite.color = Color.green;
//		}
//		if (Input.GetKeyDown(KeyCode.S))
//		{
//			BigBoss.Gooey.keySSprite.color = Color.green;
//		}
//		if (Input.GetKeyDown(KeyCode.A))
//		{
//			BigBoss.Gooey.keyASprite.color = Color.green;
//		}
//		if (Input.GetKeyDown(KeyCode.D))
//		{
//			BigBoss.Gooey.keyDSprite.color = Color.green;
//			
//		}
//		if (Input.GetKeyUp(KeyCode.W))
//		{
//			BigBoss.Gooey.keyWSprite.color = Color.white;
//		}
//		if (Input.GetKeyUp(KeyCode.S))
//		{
//			BigBoss.Gooey.keySSprite.color = Color.white;
//		}
//		if (Input.GetKeyUp(KeyCode.A))
//		{
//			BigBoss.Gooey.keyASprite.color = Color.white;
//		}
//		if (Input.GetKeyUp(KeyCode.D))
//		{
//			BigBoss.Gooey.keyDSprite.color = Color.white;
//			
//		}
		
		if (Input.GetKeyDown(KeyCode.Z))
		{
			BigBoss.PlayerInfo.playerAvatar.transform.position = BigBoss.PlayerInfo.avatarStartLocation;
		}
		#endregion
		
		if (Input.GetKeyDown(KeyCode.Q))
		{
			
			//Debug only code:
			HeroCam hCam = (HeroCam)Camera.main.GetComponent("HeroCam") as HeroCam;
			hCam.enabled = !hCam.enabled;
			maxCamera mCam = (maxCamera)Camera.main.GetComponent("maxCamera") as maxCamera;
			mCam.enabled = !mCam.enabled;
			BigBoss.Gooey.CreateTextPop(BigBoss.PlayerInfo.transform.position + Vector3.up *.75f," CAMERA SWAP ",Color.white);
		}
			
		
			
	}
	void CheckForMouseInput ()
	{
	
		horizontalMouseAxis = Input.GetAxis("Mouse X");
		verticalMouseAxis = Input.GetAxis("Mouse Y");
		//mouseLocation = Input.mousePosition;
		
		if (Input.GetMouseButton(1)) //hold right click
		{			
			
			Vector2 centerScreenPointToMousePosLookVector = (Vector2)Input.mousePosition - centerPointInScreenSpace;
			Vector3 playerConvertedTranslationVector = new Vector3(centerScreenPointToMousePosLookVector.x,0,centerScreenPointToMousePosLookVector.y);
			Debug.DrawRay(BigBoss.PlayerInfo.PlayerAvatar.transform.position,playerConvertedTranslationVector,Color.magenta);
			BigBoss.PlayerInfo.MovePlayer(playerConvertedTranslationVector);
			
		}
		if (Input.GetMouseButtonDown(0)) //hold left click
		{			
			
		}
			
	}
	
	
}//end Mono
