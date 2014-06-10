using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SpellCastInfo
{
    public SpellCastInfo(IAffectable caster)
    {
        Caster = caster;
    }
    public SpellCastInfo(List<Targeter> aspects)
    {
        int numLoc = 0;
        //int numGrid = 0;
        int numObj = 0;
        aspects.ForEach(aspect =>
        {
            //if (aspect.Style == TargetingStyle.TargetGrid)
            //    numGrid = Math.Max(numGrid, aspect.MaxTargets);
            if (aspect.Style == TargetingStyle.TargetObject)
                numObj = Math.Max(numObj, aspect.MaxTargets);
            if (aspect.Style == TargetingStyle.TargetLocation)
                numLoc = Math.Max(numLoc, aspect.MaxTargets);
        });
        // GridSpace targets overlap with objects
        //numGrid -= numObj;
        //if (numGrid < 0)
        //    numGrid = 0;
        //TargetSpaces = new GridSpace[numGrid];
        TargetObjects = new IAffectable[numObj];
        TargetLocations = new Vector3[numLoc];
    }
    public SpellCastInfo(IAffectable caster, SpellCastInfo rhs)
    {
        Caster = caster;
        _targetObjects = new IAffectable[rhs.TargetObjects.Length];
        _targetLocations = new Vector3[rhs.TargetLocations.Length];
    }
    public IAffectable Caster { get; protected set; }


    private IAffectable[] _targetObjects;
    public IAffectable[] TargetObjects
    {
        get
        {
            if (_targetObjects == null)
            { // If spaces not set
                _targetObjects = new IAffectable[0];
            }
            return _targetObjects;
        }
        set { _targetObjects = value; }
    }
    public IAffectable TargetObject
    {
        get
        {
            return TargetObjects.Random(Probability.Rand);
        }
        set { _targetObjects = new IAffectable[] { value }; }
    }

    private Vector3[] _targetLocations;
    public Vector3[] TargetLocations
    {
        get
        {
            if (_targetLocations == null)
            { // If spaces not set
                Vector3[] derivedSpaces;
                if (_targetObjects != null)
                { // If we have target Objects, use their locations
                    HashSet<Vector3> objSet = new HashSet<Vector3>();
                    foreach (IAffectable obj in _targetObjects)
                    {
                        objSet.Add(obj.Self.GO.transform.position);
                    }
                    derivedSpaces = objSet.ToArray();
                }
                else
                { // If no objects set either, use caster himself
                    derivedSpaces = new Vector3[] { Caster.Self.GO.transform.position };
                    _targetObjects = new IAffectable[] { Caster };
                }
                _targetLocations = derivedSpaces;
            }
            return _targetLocations;
        }
        set { _targetLocations = value; }
    }
    public Vector3 TargetLocation
    {
        get { return TargetLocations.Random(Probability.Rand); }
        set { _targetLocations = new Vector3[] { value }; }
    }
}
