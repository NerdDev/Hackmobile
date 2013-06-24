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
			BigBoss.ItemMaster.CreateRandomItem(place);
			Debug.Log("X");
		}
		if (Input.GetKeyDown(KeyCode.Alpha1))
		{
			BigBoss.PlayerInfo.GainLevel();
		}
		if (Input.GetKeyDown(KeyCode.Q))
		{
			BigBoss.PlayerInfo.GetXPToNextLevel();
		}
			
		
			
	}
	void CheckForMouseInput ()
	{
	
		horizontalMouseAxis = Input.GetAxis("Mouse X");
		verticalMouseAxis = Input.GetAxis("Mouse Y");
		mouseLocation = Input.mousePosition;
		
		if (Input.GetMouseButtonDown(1))
		{			
			
		}
			
	}
	
	
}//end Mono
