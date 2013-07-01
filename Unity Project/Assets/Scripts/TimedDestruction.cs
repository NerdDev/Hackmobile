using UnityEngine;
using System.Collections;

public class TimedDestruction : MonoBehaviour {
	
	public float delayTilDeath;
	
	// Use this for initialization
//	void Start () 
//	{
//		
//		StartCoroutine.DestroyMe(delayTilDeath);		
//	
//	}
	
//	// Update is called once per frame
//	void Update () {
//	
//	}
	
	IEnumerator Start ()
	{
		
		yield return new WaitForSeconds(delayTilDeath);
		Destroy(gameObject);
		
	}
}
