using UnityEngine;
using System.Collections;
using UnityEditor;

public class Theme : MonoBehaviour
{
    public ThemeElement WallElement;
    public GameObject Wall;
    public ThemeElement DoorElement;
    public GameObject Door;
    public ThemeElement FloorElement;
    public GameObject Floor;
    public ThemeElement StairUpElement;
    public GameObject StairUp;
    public ThemeElement StairDownElement;
    public GameObject StairDown;
    public ThemeElement PillarElement;
    public GameObject Pillar;
    public ThemeElement ChestElement;
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

    public void Init()
    {
        DontDestroyOnLoad(Wall);
        WallElement = new ThemeElement(Wall);
        DontDestroyOnLoad(Door);
        DoorElement = new ThemeElement(Door);
        DontDestroyOnLoad(Floor);
        FloorElement = new ThemeElement(Floor);
        DontDestroyOnLoad(StairUp);
        StairUpElement = new ThemeElement(StairUp);
        DontDestroyOnLoad(StairDown);
        StairDownElement = new ThemeElement(StairDown);
        DontDestroyOnLoad(Pillar);
        PillarElement = new ThemeElement(Pillar);
        DontDestroyOnLoad(Chest);
        ChestElement = new ThemeElement(Chest);
    }

    public ThemeElement Get(GridType t)
    {
        switch (t)
        {
            case GridType.Door:
                return DoorElement;
            case GridType.Wall:
                return WallElement;
            case GridType.StairUp:
                return StairUpElement;
            case GridType.StairDown:
                return StairDownElement;
            case GridType.Chest:
                return ChestElement;
            case GridType.NULL:
                return null;
            default:
                return FloorElement;
        }
    }

}
