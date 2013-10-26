using System;
using System.Collections.Generic;
using UnityEngine;
using XML;

public class FieldMap : SortedDictionary<string, Field>
{
    public XMLNode node;

    public FieldMap()
    {
    }

    public FieldMap(XMLNode x)
    {
        this.node = x;
    }

    public T Add<T>(string name) where T : Field, new()
    {
        Field item;
        if (!this.TryGetValue(name, out item))
        {
            T param = new T();
            XMLNode child = node.Select(name);
            if (child != null)
                param.ParseXML(child, name);
            else
                param.SetDefault();
            this.Add(name, param);
            return param;
        }
        return (T)item;
    }
}
