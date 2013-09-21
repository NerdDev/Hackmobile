using UnityEngine;
using System.Collections;

public class Theme : MonoBehaviour {
	public GameObject wall;
	public GameObject door;
	public GameObject floor;

    public string[] keywords;

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
            case GridType.Path_Horiz:
                return getFloor();
            case GridType.Path_Vert:
                return getFloor();
            case GridType.Path_RT:
                return getFloor();
            case GridType.Path_RB:
                return getFloor();
            case GridType.Path_LT:
                return getFloor();
            case GridType.Path_LB:
                return getFloor();
            default:
                return getFloor();
        }
    }

}
