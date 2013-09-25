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
            XMLNode xnode = this.x;
            param.parseXML(xnode, name);
            this.Add(name, param);
            return param;
        }
        return (T)item;
    }
}