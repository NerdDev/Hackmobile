using System;
using UnityEngine;
using System.Collections.Generic;

public class Registerable<T> where T : new()
{
    public Dictionary<object, Action<T, T>> events = new Dictionary<object, Action<T, T>>();
    T item;

    public void Set(T to)
    {
        T old = item;
        item = to;
        if (events != null)
        {
            foreach (Action<T, T> action in events.Values)
            {
                action(old, item);
            }
        }
    }

    public T Get()
    {
        return item;
    }

    public void Register(object obj, Action<T, T> action)
    {
        events.Add(obj, action);
        foreach (Action<T, T> act in events.Values)
        {
            act(new T(), item);
        }
    }

    public bool Registered(object obj)
    {
        return events.ContainsKey(obj);
    }

    public void Unregister(object obj)
    {
        events.Remove(obj);
    }
}