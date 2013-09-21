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

        internal static void parseList(XMLNode x, string node, Action<XMLNode> a)
        {
            if (x != null)
            {
                List<XMLNode> xprops = x.selectList(node);
                if (xprops != null)
                {
                    foreach (XMLNode xnode in xprops)
                    {
                        a(xnode);
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

        internal static double SelectDouble(XMLNode x, string toParse)
        {
            if (x != null)
            {
                return x.SelectDouble(toParse);
            }
            return 0;
        }

        internal static List<XMLNode> SelectList(XMLNode x, string key)
        {
            if (x != null)
            {
                return x.selectList(key);
            }
            return null;
        }
    }
}