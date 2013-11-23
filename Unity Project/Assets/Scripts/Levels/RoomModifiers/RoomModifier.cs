using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

abstract public class RoomModifier : ProbabilityItem
{

    static ProbabilityPool<RoomModifier>[] mods = new ProbabilityPool<RoomModifier>[Enum.GetNames(typeof(RoomModType)).Length];
    static bool initialized = false;

    public static void RegisterModifiers()
    {
        List<RoomModifier> modPrototypes = BigBoss.Types.GetInstantiations<RoomModifier>();
        foreach (RoomModType e in Enum.GetValues(typeof(RoomModType)))
        {
            mods[(int)e] = ProbabilityPool<RoomModifier>.Create(Probability.LevelRand);
        }
        foreach (RoomModifier mod in modPrototypes)
        {
            mods[(int)mod.GetType()].Add(mod);
        }
        initialized = true;
    }

    public static RoomModifier GetBase()
    {
        if (!initialized)
            RegisterModifiers();
        return mods[(int)RoomModType.Base].Get();
    }

    public static List<RoomModifier> GetFlexible(int num)
    {
        if (!initialized)
            RegisterModifiers();
        return mods[(int)RoomModType.Flexible].Get(num);
    }

    public static RoomModifier GetFinal()
    {
        if (!initialized)
            RegisterModifiers();
        return mods[(int)RoomModType.Final].Get();
    }

    public override string ToString()
    {
        return GetName();
    }

    // Inherited Functions
    public abstract bool Modify(RoomSpec spec);

    public new abstract RoomModType GetType();

    public abstract string GetName();

    public virtual int ProbabilityDiv()
    {
        return 1;
    }

    public virtual bool IsUnique()
    {
        return false;
    }

    #region Preset Funcs
    public static DrawAction<GridType> SetToIfNull(GridType g)
    {
        return new DrawAction<GridType>((arr, x, y) =>
        {
            if (arr[y, x] == GridType.NULL)
                arr[y, x] = g;
            return true;
        }
        );
    }

    public static DrawAction<GridType> SetToIfNotNull(GridType g)
    {
        return new DrawAction<GridType>((arr, x, y) =>
        {
            if (arr[y, x] != GridType.NULL)
                arr[y, x] = g;
            return true;
        }
        );
    }

    public static DrawAction<GridType> LoadDoorOptions(List<Point> list)
    {
        return new DrawAction<GridType>((arr, x, y) =>
        {
                if (arr.CanDrawDoor(x, y))
                    list.Add(new Point(x, y));
                return true;
            }
        );
    }

    #endregion
}
