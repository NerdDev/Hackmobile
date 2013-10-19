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
    public static Dictionary<Type, Dictionary<string, Type>> codexOfAllLife = new Dictionary<Type, Dictionary<string, Type>>();

    public void Initialize()
    {
        Harvest<EffectInstance>();
        Harvest<RoomModifier>();
        Harvest<ITargeter>();
    }

    protected void Harvest<T>()
    {
        Type type = typeof(T);
        List<Type> types = type.GetSubclassesOf();
        Dictionary<string, Type> dict = codexOfAllLife.GetCreate(type);
        foreach (Type t in types)
        {
            dict.Add(t.ToString(), t);
        }
    }

    public bool TryGetValue<T>(string key, out Type type)
    {
        Dictionary<string, Type> dict;
        if (codexOfAllLife.TryGetValue(typeof(T), out dict))
            return dict.TryGetValue(key, out type);
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
