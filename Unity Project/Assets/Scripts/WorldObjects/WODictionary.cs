using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WODictionary<W> where W : WorldObject
{
    public Dictionary<string, W> Prototypes { get; set; }
    Dictionary<string, List<W>> instantiated = new Dictionary<string, List<W>>();
    WorldObjectManager spawner;

    public WODictionary()
    {
        Prototypes = new Dictionary<string, W>();
        spawner = BigBoss.WorldObject;
    }

    public W Instantiate(string str, GridSpace g)
    {
        return Instantiate(Prototypes[str], g);
    }

    public W Instantiate(string str, GridSpace g, out WOInstance wrapper)
    {
        return Instantiate(Prototypes[str], g, out wrapper);
    }

    public W Instantiate(W proto, GridSpace g)
    {
        WOInstance wrapper;
        return Instantiate(proto, g, out wrapper);
    }

    public W InstantiateRandom(GridSpace g)
    {
        WOInstance wrapper;
        return InstantiateRandom(g, out wrapper);
    }

    public W InstantiateRandom(GridSpace g, out WOInstance wrapper)
    {
        return Instantiate(Prototypes.Values.Randomize(Probability.Rand).First(), g, out wrapper);
    }

    public W Instantiate(W proto, GridSpace g, out WOInstance wrapper)
    {
        GameObject gameObject = spawner.Instantiate(proto, g.X, g.Y);
        wrapper = gameObject.AddComponent<WOInstance>();
        W newWorldObject = wrapper.SetTo(proto);
        newWorldObject.Init();
        return newWorldObject;
    }
}
