using UnityEngine;
using System.Collections;

public abstract class SpawnModifier : ProbabilityItem
{
    public virtual double Multiplier { get { return 1; } }
    public virtual bool Unique { get { return false; } }

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

    public override string ToString()
    {
        return GetType().Name;
    }

    public abstract bool Modify(SpawnSpec spec);

    public abstract bool ShouldSpawn(SpawnSpec spec);
}
