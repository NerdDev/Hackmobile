using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

abstract public class RoomModifier : ProbabilityItem {

    static ProbabilityPool<RoomModifier>[] mods = new ProbabilityPool<RoomModifier>[Enum.GetNames(typeof(RoomModType)).Length];

    public static void RegisterModifiers()
    {
        List<RoomModifier> modPrototypes = new List<RoomModifier>();
        modPrototypes.Add(new SquareRoom());
        modPrototypes.Add(new RectangularRoom());
        modPrototypes.Add(new CircleRoom());
        modPrototypes.Add(new TrapRoomMod());
        modPrototypes.Add(new PillarMod());
        modPrototypes.Add(new GiantPillarMod());
        modPrototypes.Add(new SplitterMod());
        modPrototypes.Add(new TreasureRoomMod());
        modPrototypes.Add(new HiddenRoomMod());
        modPrototypes.Add(new FinalMod());

        #region Internal
        foreach (RoomModType e in Enum.GetValues(typeof(RoomModType)))
        {
            mods[(int)e] = ProbabilityPool<RoomModifier>.Create(Probability.LevelRand);
        }
        foreach (RoomModifier mod in modPrototypes)
        {
            mods[(int)mod.GetType()].Add(mod);
        }
        #endregion Internal
    }

    public static RoomModifier GetBase()
    {
        return mods[(int)RoomModType.Base].Get();
    }

    public static List<RoomModifier> GetFlexible(int num)
    {
        return mods[(int)RoomModType.Flexible].Get(num);
    }

    public static RoomModifier GetFinal()
    {
        return mods[(int)RoomModType.Final].Get();
    }

    public override string ToString()
    {
        return GetName();
    }

    // Inherited Functions
    public abstract void Modify(Room room, RandomGen rand);

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
}
