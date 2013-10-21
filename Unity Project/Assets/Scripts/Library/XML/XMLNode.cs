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
        const char LEFTBRACE = '<';
        const char RIGHTBRACE = '>';
        const char SPACE = ' ';
        const char QUOTE = '"';
        const char QUOTE2 = '\'';
        const char SLASH = '/';
        const char QMARK = '?';
        const char EQUALS = '=';
        const char EXCLAMATION = '!';
        const char DASH = '-';
        const char SQR = ']';
        #endregion

        public string Key { get; set; }
        public Dictionary<string, XMLNode> Children { get; protected set; }
        public XMLNode Parent { get; protected set; }
        public string Name { get { return SelectString("name"); } }
        public string Content { get; set; }

        public XMLNode(XMLNode parent)
        {
            Children = new Dictionary<string, XMLNode>();
            Parent = parent;
            Content = "";
        }

        #region Parsing
        public void Parse(string str)
        {
            Parse(new ByteStream(str));
        }

        public void Parse(ByteStream stream)
        {
            // Skip whitespace and remove startbrace
            stream.SkipWhitespace();
            if (stream.ExtractChar() != LEFTBRACE)
                throw new FormatException("Node didn't start with " + LEFTBRACE);
            // Get Name
            Key = stream.ExtractUntilWhiteSpace().Trim();
            // Parse content
            if (ParseAttributes(stream))
                ParseContent(stream);
        }

        protected void ParseContent(ByteStream stream)
        {
            stream.SkipWhitespace();
            if (stream.GetChar() == LEFTBRACE)
            { // Node content
                if (IsEndNode(stream))
                { // Done
                    return;
                }
                else
                {
                    XMLNode subNode = new XMLNode(this);
                    subNode.Parse(stream);
                }
            }
            else
            { // String content
                Content = stream.ExtractUntilTrim(LEFTBRACE);
            }
            // Parse rest of content
            ParseContent(stream);
        }

        protected bool IsEndNode(ByteStream stream)
        {
            stream.Skip(1); // Skip brace
            if (stream.GetChar() == SLASH)
                return true;
            stream.Skip(-1); // Jump back
            return false;
        }

        /*
         * Returns whether node has sub-content
         */
        protected bool ParseAttributes(ByteStream stream)
        {
            string str = stream.ExtractUntil(EQUALS, SLASH, RIGHTBRACE);
            switch (stream.ExtractChar())
            {
                // An attribute
                case EQUALS:
                    // Treating attributes as subnodes with text content
                    XMLNode attr = new XMLNode(this);
                    attr.Key = str;
                    stream.SkipWhitespace();
                    stream.ExtractChar(); // Skip quote
                    attr.Content = stream.ExtractUntilTrim(QUOTE); // Grab until next quote
                    stream.ExtractChar(); // Skip ending quote
                    Add(attr);
                    // Parse more attributes
                    return ParseAttributes(stream);
                // End of attributes, no content
                case SLASH:
                    stream.ExtractUntil(RIGHTBRACE);
                    stream.ExtractChar(); // Remove right brace
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
            Children.Add(node.Key, node);
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
            foreach (XMLNode child in Children.Values)
            {
                if (f(child))
                    return child;
                return child.Find(f); // Recursive
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
            return this.Find(p => p.Key.Equals(key));
        }

        public int SelectInt(string toParse)
        {
            XMLNode x = this.Select(toParse);
            if (x != null)
            {
                string sel = x.Content;
                if (sel != null)
                    return sel.ToInt();
            }
            return 0;
        }

        public double SelectDouble(string toParse)
        {
            XMLNode x = this.Select(toParse);
            if (x != null)
            {
                if (x.Content != null)
                    return x.Content.ToDouble();
            }
            return 0;
        }

        public float SelectFloat(string toParse)
        {
            XMLNode x = this.Select(toParse);
            if (x != null)
            {
                string sel = x.Content;
                if (sel != null)
                {
                    return sel.ToFloat();
                }
            }
            return 0;
        }

        public T SelectEnum<T>(string toParse)
        {
            XMLNode x = this.Select(toParse);
            if (x != null)
            {
                string sel = x.Content;
                if (sel != null)
                {
                    try
                    {
                        return (T)Enum.Parse(typeof(T), sel, true);
                    }
                    catch (ArgumentException)
                    {
                        return default(T);
                    }
                }
            }
            return default(T);
        }

        public string SelectString(string node)
        {
            XMLNode x = this.Select(node);
            if (x != null)
            {
                string sel = x.Content;
                if (sel != null)
                {
                    return sel;
                }
            }
            return "";
        }

        public bool SelectBool(string toParse)
        {
            XMLNode x = this.Select(toParse);
            if (x != null)
            {
                string sel = x.Content;
                if (sel != null)
                {
                    return sel.ToBool();
                }
            }
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
            for (XMLNode p = this; p != null; p = p.Parent)
                stack.Push(p);
            foreach (XMLNode c in stack)
                sb.Append(c.Key + ":");
            return sb.ToString();
        }

        public IEnumerator<XMLNode> GetEnumerator()
        {
            return Children.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
