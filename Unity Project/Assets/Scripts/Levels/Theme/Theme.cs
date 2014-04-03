using UnityEngine;
using System.Collections;
using UnityEditor;
using System;
using System.Collections.Generic;

public class Theme : ScriptableObject, IInitializable
{
    public bool Scatterable;
    public bool Chainable;
    #region Core Elements
    public ThemeElementBundle Wall;
    public ThemeElementBundle Door;
    public ThemeElementBundle Floor;
    public ThemeElementBundle Stair;
    public ThemeElementBundle Chest;
    #endregion
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

    public void ChooseAllSmartObjects(System.Random rand)
    {
        foreach (ThemeElementBundle bundle in this.FindAllDerivedObjects<ThemeElementBundle>(false))
        {
            bundle.Select(rand);
        }
    }

    public Theme Flatten()
    {
        Theme ret = (Theme) this.MemberwiseClone();
        Type bundleType = typeof(ThemeElementBundle);
        foreach (var field in ret.GetType().GetFields())
        {
            if (bundleType.IsAssignableFrom(bundleType))
            {
                ThemeElementBundle bundle = (ThemeElementBundle) field.GetValue(this);
                field.SetValue(this, bundle.SmartElement);
            }
        }
        return ret;
    }
}
