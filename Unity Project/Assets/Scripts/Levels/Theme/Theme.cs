using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;

public class Theme : ScriptableObject, IInitializable
{
    public bool Scatterable;
    public bool Chainable;
    public CoreElements Core;
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
