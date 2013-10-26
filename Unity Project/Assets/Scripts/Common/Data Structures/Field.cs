using System;
using XML;

public interface Field
{
    void ParseXML(XMLNode x, string name);
    void SetDefault();
}
