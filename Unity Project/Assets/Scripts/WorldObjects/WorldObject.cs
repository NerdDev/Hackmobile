using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using XML;

public class WorldObject : MonoBehaviour, PassesTurns, FieldContainer
{
    #region Generic Object Properties (graphical info, names, etc).

    public FieldMap map;

    private string model;
    public string Model
    {
        get { return model; }
        set { this.model = value; }
    }
    private string modelTexture;
    public string ModelTexture
    {
        get { return modelTexture; }
        set { this.modelTexture = value; }
    }
    private string objectName;
    public string Name
    {
        get { return objectName; }
        set { this.objectName = value; }
    }
    private string prefab;
    public string Prefab
    {
        get { return prefab; }
        set { this.prefab = value; }
    }
    #endregion

    // Use this for initialization
	void Start ()
    {
	}

    #region Data Management for Instances
    public virtual void setData(WorldObject wo)
    {
        this.map = wo.map.Copy();
    }

    public virtual void parseXML(XMLNode x)
    {
        map = new FieldMap(x);
        this.SetParams();
    }

    public virtual void setNull()
    {
        this.parseXML(new XMLNode());
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
