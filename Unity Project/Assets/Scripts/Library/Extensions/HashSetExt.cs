using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Text;

public static class HashSetExt
{
    public static T[] ToArray<T>(this HashSet<T> set)
    {
        T[] arr = new T[set.Count];
        int i = 0;
        foreach (T element in set)
        {
            arr[i] = element;
            i++;
        }
        return arr;
    }
}
