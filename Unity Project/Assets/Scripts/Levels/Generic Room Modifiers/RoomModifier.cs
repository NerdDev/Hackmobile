using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

abstract public class RoomModifier : ProbabilityItem
{
    public virtual double Multiplier { get { return 1; } }
    public virtual bool Unique { get { return false; } }
    public abstract string Name { get; }

    public RoomModifier()
    {
    }

    public override string ToString()
    {
        return Name;
    }

    // Inherited Functions
    public abstract bool Modify(RoomSpec spec);

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
