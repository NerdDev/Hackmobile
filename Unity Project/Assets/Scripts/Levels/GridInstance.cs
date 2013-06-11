using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridInstance : MonoBehaviour, Grid {

    public GridPrototype prototype;
    List<WorldObject> containedObjects = new List<WorldObject>();

	// Use this for initialization
	void Start () {

    }

    public GridType type
    {
        get
        {
            return prototype.type;
        }
    }

    public string ascii
    {
        get
        {
            return prototype.ascii;
        }
    }
	
}
