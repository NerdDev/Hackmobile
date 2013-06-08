using UnityEngine;
using System.Collections;

public class TimedDestruction : MonoBehaviour {
	
	
	
//	// Use this for initialization
//	void Start () {
//	
//	}
	
//	// Update is called once per frame
//	void Update () {
//	
//	}
	
	public IEnumerator DestroyMe (float delay)
	{
		
		yield return new WaitForSeconds(delay);
		Destroy(gameObject);
		
	}
}
