using UnityEngine;
using System.Collections;

public class GridPrototype : MonoBehaviour, Grid
{

    public string name;
    public string asciiRep_;
    public GridType type_;

    GridPrototype(string name, string asciiRep, GridType typeIn)
    {
        this.name = name;
        if (asciiRep.Length > 1)
        {
            asciiRep = asciiRep.Substring(0, 1);
        }
        asciiRep_ = asciiRep;
        type_ = typeIn;
    }

    public GridType type
    {
        get
        {
            return type_;
        }

    }

    public string ascii
    {
        get
        {
            return asciiRep_;
        }

    }
}
