using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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
		
	
	public List<UILabel> textPopList; //A dynamic array of created textPop objects - used for orderly management and GO cleanup
		
	
	#region NGUI REFERENCES FOR TYPE A UI   //these get switched to private towards completion - public now for editor hookups
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
	public UILabel tilesCrossedLabel;  //currently for dev only
	
	
	#endregion
	
	#region NGUI REFERENCES FOR TYPE B UI   //these get switched to private towards completion - public now for editor hookups
	//Panels
	public UIPanel MainHUDTypeBMotherPanel;
	public UISprite mainHUDTabA; //hero
	public UISprite mainHUDTabB;  //inventory
	public UISprite mainHUDTabC; //stats/dungeon/achievements
	public UISprite mainHUDTabD;  //option tab
	
	public UISprite keyWSprite;
	public UISprite keyASprite;
	public UISprite keySSprite;
	public UISprite keyDSprite;
	
	bool isUIPanelBUp = false;
	public iTweenEvent tweenPanelBUp;
	public iTweenEvent tweenPanelBDown;
	//Code transform location for the mother panel:
	public Vector3 HUDBPanelMotherLocationWhenUp = new Vector3 (-3f,50f,-5f);
	public Vector3 HUDBPanelMotherLocationWhenDown = new Vector3 (-3f,-75f,-5f);
	
	//Buttons:
	public UIImageButton HUDBarrowPullUpButton;
	//Sliders & Progress Bars
//	public UISlider HUDxpbar;
//	public UISlider HUDplayerHealthBar;
//	
//	//Labels
	public UILabel HUD2XPLabel;
//	public UILabel HUDcharacterNameLabel;
//	public UILabel HUDcharacterTitleLabel;
//	public UILabel HUDDungeonLevelLabel;
//	public UILabel HUDStrengthLabel;
//	public UILabel HUDStrengthValue;
//	public UILabel HUDIntelligenceLabel;
//	public UILabel HUDIntelligenceValue;
//	public UILabel HUDDexterityLabel;
//	public UILabel HUDDexterityValue;
//	public UILabel hungerLabel;
	
	#endregion
	

	void Start () 
	{
		//StartCoroutine(ShowDebugInfo());//This handles background data collection and should not be touched
	
//			GrabNGUIReferences();
		
		tweenPanelBUp = iTweenEvent.GetEvent(MainHUDTypeBMotherPanel.gameObject,"TweenUp");
		tweenPanelBDown = iTweenEvent.GetEvent(MainHUDTypeBMotherPanel.gameObject,"TweenDown");
		MainHUDTypeBMotherPanel.gameObject.transform.localPosition = HUDBPanelMotherLocationWhenDown;
				

	}
	
	
	
//	void Update () 
//	{
//		
//		
//	}
	
	
	
	
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
			/*
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
				//string line7MouseInfo = "\nMouse Stats:    X Location: " + BigBoss.PlayerInput.mouseLocation.x + "  -  Y Location: " +  BigBoss.PlayerInput.mouseLocation.y + "  -  X Axis: " + BigBoss.PlayerInput.horizontalMouseAxis + "  - Y Axis: " + BigBoss.PlayerInput.verticalMouseAxis;
				string line7MouseInfo = "Center Screen Space Point: " + BigBoss.PlayerInput.centerPointInScreenSpace + ", Current Mouse Screen Position: " + ((Vector2)Input.mousePosition);
				//string line8audioInfo;
				
				
				textComp.text = line1DebugKey + line2TotalTimeRunning + line3CurrentScene + line4CurrentGameState + line5currentTime + line6totalTimePlayed + line7MouseInfo;	
				
				yield return null;
				//yield return new WaitForSeconds (.05f);
			}
                */
			//else textComp.enabled = false;
			yield return null;
			
		}
		
		
		
		
	}
	
	#region UPDATING OF VARIOUS NGUI ELEMENTS - GENERALLY DRIVEN FROM CODE ELSEWHERE IN THE PROJECT
	public void UpdateHealthBar()
	{
		HUDplayerHealthBar.sliderValue = (float)BigBoss.PlayerInfo.stats.CurrentHealth/(float)BigBoss.PlayerInfo.stats.MaxHealth;	
		HUDHealthBarNumberLabel.text = BigBoss.PlayerInfo.stats.CurrentHealth + " / " + BigBoss.PlayerInfo.stats.MaxHealth;
	}
	
	public void UpdateHungerText(Color theCol)
	{
		hungerLabel.text = BigBoss.PlayerInfo.stats.HungerLevel.ToString();
		hungerLabel.color = theCol;
	}
	
	public void UpdateXPBar()
	{
		HUDxpbar.sliderValue = (float)BigBoss.PlayerInfo.stats.CurrentXP/1000f;
		HUD2XPLabel.text = BigBoss.PlayerInfo.stats.CurrentXP + " / 1000";
	}
	
	public void UpdateTilesCrossedLabel()
	{
		tilesCrossedLabel.text = "Tiles Crossed: " + BigBoss.TimeKeeper.numTilesCrossed;
	}
	
	public void CreateTextPop(Vector3 worldPosition, string message, Color col)
	{
		//Vector3 actualLoc = new Vector3(worldPosition.x,worldPosition.y,worldPosition.z);
		Vector3 camPoint = Camera.mainCamera.WorldToViewportPoint(worldPosition);
		Debug.Log(camPoint);
		GameObject go = Instantiate(BigBoss.Prefabs.textPopPrefab,camPoint,Quaternion.identity) as GameObject;
		GUIText textComp = (GUIText) go.GetComponent<GUIText>();
		textComp.text = message;
		textComp.material.color = col;
	}

//	public void TextPopEnd()  //This is method is called when the iTween on TextPops Completes.  Signals destruction of GO
//	{
//		DestroyEarliestTextPop();//iTween event was thrown - we can safely assume it's the first object in the list since we created in order
//	}//FLAGGED FOR DELETION


	
	#endregion
	
	
	#region HUD B INTERACTIONS:
	
	public void HUD2Button_ArrowUP_Click()
	{
	
		if (isUIPanelBUp == false) //panel is down - tween up
		{
			//MainHUDTypeBMotherPanel.gameObject.transform.localPosition = HUDBPanelMotherLocationWhenUp;
			tweenPanelBUp.Play();
			isUIPanelBUp = true;
			
			
		}
		else if(isUIPanelBUp == true) //panel is up - tween down
		{
			//MainHUDTypeBMotherPanel.gameObject.transform.localPosition = HUDBPanelMotherLocationWhenDown;
			tweenPanelBDown.Play();
			isUIPanelBUp = false;
			
		}
	}
	
	
	#endregion
	void GrabNGUIReferences ()
	{
		//GUIButtonTest = (UIButton)GameObject.Find("ButtonTest").GetComponent(typeof(UIButton));
		

	}
}
