using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WODictionary<W> where W : WorldObject
{
    Dictionary<string, W> prototypes = new Dictionary<string, W>();
    List<W> instantiated = new List<W>();
    WorldObjectManager spawner;
    public IEnumerable<W> Prototypes { get { return prototypes.Values; } }
    public IEnumerable<W> Existing { get { return instantiated; } }

    public WODictionary()
    {
        spawner = BigBoss.WorldObject;
    }

    public bool Add(W obj)
    {
        if (!prototypes.ContainsKey(obj.Name))
        {
            prototypes.Add(obj.Name, obj);
            return true;
        }
        return false;
    }

    public W GetPrototype(string str)
    {
        return prototypes[str];
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
        newWorldObject.Init();
        return newWorldObject;
    }

    protected void Unregister(WorldObject obj)
    {
        instantiated.Remove((W)obj);
    }
}
