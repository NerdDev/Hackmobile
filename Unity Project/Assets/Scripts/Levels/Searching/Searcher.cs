using UnityEngine;
using System;
using System.Collections;

public class GridSearcher {
    
    protected System.Random _rand;
    public Func<Value2D<GridType>, bool> Filter;

    public GridSearcher()
        : this(new System.Random())
    {
    }

    public GridSearcher(System.Random rand)
    {
        this._rand = rand;
    }

    public void AddFilter(Func<Value2D<GridType>, bool> filter)
    {
        if (Filter == null)
            Filter = filter;
        else
            Filter += filter; // Delegates allow adding
    }
}
