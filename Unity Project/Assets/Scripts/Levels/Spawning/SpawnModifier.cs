using UnityEngine;
using System.Collections;

public abstract class SpawnMod : IProbabilityItem
{
    public virtual double Multiplier { get { return 1; } }
    public virtual bool Unique { get { return false; } }

    public override string ToString()
    {
        return GetType().Name;
    }

    public abstract bool Modify(SpawnSpec spec);

    public abstract bool ShouldSpawn(SpawnSpec spec);
}
