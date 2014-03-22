using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XML;

public class ObjectDictionary<O> where O : IXmlParsable, INamed, new()
{
    protected Dictionary<string, O> prototypes = new Dictionary<string, O>();
    public IEnumerable<O> Prototypes { get { return prototypes.Values; } }

    public bool Add(O obj)
    {
        if (!prototypes.ContainsKey(obj.Name))
        {
            prototypes.Add(obj.Name, obj);
            return true;
        }
        return false;
    }

    public O GetPrototype(string str)
    {
        O o;
        if (prototypes.TryGetValue(str, out o))
            return o;
        return default(O);
    }

    public O this[string str]
    {
        get { return prototypes[str]; }
    }

    public bool ContainsKey(string str)
    {
        if (prototypes.ContainsKey(str))
        {
            return true;
        }
        else
        {
            if (BigBoss.Debug.logging(Logs.XML))
            {
                BigBoss.Debug.w(Logs.XML, this.ToString() + " does not contain the string " + str + ".");
            }
            return false;
        }
    }
    public virtual void Parse(XMLNode baseNode)
    {
        foreach (XMLNode node in baseNode)
        {
            O o = new O();
            o.ParseXML(node);
            if (!Add(o) && BigBoss.Debug.logging(Logs.XML))
            {
                BigBoss.Debug.w(Logs.XML, o.GetType().Name + " already existed with name: " + o.Name + " under node " + node);
            }
        }
    }
}
