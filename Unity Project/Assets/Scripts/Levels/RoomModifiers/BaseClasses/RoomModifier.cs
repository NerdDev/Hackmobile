using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

abstract public class RoomModifier : ProbabilityItem {

    static ProbabilityList<RoomModifier>[] mods = new ProbabilityList<RoomModifier>[Enum.GetNames(typeof(RoomModType)).Length];

    public static void RegisterModifiers()
    {
        #region Internal
        List<ProbabilityList<RoomModifier>> modsList = new List<ProbabilityList<RoomModifier>>();
        foreach (RoomModType e in Enum.GetValues(typeof(RoomModType)))
        {
            modsList.Add(new ProbabilityList<RoomModifier>());
        }
        #endregion Internal
        ProbabilityList<RoomModifier> baseMods = modsList[(int)RoomModType.Base];
        ProbabilityList<RoomModifier> flexMods = modsList[(int)RoomModType.Flexible];
        ProbabilityList<RoomModifier> finalMods = modsList[(int)RoomModType.Final];

        flexMods.Add(new PillarMod());

        #region Internal
        foreach (RoomModType e in Enum.GetValues(typeof(RoomModType)))
        {
            mods[(int)e] = modsList[(int)e];
        }
        #endregion Internal
    }

    public static RoomModifier GetBase()
    {
        return mods[(int)RoomModType.Base].Get();
    }

    public static List<RoomModifier> GetFlexible(int num)
    {
        return mods[(int)RoomModType.Base].Get();
    }

    // Inherited Functions
    public abstract void Modify(Room room, System.Random rand);

    public new abstract RoomModType GetType();

    public int ProbabilityDiv()
    {
        return 1;
    }
}
