using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TimeManager : MonoBehaviour {
	
	/* The Time Manager script is responsible for handling as many time related functions in game as possible.  The purpose of 
	 * a manager script is to robustify the Time class in order to keep track of time on certain scenes, time played on certain levels, time spent
	 * doing certain things for debugging and analytic purposes.  */
	
	#region Game Clocks
	public int MinutesSinceStartup{get{return (int)System.Math.Round(Time.realtimeSinceStartup / 60f,1);}	}//Returns TOTAL minutes
	public int HoursSinceStartup{get{return (int)System.Math.Round((Time.realtimeSinceStartup / 60f)/60,1);}	}//Returns TOTAL hours
	public int MinutesSinceSceneLoad{get{return (int)Time.timeSinceLevelLoad/60;}}//Returns TOTAL minutes since scene was load - refreshes to 0
	//Make sure to save timeSinceLevelLoad and add to this variable:
	public int totalTimePlayed;//This is held here in minutes, and formatted below into a nice string.
	
	//A neat preformatted string that we can easily use for debugging on whoever queries it:
	public string TotalTimeSinceStartup{get{return "Total time since game was launched:  " + HoursSinceStartup + " Hours and " + (MinutesSinceStartup - 60*HoursSinceStartup) + " Minutes.";}}
	public string RealComputerTimeNeat{get{return System.DateTime.Now.ToShortTimeString();}}
	public string TotalTimePlayedNeat
	{
	
		get{
			int hours = (int)System.Math.Round((totalTimePlayed / 60f),1);
			int leftoverMinutes = (int)totalTimePlayed - (hours*60);
			string nicelyFormatted = "Total Time Played is " + hours + " hours and " + leftoverMinutes + " minutes.";
			return nicelyFormatted;
			}
	}
	
	public int turnsPassed = 0;
	public int numTilesCrossed = 0;
	#endregion
	#region Expose Property In Inspector Example
	//This is our inspector property example:   Copy this example for use within this TimeManager class.  To expose properties in other classes, make a new script copied from TimeManagerEditor.cs
		[HideInInspector] [SerializeField] int myTestIntprivate;
	[ExposeProperty]
    public int MyTestInt
    {
        get
        {
            return myTestIntprivate;
        }
        set
        {
            myTestIntprivate = value;  
        }
    }
	[HideInInspector] [SerializeField] string myTeststringprivate;
	[ExposeProperty]
    public string MyTestString
    {
        get
        {
            return myTeststringprivate;
        }
        set
        {
            myTeststringprivate = value;  
        }
    }
	#endregion
	
	// Use this for initialization
	void Start () 
	{
	
		totalTimePlayed = PlayerPrefs.GetInt("MinutesPlayed",totalTimePlayed);
		StartCoroutine(TheseThingsWillHappenOncePerMinute());
		
	}
	
	// Update is called once per frame
	void Update () 
	{
	    //Does this do anything for us? Or should it simply not be tracked.
	}
	
	public void TogglePause()
	{
		//Creating a reference to our original State:  NOT SURE IF THIS IS POSSIBLE SINCE WE CANT DETERMINE THE DATA TYPE FOR THE VARIABLE AHEAD OF TIME
		//string stringRef = Managers.GameStateManager.State.ToString();
		//GameObject objectRef = GameObject.Find(stringRef);
		
		
		
			if (Time.timeScale == 1)//If Unpaused:
		{			
			//prePauseState = Managers.GameStateManager.State;
			BigBoss.GameStateManager.SetState(typeof(PausedState));
			Time.timeScale = 0;
		}
		else if (Time.timeScale == 0)//If Paused:
		{
			//Managers.GameStateManager.SetState(typeof(prePauseState));
			Time.timeScale = 1;			
		}
		
	}
	
	private IEnumerator TheseThingsWillHappenOncePerMinute()
	{
	
		while(Application.isPlaying)
		{
			yield return new WaitForSeconds(1f);
			//Managers.Audio.AdjustMusicVolume(.02f,2f);works
			totalTimePlayed ++;
			//Managers.Audio.PlaySFX(Managers.Audio.otherSFX,transform.position,.1f);//works
			yield return new WaitForSeconds(60f);//Initial 60 second delay til we compute things:
		}
	}
	
	void OnApplicationQuit()
	{
		PlayerPrefs.SetInt("MinutesPlayed",totalTimePlayed);
		Debug.Log("Application Quit Called.");
	}
	
	#region GAME TIME METHODS
	public void PassTurn(int turnPoints)
	{
		turnsPassed++;
		//Justin's hot:

        foreach (PassesTurns obj in updateList)
        {
            runUpdate(obj, turnPoints);
        }
	}
	
	#endregion

    #region Objects to Update
    public List<PassesTurns> updateList = new List<PassesTurns>();

    public void RegisterToUpdateList<T>(T obj) where T: PassesTurns
    {
        updateList.Add(obj);
    }

    public void RemoveFromUpdateList<T>(T obj) where T : PassesTurns
    {
        updateList.Remove(obj);
    }

    public void runUpdate<T>(T obj, int turnPoints) where T : PassesTurns
    {
        obj.CurrentPoints += turnPoints;
        if (obj.CurrentPoints  >= obj.BasePoints) //this will prevent AI processing from doing all the small actions constantly
        {
            obj.UpdateTurn();
        }
    }
    #endregion
}
