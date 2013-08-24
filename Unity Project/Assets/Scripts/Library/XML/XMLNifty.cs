using System;
using System.Collections.Generic;
using XML;

namespace XML
{
    public class XMLNifty
    {
        internal static int SelectInt(XMLNode x, string toParse)
        {
            if (x != null)
            {
                return x.SelectInt(toParse);
            }
            return 0;
        }

        internal static T SelectEnum<T>(XMLNode x, string toParse)
        {
            if (x != null)
            {
                return x.SelectEnum<T>(toParse);
            }
            return default(T);
        }

        internal static string SelectString(XMLNode x, string node)
        {
            if (x != null)
            {
                return x.SelectString(node);
            }
            return "";
        }

        internal static bool SelectBool(XMLNode x, string toParse)
        {
            if (x != null)
            {
                return x.SelectBool(toParse);
            }
            return false;
        }

        internal static XMLNode select(XMLNode x, string p)
        {
            if (x != null)
            {
                return x.select(p);
            }
            return new XMLNode();
        }

        internal static void parseList(XMLNode x, string topNode, string bottomNode, Action<XMLNode> a)
        {
            if (x != null)
            {
                XMLNode temp = x.select(topNode);
                if (temp != null)
                {
                    List<XMLNode> xprops = temp.selectList(bottomNode);
                    if (xprops != null)
                    {
                        foreach (XMLNode xnode in xprops)
                        {
                            a(xnode);
                        }
                    }
                }
            }
        }

        internal static float SelectFloat(XMLNode x, string toParse)
        {
            if (x != null)
            {
                return x.SelectFloat(toParse);
            }
            return 0;
        }
    }
}