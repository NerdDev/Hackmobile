using UnityEngine;
using System.Collections;

public class WorldObject : UniqueObject {

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

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

    }

    #region Data Management for Instances
    public void setData(WorldObject wo)
    {
        this.Model = wo.Model;
        this.ModelTexture = wo.ModelTexture;
    }
    #endregion
}
