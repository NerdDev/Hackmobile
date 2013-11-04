using UnityEngine;
using System.Collections;
using System;
using System.Linq;
using System.Collections.Generic;

public static class GameObjectExt
{
    public static T[] GetInterfaces<T>(this GameObject gObj)
    {
        if (!typeof(T).IsInterface) throw new SystemException("Specified type is not an interface!");
        var mObjs = gObj.GetComponents<MonoBehaviour>();
        return (from a in mObjs where a.GetType().GetInterfaces().Any(k => k == typeof(T)) select (T)(object)a).ToArray();
    }

    public static T GetInterface<T>(this GameObject gObj)
    {
        if (!typeof(T).IsInterface) throw new SystemException("Specified type is not an interface!");
        return gObj.GetInterfaces<T>().FirstOrDefault();
    }

    public static T GetInterfaceInChildren<T>(this GameObject gObj)
    {
        if (!typeof(T).IsInterface) throw new SystemException("Specified type is not an interface!");
        return gObj.GetInterfacesInChildren<T>().FirstOrDefault();
    }

    public static List<T> GetInterfacesInChildren<T>(this GameObject gObj)
    {
        if (!typeof(T).IsInterface) throw new SystemException("Specified type is not an interface!");
        List<T> ret = new List<T>();
        Type t = typeof(T);
        foreach (MonoBehaviour m in gObj.GetComponentsInChildren<MonoBehaviour>())
            if (m != null)
                if (m.GetType().GetInterfaces().Contains(t))
                    ret.Add((T)(object)m);
        return ret;
    }

    public static W GetWorldObject<W>(this GameObject gObj) where W : WorldObject
    {
        WOWrapper instance = gObj.GetComponent<WOWrapper>();
        return (W)instance.WO;
    }

    public static object List { get; set; }
}
