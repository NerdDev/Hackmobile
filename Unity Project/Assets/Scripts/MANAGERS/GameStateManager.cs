using UnityEngine;
using System.Collections;

/*  The GameStateManager class does NOT handle generic Game Information - it ONLY handles GAME STATES!!!  To implement a new custom game state make a new
 * C# script with the name of the desired state - ex. "MainMenuState".  The C# script should inherit from the GameState abstract class  and be attached 
 * to an empty gameobject of the same name.  This gameobject should be a child of the GameStateManager.  */
public class GameStateManager : MonoBehaviour 
{
	
	public bool DebugMode;
	
    private GameState currentState;
    public GameState State
    {
        get { return currentState; }
    }
 
    //Changes the current game state
    public void SetState(System.Type newStateType)
    {
        if (currentState != null)
        {
            currentState.OnDeactivate();
        }
 
        currentState = GetComponentInChildren(newStateType) as GameState;
        if (currentState != null)
        {
            currentState.OnActivate();
        }
    }
 
    void Update()
    {
        if (currentState != null)
        {
            currentState.OnUpdate();
        }
    }
 
    void Start()
    {
        SetState(typeof(MainMenuState));
    }
	
	
	public void ToggleDebugMode()
	{DebugMode = !DebugMode;}
}
