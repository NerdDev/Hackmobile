using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WODictionary<W> : ObjectDictionary<W> where W : WorldObject, new()
{
    List<W> instantiated = new List<W>();
    ObjectManager spawner;
    public IEnumerable<W> Existing { get { return instantiated; } }

    public WODictionary()
    {
        spawner = BigBoss.Objects;
    }

    public W Instantiate(string str, GridSpace g)
    {
        return Instantiate(GetPrototype(str), g);
    }

    public W Instantiate(string str, GridSpace g, out WOInstance wrapper)
    {
        return Instantiate(GetPrototype(str), g, out wrapper);
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
        return Instantiate(prototypes.Values.Randomize(Probability.Rand).First(), g, out wrapper);
    }

    public W Instantiate(W proto, GridSpace g, out WOInstance wrapper)
    {
        GameObject gameObject = spawner.Instantiate(proto, g.X, g.Y);
        wrapper = gameObject.AddComponent<WOInstance>();
        W newWorldObject = wrapper.SetTo(proto);
        newWorldObject.OnDestroy += Unregister;
        instantiated.Add(newWorldObject);
        if (newWorldObject is PassesTurns)
            BigBoss.Time.updateList.Add((PassesTurns) newWorldObject);
        newWorldObject.Init();
        return newWorldObject;
    }

    protected void Unregister(WorldObject obj)
    {
        instantiated.Remove((W)obj);
        if (obj is PassesTurns)
            BigBoss.Time.updateList.Add((PassesTurns)obj);
    }
}
