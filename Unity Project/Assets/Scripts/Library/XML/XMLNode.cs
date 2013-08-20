using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace XML
{
    public class XMLNode
    {
        public string key;
        public string text;
        public XMLNodeList nodeList;

        /**
        * @param key The string key to make for the XMLNode.
        */
        public XMLNode(string key)
        {
            this.key = key;
            nodeList = null;
        }

        /**
         * For creating a blank node.
         */
        public XMLNode()
        {
        }

        /**
         * @param key The string key to set for the XMLNode.
         */
        public void setKey(string key)
        {
            this.key = key;
        }

        /**
         * @return The string key associated with the XMLNode.
         */
        public string getKey()
        {
            return key;
        }

        /**
         * @retun The List of XMLNodes linked on to the XMLNode. If the existing list is null, then it creates a new empty list.
         */
        public XMLNodeList get()
        {
            if (nodeList != null)
            {
                return nodeList;
            }
            else
            {
                nodeList = new XMLNodeList();
                return nodeList;
            }
        }

        /**
         * @param m Adds the XMLNode onto the List of XMLNodes linked to the root instance.
         */
        public void add(XMLNode m)
        {
            if (nodeList != null)
            {
                nodeList.Add(m);
            }
            else
            {
                nodeList = new XMLNodeList();
                nodeList.Add(m);
            }
        }

        /**
         * Sets the list of XMLNodes to null, if marking that it is an ending list.
         */
        public void noValue()
        {
            if (nodeList != null)
            {
                nodeList = null;
            }
        }

        /**
         * @param f The function to use in finding.
         * 
         * A function of XMLNode, boolean.
         * Example Usage: XMLNode.find(p => p.getKey().Equals(key));
         * 
         * @return The first XMLNode matching the function.
         */
        public XMLNode find(Func<XMLNode, bool> f)
        {
            if (nodeList != null)
            {
                foreach (XMLNode XMLNode in nodeList)
                {
                    if (f(XMLNode))
                    {
                        return XMLNode;
                    }
                    else
                    {
                        XMLNode testXMLNode = XMLNode.find(f);
                        if (testXMLNode != null)
                            return testXMLNode;
                    }
                }
            }
            return null;
        }

        /**
         * @param key The string key to match for the find.
         * 
         * @return The first XMLNode with the matching key.
         */
        public XMLNode select(string key)
        {
            return this.find(p => p.getKey().Equals(key));
        }

        /**
             * @param m The root map to grab the children off of.
             * 
             * A non-recursive solution to searching for a specific node.
             * 
             * Usage: children(Map m).Where(func(Map, bool));
             * For key matching, use as the function - (p => p.getKey().Equals(someString);
             * 
             * @returns A list of the maps that match the function.
             */
        public IEnumerable<XMLNode> children(XMLNode m)
        {
            Stack<XMLNode> maps = new Stack<XMLNode>(new[] { m });
            while (maps.Any())
            {
                XMLNode map = maps.Pop();
                yield return map;
                foreach (XMLNode funmap in map.get())
                {
                    maps.Push(funmap);
                }
            }
        }

        /**
         * @param key The string key to match for the List.
         * 
         * A non-recursive solution to searching for specific nodes.
         * 
         * Implements the children(XMLNode xnode) function in an easier to read manner
         *  to return a list of nodes matching the key.
         *  
         * @return The list of XMLNodes associated with the key.
         */
        public List<XMLNode> selectList(string key)
        {
            return children(this).Where(node => node.getKey().Equals(key)).ToList<XMLNode>();
        }

        /**
         * @return The text associated with the node.
         */
        public string getText()
        {
            if (text != null)
            {
                return text;
            }
            else
            {
                return null;
            }
        }

        /**
         * @param text The string text to assign to the Node.
         */
        public void setText(string text)
        {
            this.text = text;
        }

        public int SelectInt(string toParse)
        {
            XMLNode x = this.select(toParse);
            if (x != null)
            {
                string sel = x.getText();
                if (sel != null)
                {
                    return sel.ToInt();
                }
            }
            return 0;
        }

        public double SelectDouble(string toParse)
        {
            XMLNode x = this.select(toParse);
            if (x != null)
            {
                string sel = x.getText();
                if (sel != null)
                {
                    return sel.ToDouble();
                }
            }
            return 0;
        }

        public float SelectFloat(string toParse)
        {
            XMLNode x = this.select(toParse);
            if (x != null)
            {
                string sel = x.getText();
                if (sel != null)
                {
                    return sel.ToFloat();
                }
            }
            return 0;
        }

        public T SelectEnum<T>(string toParse)
        {
            XMLNode x = this.select(toParse);
            if (x != null)
            {
                string sel = x.getText();
                if (sel != null)
                {
                    return (T)Enum.Parse(typeof(T), sel, true);
                }
            }
            return default(T);
        }

        public string SelectString(string node)
        {
            XMLNode x = this.select(node);
            if (x != null)
            {
                string sel = x.getText();
                if (sel != null)
                {
                    return sel;
                }
            }
            return "";
        }

        public bool SelectBool(string toParse)
        {
            XMLNode x = this.select(toParse);
            if (x != null)
            {
                string sel = x.getText();
                if (sel != null)
                {
                    return sel.ToBool();
                }
            }
            return false;
        }
    }
}