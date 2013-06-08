using System.Collections;
using UnityEngine;

public class PausedState : GameState
{
    public override void OnActivate()
	{
		Debug.Log("Paused state activated.");
		
	}
    public override void OnDeactivate()
	{
		Debug.Log("Paused state de-activated.");

		
	}
    public override void OnUpdate()
	{		
		//Debug.Log("Playing state updating....");

	}
}