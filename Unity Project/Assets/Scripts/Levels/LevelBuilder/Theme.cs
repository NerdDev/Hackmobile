using UnityEngine;
using System.Collections;

public class Theme : MonoBehaviour {
	public GameObject Wall;
	public GameObject Door;
    public GameObject Floor;
    public GameObject StairUp;
    public GameObject StairDown;
    public GameObject Pillar;
    public GameObject Chest;

    public Keywords[] keywords = new Keywords[0];
    private ESFlags<Keywords> keywordFlags;
    public ESFlags<Keywords> Keywords
    {
        get
        {
            if (keywordFlags == null)
            {
                keywordFlags = new ESFlags<Keywords>(keywords);
            }
            return keywordFlags;
        }
    }

    public GameObject Get(GridType t)
    {
        switch (t)
        {
            case GridType.Door:
                return Door;
            case GridType.Wall:
                return Wall;
            case GridType.StairUp:
                return StairUp;
            case GridType.StairDown:
                return StairDown;
            case GridType.Chest:
                return Chest;
            case GridType.NULL:
                return null;
            default:
                return Floor;
        }
    }

}
