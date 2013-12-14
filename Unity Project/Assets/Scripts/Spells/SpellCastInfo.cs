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
        // GridSpace targets overlap with objects
        numLoc -= numObj;
        if (numLoc < 0)
            numLoc = 0;
        _targetSpaces = new List<GridSpace>(numLoc);
        _targetObjects = new List<IAffectable>(numObj);
    }
    public SpellCastInfo(IAffectable caster, SpellCastInfo rhs)
    {
        Caster = caster;
        _targetSpaces = new List<GridSpace>(rhs.TargetSpaces.Count);
        _targetObjects = new List<IAffectable>(rhs.TargetObjects.Count);
    }
    public IAffectable Caster { get; protected set; }
    private List<GridSpace> _targetSpaces;
    public List<GridSpace> TargetSpaces
    {
        get
        {
            if (_targetSpaces == null)
            { // If spaces not set
                List<GridSpace> derivedSpaces;
                if (_targetObjects != null)
                { // If we have target Objects, use their locations
                    HashSet<GridSpace> spaceSet = new HashSet<GridSpace>();
                    foreach (IAffectable obj in _targetObjects)
                    {
                        spaceSet.Add(obj.Self.GridSpace);
                    }
                    derivedSpaces = spaceSet.ToList();
                }
                else
                { // If no objects set either, use caster himself
                    derivedSpaces = new List<GridSpace>(new[] { Caster.Self.GridSpace });
                    _targetObjects = new List<IAffectable>(new[] { Caster });
                }
                _targetSpaces = derivedSpaces;
            }
            return _targetSpaces;
        }
        set { _targetSpaces = value; }
    }
    public GridSpace TargetSpace
    {
        get { return TargetSpaces.Random(Probability.Rand); }
        set { _targetSpaces = new List<GridSpace>(new [] { value }); }
    }
    private List<IAffectable> _targetObjects;
    public List<IAffectable> TargetObjects
    {
        get
        {
            if (_targetObjects == null)
            { // If spaces not set
                List<IAffectable> derivedObjects;
                if (_targetSpaces != null)
                { // If we have target spaces, use their objects
                    var objSet = new HashSet<IAffectable>();
                    foreach (GridSpace space in _targetSpaces)
                    {
                        objSet.Union(space.GetContained().OfType<IAffectable>());
                    }
                    derivedObjects = objSet.ToList();
                }
                else
                { // If no spaces set either, use caster himself
                    derivedObjects = new List<IAffectable>(new[] { Caster });
                    _targetSpaces = new List<GridSpace>(new[] { Caster.Self.GridSpace });
                }
                _targetObjects = derivedObjects;
            }
            return _targetObjects;
        }
        set { _targetObjects = value; }
    }
    public IAffectable TargetObject
    {
        get { return TargetObjects.Random(Probability.Rand); }
        set { _targetObjects = new List<IAffectable>(new[] { value }); }
    }
}
