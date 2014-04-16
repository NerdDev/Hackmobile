using UnityEngine;
using System.Collections;
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

    public void PlaceDoor(Container2D<GenSpace> cont, int x, int y, System.Random rand)
    {
        // Count largest option
        Counter horizCount = new Counter();
        cont.DrawLine(x - 5, x + 5, y, true, Draw.CanDrawDoor().IfThen(Draw.Count<GenSpace>(horizCount)));
        Counter vertCount = new Counter();
        cont.DrawLine(y - 5, y + 5, x, false, Draw.CanDrawDoor().IfThen(Draw.Count<GenSpace>(vertCount)));

        // Find largest
        GridDirection dir;
        int count = 1;
        if (horizCount < vertCount)
        {
            count = vertCount;
            dir = GridDirection.VERT;
        }
        else if (horizCount > vertCount)
        {
            count = horizCount;
            dir = GridDirection.HORIZ;
        }
        else
        {
            dir = rand.NextBool() ? GridDirection.HORIZ : GridDirection.VERT;
        }

        // Pick random size
        for (; count > 1; count--)
        {
            if (rand.NextBool())
            {
                break;
            }
        }

        // Pick door
        SmartThemeElement doorElement;
        while (!Door.Select(rand, count, 1, out doorElement) && count > 1)
        {
            count--;
        }

        cont.DrawLine(x, y, dir, count / 2, Draw.MergeIn(doorElement, rand, this, GridType.Door));
    }
}
