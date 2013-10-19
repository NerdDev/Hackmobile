using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SpellCastInfo
{
    public SpellCastInfo(IAffectable caster)
    {
        Caster = caster;
    }
    public SpellCastInfo(List<SpellAspect> aspects)
    {
        int numLoc = 0;
        int numObj = 0;
        aspects.ForEach(aspect =>
            {
                if (aspect.Targeter.Style == TargetingStyle.TargetLocation)
                    numLoc = Math.Max(numLoc, aspect.Targeter.MaxTargets);
                if (aspect.Targeter.Style == TargetingStyle.TargetObject)
                    numObj = Math.Max(numObj, aspect.Targeter.MaxTargets);
            });
        // Location targets overlap with objects
        numLoc -= numObj;
        if (numLoc < 0)
            numLoc = 0;
        targetSpaces = new GridSpace[numLoc];
        targetObjects = new IAffectable[numObj];
    }
    public SpellCastInfo(IAffectable caster, SpellCastInfo rhs)
    {
        Caster = caster;
        targetSpaces = new GridSpace[rhs.TargetSpaces.Length];
        targetObjects = new IAffectable[rhs.TargetObjects.Length];
    }
    public IAffectable Caster { get; protected set; }
    private GridSpace[] targetSpaces;
    public GridSpace[] TargetSpaces
    {
        get
        {
            if (targetSpaces == null)
            { // If spaces not set
                GridSpace[] derivedSpaces;
                if (targetObjects != null)
                { // If we have target Objects, use their locations
                    HashSet<GridSpace> spaceSet = new HashSet<GridSpace>();
                    foreach (IAffectable obj in targetObjects)
                    {
                        spaceSet.Add(obj.Self.Location);
                    }
                    derivedSpaces = spaceSet.ToArray();
                }
                else
                { // If no objects set either, use caster himself
                    derivedSpaces = new GridSpace[] { Caster.Self.Location };
                    targetObjects = new IAffectable[] { Caster };
                }
                targetSpaces = derivedSpaces;
            }
            return targetSpaces;
        }
        set { targetSpaces = value; }
    }
    public GridSpace TargetSpace
    {
        get { return TargetSpaces.Random(Probability.Rand); }
        set { targetSpaces = new GridSpace[] { value }; }
    }
    private IAffectable[] targetObjects;
    public IAffectable[] TargetObjects
    {
        get
        {
            if (targetObjects == null)
            { // If spaces not set
                IAffectable[] derivedObjects;
                if (targetSpaces != null)
                { // If we have target spaces, use their objects
                    HashSet<IAffectable> objSet = new HashSet<IAffectable>();
                    foreach (GridSpace space in targetSpaces)
                    {
                        objSet.Union(space.GetContained().OfType<IAffectable>());
                    }
                    derivedObjects = objSet.ToArray();
                }
                else
                { // If no spaces set either, use caster himself
                    derivedObjects = new IAffectable[] { Caster };
                    targetSpaces = new GridSpace[] { Caster.Self.Location };
                }
                targetObjects = derivedObjects;
            }
            return targetObjects;
        }
        set { TargetObjects = value; }
    }
    public IAffectable TargetObject
    {
        get { return TargetObjects.Random(Probability.Rand); }
        set { targetObjects = new IAffectable[] { value }; }
    }
}
