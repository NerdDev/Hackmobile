using System;
using System.Collections.Generic;
using XML;

namespace XML
{
    public static class XMLNifty
    {
        public static int SelectInt(this XMLNode x, string toParse)
        {
            if (x != null)
            {
                return x.SelectInt(toParse);
            }
            return 0;
        }

        public static T SelectEnum<T>(this XMLNode x, string toParse)
        {
            if (x != null)
            {
                return x.SelectEnum<T>(toParse);
            }
            return default(T);
        }

        public static string SelectString(this XMLNode x, string node)
        {
            if (x != null)
            {
                return x.SelectString(node);
            }
            return "";
        }

        public static bool SelectBool(this XMLNode x, string toParse)
        {
            if (x != null)
            {
                return x.SelectBool(toParse);
            }
            return false;
        }

        public static XMLNode select(this XMLNode x, string p)
        {
            if (x != null)
            {
                return x.Select(p);
            }
            return new XMLNode();
        }

        public static void parseList(this XMLNode x, string node, Action<XMLNode> a)
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

        public static float SelectFloat(this XMLNode x, string toParse)
        {
            if (x != null)
            {
                return x.SelectFloat(toParse);
            }
            return 0;
        }

        public static double SelectDouble(this XMLNode x, string toParse)
        {
            if (x != null)
            {
                return x.SelectDouble(toParse);
            }
            return 0;
        }

        public static List<XMLNode> SelectList(this XMLNode x, string key)
        {
            if (x != null)
            {
                return x.selectList(key);
            }
            return new List<XMLNode>();
        }
    }
}
