using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*
 *  This manager automates managing a list of all subclasses manually. It keeps track of existing subclasses of any
 *  given type, to be queried via name later.
 *  
 *  For example, after harvesting EffectInstance, the EffectInstance dictionary will have the types of
 *  all EffectInstance subclasses, such as PoisonEffect, LevitationEffect, etc all stored by their names.
 *  
 */
public class TypeManager : MonoBehaviour, IManager
{
    public bool Initialized { get; set; }
    public static Dictionary<Type, Dictionary<string, Type>> codexOfAllLife = new Dictionary<Type, Dictionary<string, Type>>();

    public void Initialize()
    {
        BigBoss.Debug.w(Logs.TypeHarvest, "Initializing");
        Harvest<EffectInstance>();
        Harvest<RoomModifier>();
        Harvest<AIDecision>();
        Harvest<ITargeter>();
    }

    protected void Harvest<T>()
    {
        Type type = typeof(T);
        BigBoss.Debug.w(Logs.TypeHarvest, "Harvesting " + type.Name);
        List<Type> types = type.GetSubclassesOf();
        Dictionary<string, Type> dict;
        if (!codexOfAllLife.TryGetValue(type, out dict))
        {
            dict = new Dictionary<string, Type>();
            codexOfAllLife.Add(type, dict);
        }
        foreach (Type t in types)
        {
            BigBoss.Debug.w(Logs.TypeHarvest, "  " + t.Name);
            dict.Add(t.ToString().ToUpper(), t);
        }
    }

    public List<Type> GetSubclasses<T>()
    {
        List<Type> ret = new List<Type>();
        Dictionary<string, Type> dict;
        if (codexOfAllLife.TryGetValue(typeof(T), out dict))
            foreach (Type t in dict.Values)
                ret.Add(t);
        return ret;
    }

    public List<T> GetInstantiations<T>()
    {
        List<T> ret = new List<T>();
        foreach (Type t in GetSubclasses<T>())
            ret.Add((T)Activator.CreateInstance(t));
        return ret;
    }

    public bool TryGetValue<T>(string key, out Type type)
    {
        Dictionary<string, Type> dict;
        if (codexOfAllLife.TryGetValue(typeof(T), out dict))
            return dict.TryGetValue(key.ToUpper(), out type);
        type = null;
        return false;
    }

    public Type Get<T>(string key)
    {
        Type ret;
        TryGetValue<T>(key, out ret);
        return ret;
    }

    public T Instantiate<T>(string key)
    {
        T item;
        TryInstantiate(key, out item);
        return item;
    }

    public bool TryInstantiate<T>(string key, out T item)
    {
        Type t;
        if (TryGetValue<T>(key, out t))
        {
            item = (T)Activator.CreateInstance(Get<T>(key));
            return true;
        }
        item = default(T);
        return false;
    }
}
