using System;
using System.Collections.Generic;
using System.Linq;

namespace XML
{
    public class XMLNodeList : List<XMLNode>
    {
        //internal functions to imitate a stack
        internal XMLNode Pop()
        {
            XMLNode item = null;

            item = (XMLNode)this[this.Count - 1];
            this.Remove(item);

            return item;
        }

        internal int Push(XMLNode item)
        {
            Add(item);

            return this.Count;
        }
    }
}
