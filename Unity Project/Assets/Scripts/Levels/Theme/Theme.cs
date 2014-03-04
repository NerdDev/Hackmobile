using UnityEngine;
using System.Collections;
using UnityEditor;
using System;

public class Theme : MonoBehaviour
{
    public bool Scatterable;
    public bool Chainable;
    public ThemeElement[] Wall;
    public ThemeElement[] Door;
    public ThemeElement[] Floor;
    public ThemeElement[] StairUp;
    public ThemeElement[] StairDown;
    public ThemeElement[] Pillar;
    public ThemeElement[] Chest;
    protected RoomModCollection _roomMods;
    public SpawnKeywords[] Keywords;
    public GenericFlags<SpawnKeywords> KeywordFlags;

    public virtual void Init()
    {
        _roomMods = new RoomModCollection();
        KeywordFlags = new GenericFlags<SpawnKeywords>(Keywords);
    }

    public ThemeElement Get(GridType t, System.Random rand)
    {
        switch (t)
        {
            case GridType.Door:
                return Door.Random(rand);
            case GridType.Wall:
                return Wall.Random(rand);
            case GridType.StairUp:
                return StairUp.Random(rand);
            case GridType.StairDown:
                return StairDown.Random(rand);
            case GridType.Chest:
                return Chest.Random(rand);
            case GridType.NULL:
                return null;
            default:
                return Floor.Random(rand);
        }
    }

    public RoomModCollection GetRoomMods()
    {
        return new RoomModCollection(_roomMods);
    }
}
