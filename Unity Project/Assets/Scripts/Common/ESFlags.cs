using System;
using System.Collections.Generic;
using System.Text;
using XML;

public class ESFlags<T> : IXmlParsable where T : struct, IComparable, IConvertible
{
    GenericFlags<T> flags = new GenericFlags<T>();
    HashSet<string> strings = new HashSet<string>();
    public bool Empty { get { return strings.Count == 0 && flags.Empty; } }

    public ESFlags()
    {

    }

    public ESFlags(params T[] es)
    {
        this[es] = true;
    }

    public bool this[T[] indices]
    {
        get
        {
            foreach (T e in indices)
            {
                if (!this[e])
                    return false;
            }
            return true;
        }
        set
        {
            foreach (T e in indices)
            {
                this[e] = value;
            }
        }
    }
    public bool this[T index] { get { return flags[index]; } set { flags[index] = value; } }
    public bool this[string index]
    {
        get
        {
            T e;
            if (index.ToEnum<T>(out e))
            {
                return this[e];
            }
            else
            {
                index = index.ToUpper();
                return strings.Contains(index);
            }
        }
        set
        {
            T e;
            if (index.ToEnum<T>(out e))
            {
                this[e] = value;
            }
            else
            {
                index = index.ToUpper();
                if (value)
                    strings.Add(index);
                else
                    strings.Remove(index);
            }
        }
    }

    public bool Get(T e)
    {
        return this[e];
    }

    public bool Get(string s)
    {
        return this[s];
    }

    public void Set(bool val, T e)
    {
        this[e] = val;
    }

    public void Set(bool val, string s)
    {
        this[s] = val;
    }

    public void Set(bool val, T[] es)
    {
        foreach (T e in es)
        {
            this[e] = val;
        }
    }

    public void Set(bool val, string[] strs)
    {
        foreach (string s in strs)
        {
            this[s] = val;
        }
    }

    public bool Contains(ESFlags<T> rhs)
    {
        return flags.Contains(rhs.flags) && rhs.strings.IsSubsetOf(strings);
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append(flags.ToString());
        bool first = sb.Length == 0;
        foreach (string str in strings)
        {
            if (first)
            {
                first = false;
            }
            else
            {
                sb.Append("|");
            }
            sb.Append(str);
        }
        return sb.ToString();
    }

    public bool getAnd(params T[] index)
    {
        throw new NotImplementedException();
    }

    public static implicit operator ESFlags<T>(T e)
    {
        ESFlags<T> ret = new ESFlags<T>();
        ret[e] = true;
        return ret;
    }

    public static implicit operator ESFlags<T>(T[] e)
    {
        ESFlags<T> ret = new ESFlags<T>();
        ret[e] = true;
        return ret;
    }

    public void ParseXML(XMLNode x)
    {
        foreach (XMLNode node in x.SelectList())
            this[node.Name] = true;
    }
}
