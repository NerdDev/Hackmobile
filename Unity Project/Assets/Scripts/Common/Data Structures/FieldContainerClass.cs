using System;
using System.Collections.Generic;
using XML;

public class FieldContainerClass : FieldContainer, Field
{
    public FieldMap map;

    public void ParseXML(XMLNode x, string name)
    {
        map = new FieldMap(x.Select(name));
        if (map.node != null)
            this.SetParams();
    }

    public virtual void SetParams()
    {
    }

    public virtual void SetDefault()
    {
    }
}
