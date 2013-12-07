using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

abstract public class RoomModifier : ProbabilityItem
{
    public virtual double Multiplier { get { return 1; } }
    public virtual bool Unique { get { return false; } }

    public static ProbabilityPool<RoomModifier>[] Mods = new ProbabilityPool<RoomModifier>[Enum.GetNames(typeof(RoomModType)).Length];
    static bool initialized = false;

    public static void RegisterModifiers()
    {
        #region Debug
        double time = 0;
        if (BigBoss.Debug.logging(Logs.LevelGenMain))
        {
            time = Time.realtimeSinceStartup;
            BigBoss.Debug.printHeader(Logs.LevelGenMain, "Registering Room Mods");
        }
        #endregion
        List<RoomModifier> modPrototypes = BigBoss.Types.GetInstantiations<RoomModifier>();
        foreach (RoomModType e in Enum.GetValues(typeof(RoomModType)))
        {
            Mods[(int)e] = ProbabilityPool<RoomModifier>.Create(Probability.LevelRand);
        }
        foreach (RoomModifier mod in modPrototypes)
        {
            Mods[(int)mod.GetType()].Add(mod);
        }
        initialized = true;
        #region Debug
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            foreach (RoomModType e in Enum.GetValues(typeof(RoomModType)))
            {
                Mods[(int)e].ToLog(Logs.LevelGenMain);
            }
            BigBoss.Debug.w(Logs.LevelGenMain, "Registering Room Modifiers took " + (Time.realtimeSinceStartup - time));
            BigBoss.Debug.printFooter(Logs.LevelGenMain);
        }
        #endregion
    }

    public static RoomModifier GetBase()
    {
        if (!initialized)
            RegisterModifiers();
        Mods[(int)RoomModType.Base].ClearSkipped();
        return Mods[(int)RoomModType.Base].Get();
    }

    public static List<RoomModifier> GetFlexible(int num)
    {
        if (!initialized)
            RegisterModifiers();
        Mods[(int)RoomModType.Flexible].ClearSkipped();
        return Mods[(int)RoomModType.Flexible].Get(num);
    }

    public static RoomModifier GetFinal()
    {
        if (!initialized)
            RegisterModifiers();
        Mods[(int)RoomModType.Final].ClearSkipped();
        return Mods[(int)RoomModType.Final].Get();
    }

    public override string ToString()
    {
        return GetName();
    }

    // Inherited Functions
    public abstract bool Modify(RoomSpec spec);

    public new abstract RoomModType GetType();

    public abstract string GetName();

    public override bool Equals(object obj)
    {
        RoomModifier rhs = obj as RoomModifier;
        if (rhs == null) return false;
        return GetName().Equals(rhs.GetName());
    }

    public override int GetHashCode()
    {
        return GetName().GetHashCode();
    }
}
