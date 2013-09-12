using System;
using XML;

public interface Field
{
    void parseXML(XMLNode x, string name);
}