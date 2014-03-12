using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

public static class TypeExt
{
    public static bool IsSubclassOfGeneric(this Type toCheck, Type generic)
    {
        while (toCheck != null && toCheck != typeof(object))
        {
            var cur = toCheck.IsGenericType ? toCheck.GetGenericTypeDefinition() : toCheck;
            if (generic == cur)
            {
                return true;
            }
            toCheck = toCheck.BaseType;
        }
        return false;
    }
}
