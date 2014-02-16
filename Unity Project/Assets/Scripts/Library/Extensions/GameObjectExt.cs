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

    public static List<T> GetInterfacesInChildren<T>(this GameObject gObj, bool includeInactive = true)
    {
        return new List<T>(GetMonobehaviorsWithInstanceInChildren<T>(gObj, includeInactive).Cast<T>());
    }

    public static List<MonoBehaviour> GetMonobehaviorsWithInstanceInChildren<T>(this GameObject gObj, bool includeInactive = true)
    {
        if (!typeof(T).IsInterface) throw new SystemException("Specified type is not an interface!");
        List<MonoBehaviour> ret = new List<MonoBehaviour>();
        Type t = typeof(T);
        foreach (MonoBehaviour m in gObj.GetComponentsInChildren<MonoBehaviour>(includeInactive))
        {
            if (m != null && m.GetType().GetInterfaces().Contains(t))
            {
                ret.Add(m);
            }
        }
        return ret;
    }

    public static W GetWorldObject<W>(this GameObject gObj) where W : WorldObject
    {
        WOWrapper instance = gObj.GetComponent<WOWrapper>();
        return (W)instance.WO;
    }
}
