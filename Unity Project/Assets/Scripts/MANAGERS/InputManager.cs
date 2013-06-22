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
	//Mouse:
	public float horizontalMouseSensitivity;
	public float horizontalMouseAxis;//Number spit out by Unity's Input.GetAxis
	public float verticalMouseSensitivity;
	public float verticalMouseAxis;//Number spit out by Unity's Input.GetAxis
	public Vector2 mouseLocation;

	
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
			//Standard Escape Menu/Pause
				if (Input.GetKeyDown(KeyCode.Escape)){BigBoss.TimeKeeper.TogglePause();}
			
				//if(Input.GetKeyDown(KeyCode.C))
		   //AudioController.PlayMusic("Plains");   //audiomanager in limbo for now
				if (Input.GetKeyDown(KeyCode.X))
		{
			Vector3 place = new Vector3(15f,.5f,18);
			BigBoss.NPCManager.CreateNPC(place,"newt");
			Debug.Log("X");
		}
		if (Input.GetKeyDown(KeyCode.W))
		{
			BigBoss.PlayerInfo.PlayerMoveForward();
		}
		if (Input.GetKeyDown(KeyCode.A))
		{
			Debug.Log("lulz");
			BigBoss.PlayerInfo.PlayerAvatar.renderer.enabled = !BigBoss.PlayerInfo.PlayerAvatar.renderer.enabled;
		}
		if (Input.GetKeyDown(KeyCode.S))
		{
			
		}
		if (Input.GetKeyDown(KeyCode.D))
		{
			
		}
			
		
			
	}
	void CheckForMouseInput ()
	{
	
		horizontalMouseAxis = Input.GetAxis("Mouse X");
		verticalMouseAxis = Input.GetAxis("Mouse Y");
		mouseLocation = Input.mousePosition;
		
		if (Input.GetMouseButtonDown(1)) //left click?
		{			
			//string mes = ("Hi!");
			BigBoss.Gooey.CreateTextPop(new Vector3 (BigBoss.PlayerInfo.PlayerAvatar.transform.position.x,BigBoss.PlayerInfo.PlayerAvatar.transform.position.y+3.5f,BigBoss.PlayerInfo.PlayerAvatar.transform.position.z),"Poisoned!",Color.green);
			//Vector3 camPoint = Camera.mainCamera.WorldToViewportPoint(BigBoss.PlayerInfo.playerAvatar.transform.position);
			//Debug.Log(camPoint);
			
		}
		if (Input.GetMouseButtonDown(2)) //right click?
		{			
			
		}
			
	}
	
	
}//end Mono
