using System;
using System.Collections;
using XML;

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
public class GenericFlags<T> : FieldContainerClass where T : IComparable, IConvertible
{
    public BitArray ba;

    public GenericFlags()
    {
        ba = new BitArray(256, false);
    }

    public GenericFlags(T e)
    {
        ba = new BitArray(256, false);
        ba.Set(Convert.ToInt32(e), true);
    }

    //This also resets all bits.
    public void setSize(int size)
    {
        ba = new BitArray(size, false);
    }

    public bool this[T index]
    {
        get
        {
            return ba.Get(Convert.ToInt32(index));
        }
        set
        {
            if (value)
            {
                ba.Set(Convert.ToInt32(index), true);
            }
            else
            {
                ba.Set(Convert.ToInt32(index), false);
            }
        }
    }

    public bool get(T index)
    {
        return ba.Get(Convert.ToInt32(index));
    }

    public bool getAnd(params T[] index)
    {
        for (int i = 0; i < index.Length; i++)
        {
            if (!ba.Get(Convert.ToInt32(index[i])))
            {
                return false;
            }
        }
        return true;
    }

    public bool getOr(params T[] index)
    {
        for (int i = 0; i < index.Length; i++)
        {
            if (ba.Get(Convert.ToInt32(index[i])))
            {
                return true;
            }
        }
        return false;
    }

    public void set(bool val, params T[] index)
    {
        for (int i = 0; i < index.Length; i++)
        {
            ba.Set(Convert.ToInt32(index[i]), val);
        }
    }

    public static implicit operator T(GenericFlags<T> src)
    {
        return src != null ? (T)Enum.ToObject(typeof(T), src.ba) : (T)Enum.ToObject(typeof(T), 0);
    }

    public static implicit operator GenericFlags<T>(T src)
    {
        return new GenericFlags<T>(src);
    }

    public void setNull()
    {
        ba = new BitArray(1, false);
    }

    public enum Ops
    {
        AND,
        OR,
    }

    public override void SetParams()
    {
        base.SetParams();
        XMLNifty.parseList(map.x, "entry",
            obj =>
            {
                T np = obj.SelectEnum<T>("name");
                this[np] = true;
            });
    }
}