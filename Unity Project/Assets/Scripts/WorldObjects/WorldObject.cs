using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using XML;

public class WorldObject : PassesTurns, IXmlParsable
{
    #region Generic Object Properties (graphical info, names, etc).
    private WOInstance _instance; // Private member to allow for one-set-only logic
    public WOInstance Instance { get { return _instance; } set { if (_instance == null) _instance = value; } }
    public GameObject GO { get { return _instance.gameObject; } }
    public string Model { get; set; }
    public string ModelTexture { get; set; }
    public string Name { get; set; }
    public string Prefab { get; set; }
    public GridSpace Location { get; set; }  // Placeholder. Needs to actually be implemented and updated.
    public event Action<WorldObject> OnDestroy;

    public override string ToString()
    {
        return Name;
    }
    #endregion

    // Use this for initialization
    void Start()
    {
    }

    public virtual void Init()
    {
        IsActive = true;
        BigBoss.Time.RemoveFromUpdateList(this);
    }

    public virtual void Destroy()
    {
        Unregister();
        BigBoss.Destroy(GO);
        if (OnDestroy != null)
            OnDestroy(this);
    }

    protected virtual void Unregister()
    {
        BigBoss.Time.RemoveFromUpdateList(this);
    }

    #region Data Management for Instances
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
