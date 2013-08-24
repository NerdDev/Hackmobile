using UnityEngine;
using System.Collections;
using XML;

public class WorldObject : MonoBehaviour, PassesTurns
{

    #region Generic Object Properties (graphical info, names, etc).
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
	void Start () {
	
	}

    #region Data Management for Instances
    public virtual void setData(WorldObject wo)
    {
        this.Model = wo.Model;
        this.ModelTexture = wo.ModelTexture;
        this.Name = wo.Name;
        this.Prefab = wo.Prefab;
    }

    public virtual void parseXML(XMLNode x)
    {
        //name is handled in DataManager so we get the GameObject name
        this.Model = XMLNifty.SelectString(x, "model");
        this.ModelTexture = XMLNifty.SelectString(x, "modeltexture");
        this.Prefab = XMLNifty.SelectString(x, "prefab");
    }

    public virtual void setNull()
    {
        //these should be set to some type of model that we can tell shouldn't be ingame
        this.Model = "";
        this.ModelTexture = "";
        this.Name = "null";
        this.Prefab = "";
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
