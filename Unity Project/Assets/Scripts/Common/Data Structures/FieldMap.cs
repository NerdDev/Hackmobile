using System;
using System.Collections.Generic;
using UnityEngine;
using XML;

public class FieldMap : SortedDictionary<string, Field>
{
    public XMLNode x;

    public FieldMap()
    {
    }

    public FieldMap(XMLNode x)
    {
        this.x = x;
    }

    public T Add<T>(string name) where T : Field, new()
    {
        Field item;
        if (!this.TryGetValue(name, out item))
        {
            T param = new T();
            if (param is FieldContainer || param is EffectEvent)
            {
                //BigBoss.Log("Param is FieldContainer");
                //BigBoss.Log("T is: " + typeof(T).ToString());
                x = XMLNifty.select(x, name);
            }
            param.parseXML(x, name);
            this.Add(name, param);
            return param;
        }
        return (T)item;
    }
}