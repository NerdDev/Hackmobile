using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using XML;

public class WorldObject : PassesTurns, IXmlParsable, INamed
{
    #region Generic Object Properties (graphical info, names, etc).
    private WOWrapper _instance; // Private member to allow for one-set-only logic
    public WOWrapper Instance { get { return _instance; } set { if (_instance == null) _instance = value; } }
    protected static uint nextID = 0;
    public uint ID { get; protected set; }
    public GameObject GO { get { return _instance.gameObject; } }
    public string Model;
    public string ModelTexture;
    public string Name { get; set; }
    private string _prefab;
    public virtual string Prefab { get { return _prefab; } set { _prefab = "Prefabs/" + value; } }
    public GridSpace GridSpace { 
        get { return _grid; }
        set { 
            if (_grid != null) _grid.Remove(this);
            value.Put(this);
            _grid = value; 
        } }
    //X, Y in integers, GridSpace ref
    private GridSpace _grid;
    public event Action<WorldObject> OnDestroy;

    public override string ToString()
    {
        return Name;
    }
    #endregion

    public WorldObject()
    {
        ID = nextID++;
    }

    // Use this for initialization
    public virtual void Start()
    {
    }

    public virtual void Update()
    {
    }

    public virtual void FixedUpdate()
    {
    }

    public virtual void OnClick()
    {
    }

    public virtual void Init()
    {
        IsActive = true;
        BigBoss.Time.RegisterToUpdateList(this);
    }

    public virtual void Destroy()
    {
        Unregister();
        Unwrap();
        if (GridSpace != null) GridSpace.Remove(this);
        if (OnDestroy != null)
            OnDestroy(this);
    }

    public void Unwrap()
    {
        if (Instance != null) //maybe the item isn't tied to a gameobject?
        {
            Instance.Destroy();
        }
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
    public bool isActive = false;

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
