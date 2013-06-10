using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridBox : MonoBehaviour {

    GridPrototype prototype;
    List<WorldObject> containedObjects = new List<WorldObject>();

    GridBox(GridPrototype proto)
    {
        prototype = proto;
    }

	// Use this for initialization
	void Start () {
	
	}

    public GridType getGridType()
    {
        return prototype.type;
    }
	
}
