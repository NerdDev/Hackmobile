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

    public static T GetInterfaceInChildren<T>(this MonoBehaviour gObj)
    {
        if (!typeof(T).IsInterface) throw new SystemException("Specified type is not an interface!");
        return gObj.GetInterfacesInChildren<T>().FirstOrDefault();
    }

    public static List<T> GetInterfacesInChildren<T>(this MonoBehaviour gObj, bool includeInactive = true)
    {
        return new List<T>(GetMonobehaviorsWithInstanceInChildren<T>(gObj, includeInactive).Cast<T>());
    }

    public static List<MonoBehaviour> GetMonobehaviorsWithInstanceInChildren<T>(this MonoBehaviour gObj, bool includeInactive = true)
    {
        if (!typeof(T).IsInterface) throw new SystemException("Specified type is not an interface!");
        List<MonoBehaviour> ret = new List<MonoBehaviour>();
        Type t = typeof(T);
        MonoBehaviour[] behaviours =  gObj.GetComponentsInChildren<MonoBehaviour>(includeInactive);
        foreach (MonoBehaviour m in behaviours)
        {
            if (m != null && m.GetType().GetInterfaces().Contains(t))
            {
                ret.Add(m);
            }
        }
        return ret;
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
        MonoBehaviour[] behaviours = gObj.GetComponentsInChildren<MonoBehaviour>(includeInactive);
        foreach (MonoBehaviour m in behaviours)
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

    public static void MoveStepWise(this GameObject obj, Vector3 target, float speed)
    {
        Vector3 heading = new Vector3(target.x - obj.transform.position.x, 0f, target.z - obj.transform.position.z);
        obj.transform.Translate(Vector3.forward * speed * Time.deltaTime, Space.Self);
        Quaternion toRot = Quaternion.LookRotation(heading);
        obj.transform.rotation = toRot;
    }

    public static void MoveStepWise(this CharacterController obj, Vector3 target, float speed)
    {
        Vector3 heading = new Vector3(target.x - obj.transform.position.x, 0f, target.z - obj.transform.position.z);
        obj.Move(heading.normalized * speed * Time.deltaTime);
        Quaternion toRot = Quaternion.LookRotation(heading);
        obj.transform.rotation = toRot;
    }

    public static void MoveStepWise(this Rigidbody obj, Vector3 target, float speed)
    {
        Vector3 heading = new Vector3(target.x - obj.transform.position.x, 0f, target.z - obj.transform.position.z);
        obj.velocity = (heading.normalized * speed);
        Quaternion toRot = Quaternion.LookRotation(heading);
        obj.MoveRotation(toRot);
    }
}
