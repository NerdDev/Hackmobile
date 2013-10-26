using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace XML
{
    public class XMLNode : IEnumerable<XMLNode>
    {
        #region Constant Chars
        const int MaxDepth = 25;
        const char LEFTBRACE = '<';
        const char RIGHTBRACE = '>';
        const char SLASH = '/';
        const char QUOTE = '"';
        const char EQUALS = '=';
        const string ENDNODE = "</";
        const string COMMENT = "<!--";
        const string ENDCOMMENT = "-->";
        #endregion

        public string Key { get; set; }
        public List<XMLNode> Children { get; protected set; }
        public XMLNode Parent { get; protected set; }
        public string Name { get { return SelectString("name"); } }
        public string Content { get; set; }
        public int Depth { get; set; }

        public XMLNode(XMLNode parent)
        {
            Children = new List<XMLNode>();
            Parent = parent;
            Content = "";
            if (parent == null)
                Depth = 0;
            else
                Depth = parent.Depth + 1;
        }

        #region Parsing
        public bool Parse(string str)
        {
            return Parse(new StringStream(str));
        }

        public bool Parse(StringStream stream)
        {
            try
            {
                stream.ExtractChar(); //remove startbrace
                // Get Name
                Key = stream.ExtractUntil(false, '\n', '\t', ' ', RIGHTBRACE);
                // Parse content
                if (ParseAttributes(stream))
                    ParseContent(stream);
                return true;
            }
            catch (Exception ex)
            {
                BigBoss.Debug.w(Logs.XML, "Exception while parsing node: " + this + " " + ex);
                return false;
            }
        }

        protected void ParseContent(StringStream stream)
        {
            if (stream.GetChar() == LEFTBRACE)
            { // Node content
                if (stream.IsNext(ENDNODE))
                { // End node
                    stream.ExtractUntil(true, RIGHTBRACE); // Skip to end
                    return;
                }
                else if (stream.IsNext(COMMENT))
                { // Comment
                    stream.ExtractUntil(ENDCOMMENT, true);
                }
                else
                {
                    XMLNode subNode = new XMLNode(this);
                    if (!subNode.Parse(stream))
                    { // Error parsing.  Stop
                        return;
                    }
                    Add(subNode);
                }
            }
            else
            { // Text content
                Content = stream.ExtractUntil(false, LEFTBRACE);
            }
            // Parse rest of content
            ParseContent(stream);
        }

        /*
         * Returns whether node has sub-content
         */
        protected bool ParseAttributes(StringStream stream)
        {
            string str = stream.ExtractUntil(false, EQUALS, SLASH, RIGHTBRACE);
            switch (stream.ExtractChar())
            {
                // An attribute
                case EQUALS:
                    // Treating attributes as subnodes with text content
                    XMLNode attr = new XMLNode(this);
                    attr.Key = str;
                    attr.Content = stream.ExtractBetween(QUOTE, QUOTE);
                    Add(attr);
                    // Parse more attributes
                    return ParseAttributes(stream);
                // End of attributes, no content
                case SLASH:
                    stream.ExtractUntil(true, RIGHTBRACE); // Remove right brace
                    return false;
                // End of attributes, with content
                case RIGHTBRACE:
                default:
                    return true;
            }
        }
        #endregion

        public void Add(XMLNode node)
        {
            Children.Add(node);
        }

        /**
         * @param f The function to use in finding.
         * 
         * A function of XMLNode, boolean.
         * Example Usage: XMLNode.find(p => p.getKey().Equals(key));
         * 
         * @return The first XMLNode matching the function.
         */
        public XMLNode Find(Func<XMLNode, bool> f)
        {
            if (f(this))
                return this;
            foreach (XMLNode child in Children)
            {
                if (f(child))
                    return child;
                XMLNode recursive = child.Find(f);
                if (recursive != null)
                    return recursive;
            }
            return null;
        }

        #region Selects
        /**
         * @param key The string key to match for the find.
         * 
         * @return The first XMLNode with the matching key.
         */
        public XMLNode Select(string key)
        {
            if (key == null)
                return null;
            return this.Find(p => key.Equals(p.Key));
        }

        public int SelectInt(string toParse)
        {
            XMLNode x = this.Select(toParse);
            if (x != null && !string.IsNullOrEmpty(x.Content))
                return x.Content.ToInt();
            return 0;
        }

        public double SelectDouble(string toParse)
        {
            XMLNode x = this.Select(toParse);
            if (x != null && !string.IsNullOrEmpty(x.Content))
                return x.Content.ToDouble();
            return 0;
        }

        public float SelectFloat(string toParse)
        {
            XMLNode x = this.Select(toParse);
            if (x != null && !string.IsNullOrEmpty(x.Content))
                return x.Content.ToFloat();
            return 0;
        }

        public T SelectEnum<T>(string toParse)
        {
            XMLNode x = this.Select(toParse);
            if (x != null && !string.IsNullOrEmpty(x.Content))
            {
                try
                {
                    return (T)Enum.Parse(typeof(T), x.Content, true);
                }
                catch (ArgumentException)
                {
                }
            }
            return default(T);
        }

        public string SelectString(string node)
        {
            XMLNode x = this.Select(node);
            if (x != null)
                return x.Content;
            return "";
        }

        public bool SelectBool(string toParse)
        {
            XMLNode x = this.Select(toParse);
            if (x != null && !string.IsNullOrEmpty(x.Content))
                return x.Content.ToBool();
            return false;
        }
        #endregion

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
        public List<XMLNode> SelectList(string key)
        {
            return this.Where((node) =>
            {
                return node.Key.Equals(key);
            }
            ).ToList<XMLNode>();
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            Stack<XMLNode> stack = new Stack<XMLNode>();
            string name = null;
            for (XMLNode p = this; p != null; p = p.Parent)
            {
                if (name == null && !string.IsNullOrEmpty(p.Name))
                    name = p.Name;
                stack.Push(p);
            }
            sb.Append("(" + name + ")");
            foreach (XMLNode c in stack)
                sb.Append(c.Key + ":");
            return sb.ToString();
        }

        public string Print()
        {
            StringBuilder sb = new StringBuilder();
            Print(sb, 0);
            return sb.ToString();
        }

        protected void Print(StringBuilder sb, int depth)
        {
            for(int i = 0 ; i < depth ; i++)
                sb.Append("  ");
            sb.Append(Key + ":" + Content);
            depth++;
            foreach (XMLNode node in Children)
            {
                sb.Append("\n");
                node.Print(sb, depth);
            }
        }

        public IEnumerator<XMLNode> GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
