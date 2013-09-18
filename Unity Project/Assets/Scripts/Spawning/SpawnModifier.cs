using UnityEngine;
using System.Collections;

public abstract class SpawnModifier : ProbabilityItem {

    static ProbabilityPool<SpawnModifier> mods = new ProbabilityList<SpawnModifier>();

    public static void RegisterModifiers()
    {
        AddMod(new SpawnNPCs());
    }

    public static SpawnModifier GetMod()
    {
        return mods.Get();
    }

    private static void AddMod(SpawnModifier mod)
    {
        mods.Add(mod);
    }

    public int ProbabilityDiv()
    {
        return 1;
    }

    public bool IsUnique()
    {
        return false;
    }

    public override string ToString()
    {
        return GetType().Name;
    }

    public abstract void Modify(RandomGen rand, RoomMap room, params Keywords[] keywords);

    public abstract bool ShouldSpawn(RoomMap room, params Keywords[] keywords);
}
