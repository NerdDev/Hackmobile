using System;
using System.Collections.Generic;
using System.Linq;

namespace XML
{
    /**
     * The XML reader class for reading XML's into a tree map.
     */
    public class XMLReader
    {

        private XMLParser xml;
        XMLNode xnode;

        /**
         * @param path The path to the XML to read.
         */
        public XMLReader(string path)
        {
            xml = new XMLParser();
            xnode = xml.Parse(System.IO.File.ReadAllText(path));
        }

        /**
         * @param key The string key to match for the find.
         * 
         * @return The first Map with the matching key.
         */
        public XMLNode select(string key)
        {
            return xnode.Select(key);
        }

        /**
         * @return The root Map for the XML document.
         */
        public XMLNode getRoot()
        {
            return xnode;
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
            return children(xnode).Where(node => node.getKey().Equals(key)).ToList<XMLNode>();
        }
    }
}
