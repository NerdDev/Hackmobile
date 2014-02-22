using UnityEngine;
using System.Collections;
using UnityEditor;

public class Theme : MonoBehaviour
{
    public bool Scatterable;
    public bool Chainable;
    public ThemeElement[] WallElement;
    public GameObject[] Wall;
    public ThemeElement[] DoorElement;
    public GameObject[] Door;
    public ThemeElement[] FloorElement;
    public GameObject[] Floor;
    public ThemeElement[] StairUpElement;
    public GameObject[] StairUp;
    public ThemeElement[] StairDownElement;
    public GameObject[] StairDown;
    public ThemeElement[] PillarElement;
    public GameObject[] Pillar;
    public ThemeElement[] ChestElement;
    public GameObject[] Chest;

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

    public void Init()
    {
        WallElement = Generate(Wall);
        DoorElement = Generate(Door);
        FloorElement = Generate(Floor);
        StairUpElement = Generate(StairUp);
        StairDownElement = Generate(StairDown);
        PillarElement = Generate(Pillar);
        ChestElement = Generate(Chest);
    }

    protected ThemeElement[] Generate(GameObject[] objs)
    {
        ThemeElement[] ret = new ThemeElement[objs.Length];
        for (int i = 0; i < objs.Length; i++)
        {
            DontDestroyOnLoad(objs[i]);
            ret[i] = new ThemeElement(objs[i]);
        }
        return ret;
    }

    public ThemeElement Get(GridType t, System.Random rand)
    {
        switch (t)
        {
            case GridType.Door:
                return DoorElement.Random(rand);
            case GridType.Wall:
                return WallElement.Random(rand);
            case GridType.StairUp:
                return StairUpElement.Random(rand);
            case GridType.StairDown:
                return StairDownElement.Random(rand);
            case GridType.Chest:
                return ChestElement.Random(rand);
            case GridType.NULL:
                return null;
            default:
                return FloorElement.Random(rand);
        }
    }

}
