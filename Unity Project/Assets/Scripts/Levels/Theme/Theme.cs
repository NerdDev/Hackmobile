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
        Counter horiz = new Counter();
        cont.DrawLineExpanding(x, y, GridDirection.HORIZ, 5, Draw.CanDrawDoor().IfThen(Draw.Count<GenSpace>(horiz)));
        Counter vert = new Counter();
        cont.DrawLineExpanding(x, y, GridDirection.VERT, 5, Draw.CanDrawDoor().IfThen(Draw.Count<GenSpace>(vert)));

        // Find largest
        GridDirection dir;
        MultiMap<GenSpace> map;
        int count;
        if (horiz.Count < vert || (horiz == vert && rand.NextBool()))
        {
            count = vert;
            dir = GridDirection.VERT;
        }
        else
        {
            count = horiz;
            dir = GridDirection.HORIZ;
        }
        count = Math.Max(count, 1);

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

        ThemeElement door = doorElement.Get(rand);
        cont.DrawLineExpanding(x, y, dir, count / 2, Draw.MergeIn(door, this, GridType.Door).And(Draw.Around(false, Draw.IsNull<GenSpace>().IfThen(Draw.SetTo(GridType.Floor, this)))));
    }
}
