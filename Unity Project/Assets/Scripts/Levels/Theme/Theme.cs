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
    public ThemeElement[] Chest;
    protected RoomModCollection _roomMods;
    public SpawnKeywords[] Keywords;
    public GenericFlags<SpawnKeywords> KeywordFlags;

    public virtual void Init()
    {
        _roomMods = new RoomModCollection();
        KeywordFlags = new GenericFlags<SpawnKeywords>(Keywords);
    }

    public RoomModCollection GetRoomMods()
    {
        return new RoomModCollection(_roomMods);
    }
}
