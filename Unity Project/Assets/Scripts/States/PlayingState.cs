using System.Collections;
using UnityEngine;

public class PlayingState : GameState
{
    public override void OnActivate()
	{
		Debug.Log("Playing state activated.");
		
	}
    public override void OnDeactivate()
	{
		Debug.Log("Playing state de-activated.");

		
	}
    public override void OnUpdate()
	{		
		//Debug.Log("Playing state updating....");

	}
}