using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public enum RoomModifierType
{
    Base,
    Heavy,
    Fill, 
    Decoration
}

abstract public class RoomModifier : IProbabilityItem
{
    public virtual double Multiplier { get { return 1; } }
    public virtual bool Unique { get { return false; } }
    public abstract RoomModifierType Type { get; }

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

    public void PlaceDoodads(RoomSpec spec, ThemeElementBundle doodad, IEnumerable<Point> points)
    {
        SmartThemeElement smart;
        doodad.Select(spec.Random, 1, 1, out smart, false);
        foreach (Point p in points)
        {
            GenDeploy deploy = new GenDeploy(smart.Get(spec.Random));
            spec.Grids.MergeIn(p.x, p.y, deploy, spec.Theme, GridType.Trap);
        }
    }

    public T EnsureThemeImplements<T>(RoomSpec spec)
        where T : class
    {
        T castTheme = spec.Theme as T;
        if (castTheme == null) throw new ArgumentException("Theme must be " + typeof(T).Name);
        return castTheme;
    }
}
