using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

abstract public class RoomModifier : ProbabilityItem
{
    public virtual double Multiplier { get { return 1; } }
    public virtual bool Unique { get { return false; } }

    static ProbabilityPool<RoomModifier>[] mods = new ProbabilityPool<RoomModifier>[Enum.GetNames(typeof(RoomModType)).Length];
    static bool initialized = false;

    public static void RegisterModifiers()
    {
        #region Debug
        double time = 0;
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            time = Time.realtimeSinceStartup;
        }
        #endregion
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
        #region Debug
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.w(Logs.LevelGen, "Registering Room Modifiers took " + (Time.realtimeSinceStartup - time));
        }
        #endregion
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
}
