using UnityEngine;
using System.Collections;

public class GUIManager : MonoBehaviour {

	/*
	 * The role of GUIManager is to centralize the use of methods that can be overloaded in order
	to create reusable, yet easily customizable screen graphics.  These should all return the GUIText
	object and allow for parameterization of text, fly-through style, delay til destruction, etc.
	
	//Example of creating text on screen:
		Managers.Gooey.CreateText(GUIManager.TextLocations.TopLeft,"Works!");
	*/
	
	//Safeties:
		public bool confirmationWindowOpen;
		public bool tooltipOpen;
	
		public GameObject debugText;
	//public int iTweenReference;
	//Window References:
		
	//Textures:
		//public Texture testTexture;
		
	
	
		
	
	#region NGUI REFERENCES   //these get switched to private towards completion - public now for editor hookups
	//Panels
	public UIPanel mainHUDPanel;
	
	//Sliders & Progress Bars
	public UISlider HUDxpbar;
	public UISlider HUDplayerHealthBar;
	
	//Labels
	public UILabel HUDHealthBarNumberLabel;
	public UILabel HUDcharacterNameLabel;
	public UILabel HUDcharacterTitleLabel;
	public UILabel HUDDungeonLevelLabel;
	public UILabel HUDStrengthLabel;
	public UILabel HUDStrengthValue;
	public UILabel HUDIntelligenceLabel;
	public UILabel HUDIntelligenceValue;
	public UILabel HUDDexterityLabel;
	public UILabel HUDDexterityValue;
	public UILabel hungerLabel;
	
	#endregion
	

	void Start () 
	{
		//StartCoroutine(ShowDebugInfo());//This handles background data collection and should not be touched
	
//			GrabNGUIReferences();
			
		
		

	}
	
	
//	void Update () 
//	{
//		
//		
//	}
	
	public void UpdateHealthBar()
	{
		HUDplayerHealthBar.sliderValue = (float)BigBoss.PlayerInfo.PlayerCurrentHealth/(float)BigBoss.PlayerInfo.PlayerMaxHealth;	
		HUDHealthBarNumberLabel.text = BigBoss.PlayerInfo.PlayerCurrentHealth + " / " + BigBoss.PlayerInfo.PlayerMaxHealth;
	}
	
	
	//Debugging Coroutine
	private IEnumerator ShowDebugInfo()
	{
	
		GUIText textComp;
		//If text doesn't exist, create one:
				if (GameObject.Find("DEBUG TEXT") == null)
			{
			debugText = new GameObject("DEBUG TEXT");
			GUIText text = debugText.AddComponent("GUIText") as GUIText;
			textComp = text;
			textComp.fontSize = 14;
			Vector3 placeToPutIt = new Vector3(0,1,1);
			debugText.transform.position = placeToPutIt;
			
			}
		else textComp = GameObject.Find("DEBUG TEXT").GetComponent("GUIText") as GUIText;
			
		
				
			
		while(this.enabled == true)//Infinite Loop:
		{
			
			if (BigBoss.GameStateManager.DebugMode	== true)//Only Calculate this stuff in Debug Mode:
			{
				textComp.enabled = true;
				
				//This is only on top because letting this run first frame will throw a nullref due to audio
				string line1DebugKey = "Tilde-Backquote Key Toggles Debug Mode\n";
				string line2TotalTimeRunning = BigBoss.TimeKeeper.TotalTimeSinceStartup;
				string line3CurrentScene = "\nCurrent Level/Scene: " + Application.loadedLevelName;				
				string line4CurrentGameState = "\nCurrent Game State: " + BigBoss.GameStateManager.State;
				string line5currentTime = "\n" + BigBoss.TimeKeeper.RealComputerTimeNeat;
				string line6totalTimePlayed = "\n" + BigBoss.TimeKeeper.TotalTimePlayedNeat;
				string line7MouseInfo = "\nMouse Stats:    X Location: " + BigBoss.PlayerInput.mouseLocation.x + "  -  Y Location: " +  BigBoss.PlayerInput.mouseLocation.y + "  -  X Axis: " + BigBoss.PlayerInput.horizontalMouseAxis + "  - Y Axis: " + BigBoss.PlayerInput.verticalMouseAxis;
				//string line8audioInfo;
				
				
				textComp.text = line1DebugKey + line2TotalTimeRunning + line3CurrentScene + line4CurrentGameState + line5currentTime + line6totalTimePlayed + line7MouseInfo;	
				
				yield return null;
				//yield return new WaitForSeconds (.05f);
			}
			else textComp.enabled = false;
			yield return null;
			
		}
		
		
		
		
	}
	
	public void UpdateHungerText(Color theCol)
	{
		hungerLabel.text = BigBoss.PlayerInfo.CurrentHungerLevel.ToString();
		hungerLabel.color = theCol;
	}
	
	void GrabNGUIReferences ()
	{
		//GUIButtonTest = (UIButton)GameObject.Find("ButtonTest").GetComponent(typeof(UIButton));
		

	}
}
