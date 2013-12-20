using UnityEngine;
using System.Collections;

public class JustinTests : MonoBehaviour {

	// Use this for initialization
	void Start () {
        bool[,] arr = new bool[30, 30];
        arr.DrawSpiral(5, 5, Draw.SetTo(true).Then((arr2, x, y) => { arr2.ToLog(Logs.Main); return true; }));
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
