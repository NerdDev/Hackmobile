using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

/**
 * Helper class for dealing with flags.
 * 
 * Use setSize to change the size to a larger value. Default = 256.
 *  setSize will reset all current values in the class.
 *  
 * Usage:
 *  GenericFlags fl = new GenericFlags<SomeEnum>();
 *  fl[SomeEnum.VALUE1] = true; //sets the value1 bit to true.
 *  if (fl[SomeEnum.VALUE2]) { } //checks if the bit is true or not.
 *  
 * All checks can be done with multiple bits, ie;
 *  fl.set(true, SomeEnum.VALUE1, SomeEnum.VALUE2); //sets value 1 and value 2 to true
 *  fl.get(GenericFlags<>.Ops.OR, SomeEnum.VALUE1, SomeEnum.VALUE2) //gets VALUE1 | VALUE2
 *  fl.get(GenericFlags<>.Ops.AND, SomeEnum.VALUE1, SomeEnum.VALUE2) //gets VALUE1 & VALUE2
 * 
 */
public class GenericFlags<T> : IEnumerable<T> where T : struct, IComparable, IConvertible
{
    HashSet<T> _set = new HashSet<T>();
    public bool Empty { get { return _set.Count == 0; } }

    public GenericFlags()
    {
        if (!typeof(T).IsEnum)
        {
            throw new ArgumentException("T must be an enumerated type");
        }
    }

    public GenericFlags(T e)
        : this()
    {
        this[e] = true;
    }

    public GenericFlags(IEnumerable<T> keys)
        : this()
    {
        foreach (T t in keys)
        {
            this[t] = true;
        }
    }

    public bool this[T index]
    {
        get
        {
            return _set.Contains(index);
        }
        set
        {
            if (value)
            {
                _set.Add(index);
            }
            else
            {
                _set.Remove(index);
            }
        }
    }

    public bool GetAnd(params T[] index)
    {
        for (int i = 0; i < index.Length; i++)
        {
            if (!this[index[i]])
            {
                return false;
            }
        }
        return true;
    }

    public bool GetOr(params T[] index)
    {
        for (int i = 0; i < index.Length; i++)
        {
            if (this[index[i]])
            {
                return true;
            }
        }
        return false;
    }

    public bool Contains(GenericFlags<T> rhs)
    {
        return rhs._set.IsSubsetOf(_set);
    }

    public void Set(bool val, params T[] index)
    {
        for (int i = 0; i < index.Length; i++)
        {
            this[index[i]] = true;
        }
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        bool first = true;
        foreach (T t in this)
        {
            if (first)
            {
                first = false;
            }
            else
            {
                sb.Append("|");
            }
            sb.Append(t.ToString());
        }
        return sb.ToString();
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _set.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    public override bool Equals(object obj)
    {
        GenericFlags<T> rhs = obj as GenericFlags<T>;
        if (rhs == null) return false;
        if (_set.Count != rhs._set.Count) return false;
        return _set.IsSubsetOf(rhs._set);
    }

    public override int GetHashCode()
    {
        int hash = 17;
        foreach (T t in _set)
        {
            hash = hash * 23 + t.GetHashCode();
        }
        return hash;
    }

    public void ToLog(Logs log)
    {
        if (BigBoss.Debug.logging(log))
        {
            foreach (T t in this)
            {
                BigBoss.Debug.w(log, t.ToString());
            }
        }
    }
}
