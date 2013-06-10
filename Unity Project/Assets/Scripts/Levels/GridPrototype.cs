using UnityEngine;
using System.Collections;

public class GridPrototype {

    string name;
    public GridType type { get; private set; }

    GridPrototype(string name, GridType typeIn)
    {
        this.name = name;
        type = typeIn;
    }
}
