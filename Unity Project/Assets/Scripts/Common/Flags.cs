using System;

/**
 * Helper class for dealing with flags.
 * 
 * Do not use with enums larger than 62 entries. All enums this is used with must
 *  be based on the [Flags] attribute, and enum values marked accordingly as
 *  Val1 = 0x1, Val2 = 0x2, Val3 = 0x4, and etc.
 *  
 * Usage:
 *  Flags fl = new Flags<SomeEnum>(); // use the 0 value or somesuch with all bits cleared.
 *  fl[SomeEnum.VALUE1] = true; //sets the value1 bit to true.
 *  if (fl[SomeEnum.VALUE2]) { } //checks if the bit is true or not.
 *  
 * All checks can be done with multiple bits, ie;
 *  fl[SomeEnum.VALUE1 | SomeEnum.VALUE2] = true; //sets both value1 and value2 to true.
 * 
 */
public class Flags<T> where T : IComparable, IConvertible
{
    protected long flags;

    public Flags()
    {
        flags = 0;
    }

    public Flags(T e)
    {
        flags = Convert.ToInt64(e);
    }

    public bool this[T index]
    {
        get
        {
            long i = Convert.ToInt64(index);
            return (this.flags & i) == i;
        }
        set
        {
            if (value)
            {
                this.flags |= Convert.ToInt64(index);
            }
            else
            {
                this.flags &= ~Convert.ToInt64(index);
            }
        }
    }

    public static implicit operator T(Flags<T> src)
    {
        return src != null ? (T)Enum.ToObject(typeof(T), src.flags) : (T)Enum.ToObject(typeof(T), 0);
    }

    public static implicit operator Flags<T>(T src)
    {
        return new Flags<T>(src);
    }

    public void setNull()
    {
        flags = 0;
    }

    public int GetHash()
    {
        return flags.GetHashCode() * 3;
    }
}