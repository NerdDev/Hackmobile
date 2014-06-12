using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class WODictionary<W, T> : ObjectDictionary<W> where W : WorldObject, new() where T : WOWrapper
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

    public bool InstantiateAndWrap(string str, GridSpace g, out T wrapper, out W obj)
    {
        return InstantiateAndWrap(GetPrototype(str), g, out wrapper, out obj);
    }

    public bool InstantiateAndWrap(string str, GridSpace g, out W obj)
    {
        T wrapper;
        return InstantiateAndWrap(GetPrototype(str), g, out wrapper, out obj);
    }

    public W InstantiateRandom()
    {
        return Instantiate(prototypes.Values.Randomize(Probability.Rand).First());
    }

    public bool InstantiateAndWrapRandom(GridSpace g, out W obj)
    {
        T wrapper;
        return InstantiateAndWrapRandom(g, out wrapper, out obj);
    }

    public bool InstantiateAndWrapRandom(GridSpace g, out T wrapper, out W obj)
    {
        obj = InstantiateRandom();
        return Wrap(obj, g, out wrapper);
    }

    public bool InstantiateAndWrap(W proto, GridSpace g, out T wrapper, out W obj)
    {
        obj = Instantiate(proto);
        return Wrap(obj, g, out wrapper);
    }

    public bool InstantiateAndWrap(W proto, GridSpace g, out W obj)
    {
        T wrapper;
        return InstantiateAndWrap(proto, g, out wrapper, out obj);
    }

    public W Instantiate(W proto)
    {
        W newWorldObject = proto.Copy();
        newWorldObject.OnDestroy += Unregister;
        existing.Add(newWorldObject);
        if (newWorldObject is PassesTurns)
            BigBoss.Time.Register((PassesTurns)newWorldObject);
        newWorldObject.IsActive = true;
        return newWorldObject;
    }

    public bool Wrap(W obj, GridSpace g, out T wrapper)
    {
        GameObject gameObject;
        if (!spawner.Instantiate(obj, g.X, g.Y, out gameObject))
        {
            wrapper = null;
            return false;
        }
        wrapper = gameObject.AddComponent<T>();
        wrapper.SetTo(obj);
        obj.Init();
        return true;
    }

    public T Wrap(W obj, Transform parent)
    {
        GameObject gameObject = spawner.Instantiate(obj);
        gameObject.transform.parent = parent;
        T wrapper = gameObject.AddComponent<T>();
        wrapper.SetTo(obj);
        spawner.ResetObj(obj);
        obj.Init();
        return wrapper;
    }

    public T WrapEquipment(W obj, BoneStructure parent)
    {
        GameObject gameObject = spawner.Instantiate(obj);
        GameObject holder = new GameObject(obj.Prefab);
        holder.transform.parent = parent.transform;
        List<GameObject> objects = parent.AddEquipment(gameObject);
        foreach (GameObject go in objects)
        {
            go.transform.parent = holder.transform;
        }
        WeaponAnimations animations = gameObject.GetComponent<WeaponAnimations>();
        if (animations != null) holder.AddComponent<WeaponAnimations>().CopyInto(animations);
        GameObject.Destroy(gameObject);

        T wrapper = holder.AddComponent<T>();
        wrapper.SetTo(obj);
        return wrapper;
    }

    public T Wrap(W obj)
    {
        GameObject gameObject = spawner.Instantiate(obj);
        T wrapper = gameObject.AddComponent<T>();
        wrapper.SetTo(obj);
        obj.Init();
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
    }
}
