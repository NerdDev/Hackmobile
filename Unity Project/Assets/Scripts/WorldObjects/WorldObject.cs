using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using XML;

public class WorldObject : MonoBehaviour, PassesTurns, IXmlParsable
{
    #region Generic Object Properties (graphical info, names, etc).
    public string Model { get; set; }
    public string ModelTexture { get; set; }
    public string Name { get; set; }
    public string Prefab { get; set; }
    public GridSpace Location { get; set; }  // Placeholder. Needs to actually be implemented and updated.

    public override string ToString()
    {
        return Name;
    }
    #endregion

    // Use this for initialization
    void Start()
    {
    }

    #region Data Management for Instances
    public virtual void setNull()
    {
        IsActive = false;
    }

    public virtual void ParseXML(XMLNode x)
    {
        Name = x.SelectString("name", "NONAME!");
        Model = x.SelectString("model");
        ModelTexture = x.SelectString("modeltexture");
        Prefab = x.SelectString("prefab");
    }
    #endregion

    #region Time Management

    int turnPoints = 0;
    int basePoints = 60;
    protected bool isActive = false;

    public virtual void UpdateTurn()
    {
        //do nothing atm
    }

    public virtual int CurrentPoints
    {
        get
        {
            return this.turnPoints;
        }
        set
        {
            this.turnPoints = value;
        }
    }

    public virtual int BasePoints
    {
        get
        {
            return this.basePoints;
        }
        set
        {
            this.basePoints = value;
        }
    }

    public virtual bool IsActive
    {
        get
        {
            return this.isActive;
        }
        set
        {
            this.isActive = value;
        }
    }

    #endregion
}
