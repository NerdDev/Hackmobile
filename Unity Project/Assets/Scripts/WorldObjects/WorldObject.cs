using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using XML;

public class WorldObject : MonoBehaviour, PassesTurns, FieldContainer
{
    #region Generic Object Properties (graphical info, names, etc).

    public FieldMap map;
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
    public virtual void setData(WorldObject wo)
    {
        if (map == null)
        {
            map = new FieldMap(wo.map.node);
        }
        this.SetParams();
    }

    public void parseXML(XMLNode x)
    {
        map = new FieldMap(x);
        this.SetParams();
    }

    public virtual void setNull()
    {
        this.parseXML(new XMLNode(null));
        IsActive = false;
    }

    public virtual void SetParams()
    {
        Name = map.Add<String>("name");
        Model = map.Add<String>("model");
        ModelTexture = map.Add<String>("modeltexture");
        Prefab = map.Add<String>("prefab");
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
