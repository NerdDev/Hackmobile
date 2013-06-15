using System;
using System.Collections;
using System.Collections.Generic;

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
         * @return The text associated with the node.
         */
        public string getText()
        {
            return text;
        }

        /**
         * @param text The string text to assign to the Node.
         */
        public void setText(string text)
        {
            this.text = text;
        }
    }
}