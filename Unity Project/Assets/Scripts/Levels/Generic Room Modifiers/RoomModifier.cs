using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

abstract public class RoomModifier : IProbabilityItem
{
    public virtual double Multiplier { get { return 1; } }
    public virtual bool Unique { get { return false; } }

    public RoomModifier()
    {
    }

    public override string ToString()
    {
        return GetType().Name;
    }

    public bool Modify(RoomSpec spec)
    {
        return ModifyInternal(spec);
    }

    protected abstract bool ModifyInternal(RoomSpec spec);

    public override bool Equals(object obj)
    {
        RoomModifier rhs = obj as RoomModifier;
        if (rhs == null) return false;
        return GetType().Equals(rhs.GetType());
    }

    public override int GetHashCode()
    {
        return GetType().GetHashCode();
    }
}
