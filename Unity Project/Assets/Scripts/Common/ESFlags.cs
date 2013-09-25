using System;
using System.Collections.Generic;
using XML;

public class ESFlags<T> : FieldContainerClass where T : struct, IComparable, IConvertible
{
    GenericFlags<T> flags = new GenericFlags<T>();
    HashSet<string> strings = new HashSet<string>();

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

    public bool getAnd(params T[] index)
    {
        throw new NotImplementedException();
    }

    public override void SetParams()
    {
        base.SetParams();
        XMLNifty.parseList(map.x, "entry",
            obj =>
            {
                this[obj.SelectString("name")] = true;
            });
    }
}