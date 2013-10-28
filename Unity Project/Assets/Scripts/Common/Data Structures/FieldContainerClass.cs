using System;
using System.Collections.Generic;
using XML;

public class FieldContainerClass : FieldContainer, Field
{
    public FieldMap map;

    public void parseXML(XMLNode x, string name)
    {
        map = new FieldMap(x.select(name));
        this.SetParams();
    }

    public virtual void SetParams()
    {
    }
}
