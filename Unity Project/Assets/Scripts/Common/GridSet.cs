using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridSet : HashSet<GridType> {

    public GridSet() : base()
    {
    }

    public GridSet(GridType t) : base()
    {
        Add(t);
    }

    public GridSet(GridType[] t) : base(t)
    {
    }

    public static implicit operator GridSet(GridType src)
    {
        return new GridSet(src);
    }

    public static implicit operator GridSet(GridType[] arr)
    {
        return new GridSet(arr);
    }
}
