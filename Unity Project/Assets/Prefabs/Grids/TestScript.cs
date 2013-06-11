using UnityEngine;
using System.Collections;

public class TestScript : MonoBehaviour {

    public GridInstance one;
    public GridInstance two;

	// Use this for initialization
    void Start()
    {
        Debug.Log("One: " + one.prototype.name);
        Debug.Log("TWo: " + two.prototype.name);
        one.prototype.name = "Bloop";
        Debug.Log("One: " + one.prototype.name);
        Debug.Log("TWo: " + two.prototype.name);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
