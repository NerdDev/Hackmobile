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

    public W InstantiateAndWrap(string str, GridSpace g, out T wrapper)
    {
        return InstantiateAndWrap(GetPrototype(str), g, out wrapper);
    }

    public W InstantiateAndWrap(string str, GridSpace g)
    {
        T wrapper;
        return InstantiateAndWrap(GetPrototype(str), g, out wrapper);
    }

    public W InstantiateRandom()
    {
        return Instantiate(prototypes.Values.Randomize(Probability.Rand).First());
    }

    public W InstantiateAndWrapRandom(GridSpace g)
    {
        T wrapper;
        return InstantiateAndWrapRandom(g, out wrapper);
    }

    public W InstantiateAndWrapRandom(GridSpace g, out T wrapper)
    {
        W obj = InstantiateRandom();
        wrapper = Wrap(obj, g);
        return obj;
    }

    public W InstantiateAndWrap(W proto, GridSpace g, out T wrapper)
    {
        W obj = Instantiate(proto);
        wrapper = Wrap(obj, g);
        return obj;
    }

    public W InstantiateAndWrap(W proto, GridSpace g)
    {
        T wrapper;
        return InstantiateAndWrap(proto, g, out wrapper);
    }

    public W Instantiate(W proto)
    {
        W newWorldObject = proto.Copy();
        newWorldObject.OnDestroy += Unregister;
        existing.Add(newWorldObject);
        if (newWorldObject is PassesTurns)
            BigBoss.Time.TotalObjectList.Add((PassesTurns)newWorldObject);
        newWorldObject.IsActive = true;
        return newWorldObject;
    }

    public T Wrap(W obj, GridSpace g)
    {
        GameObject gameObject = spawner.Instantiate(obj, g.X, g.Y);
        T wrapper = gameObject.AddComponent<T>();
        wrapper.SetTo(obj);
        obj.Init();
        return wrapper;
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
        obj.Init();
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
        if (obj is PassesTurns)
            BigBoss.Time.RemoveFromUpdateList((PassesTurns)obj);
    }
}
