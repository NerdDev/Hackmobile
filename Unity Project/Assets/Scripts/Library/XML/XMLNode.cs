using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

    public string Key;
    public List<XMLNode> Children { get; protected set; }
    public XMLNode Parent { get; protected set; }
    public string Name { get { return SelectString("name"); } }
    public string Content;
    public int Depth;

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
        key = key.ToUpper();
        return this.Find(p => key.Equals(p.Key.ToUpper()));
    }

    public int SelectInt(string toParse, int defaultVal = 0)
    {
        XMLNode x = this.Select(toParse);
        if (x != null && !string.IsNullOrEmpty(x.Content))
            return x.Content.ToInt();
        return defaultVal;
    }

    public ushort SelectUShort(string toParse, ushort defaultVal = 0)
    {
        XMLNode x = this.Select(toParse);
        if (x != null && !string.IsNullOrEmpty(x.Content))
            return x.Content.ToUShort();
        return defaultVal;
    }

    public double SelectDouble(string toParse, double defaultVal = 0)
    {
        XMLNode x = this.Select(toParse);
        if (x != null && !string.IsNullOrEmpty(x.Content))
            return x.Content.ToDouble();
        return defaultVal;
    }

    public float SelectFloat(string toParse, float defaultVal = 0)
    {
        XMLNode x = this.Select(toParse);
        if (x != null && !string.IsNullOrEmpty(x.Content))
            return x.Content.ToFloat();
        return defaultVal;
    }

    public T SelectEnum<T>(string toParse, T defaultVal = default(T))
    {
        T t;
        if (!SelectEnum(toParse, out t))
        {
            t = defaultVal;
        }
        return t;
    }

    public bool SelectEnum<T>(string toParse, out T t)
    {
        XMLNode x = this.Select(toParse);
        if (x != null && !string.IsNullOrEmpty(x.Content))
        {
            try
            {
                t = (T)Enum.Parse(typeof(T), x.Content, true);
                return true;
            }
            catch (ArgumentException)
            {
            }
        }
        t = default(T);
        return false;
    }

    public IEnumerable<T> SelectEnums<T>(string toParse)
    {
        XMLNode x = this.Select(toParse);
        if (x != null)
        {
            foreach (XMLNode node in x)
            {
                XMLNode name = node.Select("name");
                if (name == null) continue;
                T t;
                try
                {
                    t = (T)Enum.Parse(typeof(T), name.Content, true);
                }
                catch (ArgumentException)
                {
                    #region DEBUG
                    if (BigBoss.Debug.logging(Logs.XML))
                    {
                        BigBoss.Debug.w(Logs.XML, "Error parsing enum " + name.Content + " in " + this);
                    }
                    #endregion
                    continue;
                }
                yield return t;
            }
        }
    }

    public string SelectString(string node, string defaultVal = "")
    {
        XMLNode x = this.Select(node);
        if (x != null)
            return x.Content;
        return defaultVal;
    }

    public bool SelectString(string node, out string value)
    {
        XMLNode x = this.Select(node);
        if (x != null)
        {
            value = x.Content;
            return true;
        }
        value = null;
        return false;
    }

    public bool SelectBool(string toParse, bool defaultVal = false)
    {
        XMLNode x = this.Select(toParse);
        if (x != null && !string.IsNullOrEmpty(x.Content))
            return x.Content.ToBool();
        return defaultVal;
    }

    public T Select<T>(string nodeName) where T : IXmlParsable, new()
    {
        return Select(nodeName, new T());
    }

    public T Select<T>(string nodeName, T t)
         where T : IXmlParsable
    {
        XMLNode node = Select(nodeName);
        if (node != null)
        {
            t.ParseXML(node);
        }
        return t;
    }

    public T Select<T>() where T : IXmlParsable, new()
    {
        T t = new T();
        t.ParseXML(this);
        return t;
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
    public IEnumerable<XMLNode> SelectList(string key)
    {
        return this.Where((node) =>
        {
            return node.Key.Equals(key);
        });
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
        for (int i = 0; i < depth; i++)
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
