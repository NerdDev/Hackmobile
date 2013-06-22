using UnityEngine;
using System.Collections;

public class WorldObject : UniqueObject
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
    private new string name;
    public string Name
    {
        get { return name; }
        set { this.name = value; }
    }
    #endregion

    // Use this for initialization
	void Start () {
	
	}

    #region Data Management for Instances
    public void setData(WorldObject wo)
    {
        this.Model = wo.Model;
        this.ModelTexture = wo.ModelTexture;
        this.Name = wo.Name;
    }

    public virtual void setNull()
    {
        //these should be set to some type of model that we can tell shouldn't be ingame
        this.Model = "";
        this.ModelTexture = "";
        this.Name = "null";
    }
    #endregion
}
