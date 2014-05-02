using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class WorldObject : PassesTurns, IXmlParsable, INamed, ICopyable
{
    #region Generic Object Properties (graphical info, names, etc).
    private WOWrapper _instance; // Private member to allow for one-set-only logic
    public WOWrapper Instance { get { return _instance; } set { if (_instance == null) _instance = value; } }
    public static int NextID = 0;
    public int ID { get; protected set; }
    public GameObject GO { get { return _instance.gameObject; } }
    public string Name { get; set; }
    private string _prefab;
    public virtual string Prefab { get { return _prefab; } set { _prefab = "Prefabs/" + value; } }
    public GridSpace GridSpace
    {
        get { return _grid; }
        set
        {
            if (_grid != null)
            {
                _grid.Remove(this);
                if (!System.Object.ReferenceEquals(_grid.Level, value.Level))
                { // Transitioning to a new level.
                    _grid.Level.WorldObjects.Remove(this);
                }
            }
            value.Level.WorldObjects.Add(this);
            value.Put(this);
            _grid = value;
        }
    }
    private GridSpace _grid;
    public event Action<WorldObject> OnDestroy;
    public virtual Vector3 CanSeePosition
    {
        get
        {
            return this.GO.transform.position;
        }
    }

    public override string ToString()
    {
        return Name;
    }
    #endregion

    public WorldObject()
    {
        ID = NextID++;
        BasePoints = 60;
    }

    public void PostPrimitiveCopy()
    {
        ID = NextID++;
    }

    public void PostObjectCopy()
    {
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
        if (GridSpace != null)
        {
            GridSpace.Remove(this);
            GridSpace.Level.WorldObjects.Remove(this);
        }
        if (OnDestroy != null)
            OnDestroy(this);
    }

    public virtual void JustDestroy()
    {
        Unwrap();

    }

    public virtual void JustUnregister()
    {
        Unregister();
        if (GridSpace != null) GridSpace.Remove(this);
        if (OnDestroy != null)
            OnDestroy(this);
    }

    public void Unwrap()
    {
        if (Instance != null) //maybe the item isn't tied to a gameobject?
        {
            Instance.Destroy();
            _instance = null;
        }
    }

    public virtual void Wrap()
    {
    }

    public void Wrap(bool wrap)
    {
        if (wrap)
        {
            this.Wrap();
        }
        else
        {
            Unwrap();
        }
    }

    protected virtual void Unregister()
    {
        IsActive = false;
        BigBoss.Time.RemoveFromUpdateList(this);
    }

    #region XML
    public virtual void ParseXML(XMLNode x)
    {
        Name = x.SelectString("name", "NONAME!");
        Prefab = x.SelectString("prefab");
    }
    #endregion

    #region Time Management
    ulong basePoints = 60;

    public virtual void UpdateTurn()
    {
        //do nothing atm
    }

    public virtual ulong CurrentPoints { get; set; }

    public virtual ulong BasePoints { get; set; }

    public virtual bool IsActive { get; set; }
    #endregion

    #region Members
    public override bool Equals(object obj)
    {
        WorldObject rhs = obj as WorldObject;
        if (rhs == null) return false;
        return ID == rhs.ID;
    }

    public override int GetHashCode()
    {
        return ID.GetHashCode();
    }
    #endregion
}
