using UnityEngine;
using System.Collections;

public class Theme : MonoBehaviour {
	public GameObject wall;
	public GameObject door;
	public GameObject floor;

    public GameObject getFloor()
    {
        return floor;
    }

    public GameObject getWall()
    {
        return wall;
    }

    public GameObject getDoor()
    {
        return door;
    }

    public GameObject Get(GridType t)
    {
        switch (t)
        {
            case GridType.Door:
                return getDoor();
            case GridType.Wall:
                return getWall();
            case GridType.Floor:
                return getFloor();
            default:
                return null;
        }
    }

}
