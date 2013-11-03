using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WODictionary<W> : ObjectDictionary<W> where W : WorldObject, new()
{
    List<W> existing = new List<W>();
    List<W> wrapped = new List<W>();
    ObjectManager spawner;
    public IEnumerable<W> Existing { get { return existing; } }
    public IEnumerable<W> Wrapped { get { return wrapped; } }

    public WODictionary()
    {
        spawner = BigBoss.Objects;
    }

    public W Instantiate(string str)
    {
        return Instantiate(GetPrototype(str));
    }

    public W InstantiateAndWrap(string str, GridSpace g, out WOWrapper wrapper)
    {
        return InstantiateAndWrap(GetPrototype(str), g, out wrapper);
    }

    public W InstantiateAndWrap(string str, GridSpace g)
    {
        WOWrapper wrapper;
        return InstantiateAndWrap(GetPrototype(str), g, out wrapper);
    }

    public W InstantiateRandom()
    {
        return Instantiate(prototypes.Values.Randomize(Probability.Rand).First());
    }

    public W InstantiateAndWrapRandom(GridSpace g)
    {
        WOWrapper wrapper;
        return InstantiateAndWrapRandom(g, out wrapper);
    }

    public W InstantiateAndWrapRandom(GridSpace g, out WOWrapper wrapper)
    {
        W obj = InstantiateRandom();
        wrapper = Wrap(obj, g);
        return obj;
    }

    public W InstantiateAndWrap(W proto, GridSpace g, out WOWrapper wrapper)
    {
        W obj = Instantiate(proto);
        wrapper = Wrap(obj, g);
        return obj;
    }

    public W InstantiateAndWrap(W proto, GridSpace g)
    {
        WOWrapper wrapper;
        return InstantiateAndWrap(proto, g, out wrapper);
    }

    public W Instantiate(W proto)
    {
        W newWorldObject = proto.Copy();
        newWorldObject.OnDestroy += Unregister;
        existing.Add(newWorldObject);
        if (newWorldObject is PassesTurns)
            BigBoss.Time.updateList.Add((PassesTurns)newWorldObject);
        newWorldObject.Init();
        return newWorldObject;
    }

    public WOWrapper Wrap(W obj, GridSpace g)
    {
        GameObject gameObject = spawner.Instantiate(obj, g.X, g.Y);
        WOWrapper wrapper = gameObject.AddComponent<WOWrapper>();
        wrapper.SetTo(obj);
        return wrapper;
    }

    public void DestroyWrappers()
    {
        foreach (W w in wrapped)
            w.Unwrap();
        wrapped.Clear();
    }

    public void DestroyExisting()
    {
        foreach (W w in existing)
            w.Destroy();
        existing.Clear();
    }

    protected void Unregister(WorldObject obj)
    {
        existing.Remove((W)obj);
        if (obj is PassesTurns)
            BigBoss.Time.updateList.Add((PassesTurns)obj);
    }
}
