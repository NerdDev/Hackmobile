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

    public abstract void Modify(RoomMap room, RandomGen rand);

    public virtual bool ShouldSpawn(RoomMap room)
    {
        return true;
    }
}
