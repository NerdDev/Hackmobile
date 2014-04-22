using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class TimeManager : MonoBehaviour, IManager
{

    public bool Initialized { get; set; }
    /* The Time Manager script is responsible for handling as many time related functions in game as possible.  The purpose of 
     * a manager script is to robustify the Time class in order to keep track of time on certain scenes, time played on certain levels, time spent
     * doing certain things for debugging and analytic purposes.  */

    #region Game Clocks
    public int MinutesSinceStartup { get { return (int)System.Math.Round(Time.realtimeSinceStartup / 60f, 1); } }//Returns TOTAL minutes
    public int HoursSinceStartup { get { return (int)System.Math.Round((Time.realtimeSinceStartup / 60f) / 60, 1); } }//Returns TOTAL hours
    public int MinutesSinceSceneLoad { get { return (int)Time.timeSinceLevelLoad / 60; } }//Returns TOTAL minutes since scene was load - refreshes to 0
    //Make sure to save timeSinceLevelLoad and add to this variable:
    public int totalTimePlayed;//This is held here in minutes, and formatted below into a nice string.

    //A neat preformatted string that we can easily use for debugging on whoever queries it:
    public string TotalTimeSinceStartup
    {
        get
        {
            return "Total time since game was launched:  "
                + HoursSinceStartup + " Hours and "
                + (MinutesSinceStartup - 60 * HoursSinceStartup)
                + " Minutes.";
        }
    }
    public string RealComputerTimeNeat
    {
        get { return System.DateTime.Now.ToShortTimeString(); }
    }
    public string TotalTimePlayedNeat
    {
        get
        {
            int hours = (int)System.Math.Round((totalTimePlayed / 60f), 1);
            int leftoverMinutes = (int)totalTimePlayed - (hours * 60);
            string nicelyFormatted = "Total Time Played is " + hours + " hours and " + leftoverMinutes + " minutes.";
            return nicelyFormatted;
        }
    }

    public int turnsPassed = 0;
    public int numTilesCrossed = 0;
    #endregion
    #region Action Costs
    public int diagonalMoveCost = 84;
    public int regularMoveCost = 1;
    public int attackCost = 60;
    public int eatItemCost = 60;

    public float TimeInterval;
    #endregion

    public void Initialize()
    {
        totalTimePlayed = PlayerPrefs.GetInt("MinutesPlayed", totalTimePlayed);
        StartCoroutine(TheseThingsWillHappenOncePerMinute());
        StartCoroutine(TurnProcessing());
    }

    public void TogglePause()
    {
        if (Time.timeScale == 1)//If Unpaused:
        {
            Time.timeScale = 0;
        }
        else if (Time.timeScale == 0)//If Paused:
        {
            Time.timeScale = 1;
        }
    }

    private IEnumerator TheseThingsWillHappenOncePerMinute()
    {
        while (Application.isPlaying)
        {
            yield return new WaitForSeconds(1f);
            totalTimePlayed++;
            yield return new WaitForSeconds(60f);//Initial 60 second delay til we compute things:
        }
    }

    void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("MinutesPlayed", totalTimePlayed);
        Debug.Log("Application Quit Called.");
    }

    #region GAME TIME METHODS

    public int CapOnTurnPoints;
    public void PassTurn(int turnPoints)
    {
        for (int i = 0; i < turnPoints; i++)
            PassTurn();
    }

    private void PassTurn()
    {
        turnsPassed++;
        //Justin's hot: // Thx brah
        if (turnsPassed % 2 == 0)
            runGroupUpdate();
    }

    #endregion

    #region Objects to Update
    public HashSet<PassesTurns> TotalObjectList = new HashSet<PassesTurns>();
    internal Queue<PassesTurns> updateList = new Queue<PassesTurns>();

    public void RegisterToUpdateList<T>(T obj) where T : PassesTurns
    {
        //TotalObjectList.Add(obj);
    }

    public void RemoveFromUpdateList<T>(T obj) where T : PassesTurns
    {
        //TotalObjectList.Remove(obj);
    }

    public void runGroupUpdate()
    {
        foreach (PassesTurns obj in TotalObjectList)
        {
            if (obj.IsActive)
            {
                updateList.Enqueue(obj);
            }
        }
    }

    internal IEnumerator TurnProcessing()
    {
        while (Time.timeScale > 0)
        {
            if (updateList.Count > 0)
            {
                PassesTurns obj = updateList.Dequeue();
                if (TotalObjectList.Contains(obj))
                {
                    runUpdate(obj);
                }
            }
            else
            {
                //BigBoss.PlayerInput.allowTouchInput = true;
            }
            yield return null;
        }
    }

    public void runUpdate<T>(T obj) where T : PassesTurns
    {
        //The object must lower it's own base points
        obj.UpdateTurn();
    }
    #endregion
}
