using System;
using System.Collections.Generic;

public class ESFlags<T> where T : struct, IComparable, IConvertible
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
                return this[e];
            else
                return strings.Contains(index);
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
                if (value)
                    strings.Add(index);
                else
                    strings.Remove(index);
            }
        }
    }
}
