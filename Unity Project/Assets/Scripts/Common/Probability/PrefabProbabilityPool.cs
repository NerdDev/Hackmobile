using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class PrefabProbabilityPool<T> : ProbabilityPool<T>, IInitializable
    where T : UnityEngine.Component
{
    private ProbabilityPool<T> _pool;
    public PrefabProbabilityContainer[] Elements;

    public void Init()
    {
        _pool = ProbabilityPool<T>.Create();
        foreach (PrefabProbabilityContainer cont in Elements)
        {
            if (cont.Item == null)
            {
                throw new ArgumentException("Prefab has to be not null");
            }
            T t = cont.Item.GetComponentInChildren<T>();
            if (t == null)
            {
                throw new ArgumentException("Prefab of type " + cont.Item.GetType() + " has to be of type " + typeof(T));
            }
            _pool.Add(t, cont.Multiplier, cont.Unique);
        }
    }

    #region Probability Pool
    public override int Count
    {
        get { return _pool.Count; }
    }

    public override void Add(T item, double multiplier, bool unique)
    {
        _pool.Add(item, multiplier, unique);
    }

    public override int Remove(T item, bool all)
    {
        return _pool.Remove(item, all);
    }

    public override void AddAll(ProbabilityPool<T> rhs)
    {
        _pool.AddAll(rhs);
    }

    public override bool Get(System.Random random, out T item)
    {
        return _pool.Get(random, out item);
    }

    public override ProbabilityPool<T> Filter(Func<T, bool> filter)
    {
        return _pool.Filter(filter);
    }

    public override void Freshen()
    {
        _pool.Freshen();
    }

    public override void ToLog(Log log, string name = "")
    {
        _pool.ToLog(log, name);
    }

    public override IEnumerator<ProbabilityItem<T>> GetEnumerator()
    {
        return _pool.GetEnumerator();
    }

    public override bool Take(System.Random random, out T item)
    {
        return _pool.Take(random, out item);
    }

    public override void BeginTaking()
    {
        _pool.BeginTaking();
    }

    public override void EndTaking()
    {
        _pool.EndTaking();
    }
    #endregion
}
