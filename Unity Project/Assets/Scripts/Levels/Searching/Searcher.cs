using UnityEngine;
using System.Collections;

public class Searcher {
    
    protected System.Random _rand;
    public Searcher()
        : this(new System.Random())
    {
    }

    public Searcher(System.Random rand)
    {
        this._rand = rand;
    }
}
