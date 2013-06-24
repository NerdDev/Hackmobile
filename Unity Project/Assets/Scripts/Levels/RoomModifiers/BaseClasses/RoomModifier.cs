using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

abstract public class RoomModifier {

    static RoomModifier[][] modArr = new RoomModifier[Enum.GetNames(typeof(RoomModType)).Length][];

    public static void RegisterModifiers()
    {
        #region Internal
        List<List<RoomModifier>> mods = new List<List<RoomModifier>>();
        foreach (RoomModType e in Enum.GetValues(typeof(RoomModType)))
        {
            mods.Add(new List<RoomModifier>());
        }
        #endregion Internal
        List<RoomModifier> baseMods = mods[(int)RoomModType.Base];
        List<RoomModifier> flexMods = mods[(int)RoomModType.Flexible];
        List<RoomModifier> finalMods = mods[(int)RoomModType.Final];

        flexMods.Add(new PillarMod());

        #region Internal
        foreach (RoomModType e in Enum.GetValues(typeof(RoomModType)))
        {
            modArr[(int)e] = mods[(int)e].ToArray();
        }
        #endregion Internal
    }

    // Inherited Functions
    public abstract void Modify(Room room);

    public abstract RoomModType GetType();

    public virtual int GetProbabilityDivider()
    {
        return 1;
    }
}
