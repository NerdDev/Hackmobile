using System;

/**
 * Helper class for dealing with flags.
 * 
 * Do not use with enums longer than 62 variables.
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
public class GenericFlags<T> where T : IComparable, IConvertible
{
    protected long flags;

    public GenericFlags()
    {
        flags = 0;
    }

    public GenericFlags(T e)
    {
        flags = bitwise(e);
    }

    private static long bitwise(T e)
    {
        return (long) 1 << Convert.ToInt32(e);
    }

    public bool this[T index]
    {
        get
        {
            long i = bitwise(index);
            return (this.flags & i) == i;
        }
        set
        {
            if (value)
            {
                this.flags |= bitwise(index);
            }
            else
            {
                this.flags &= ~(bitwise(index));
            }
        }
    }

    public static implicit operator T(GenericFlags<T> src)
    {
        return src != null ? (T)Enum.ToObject(typeof(T), src.flags) : (T)Enum.ToObject(typeof(T), 0);
    }

    public static implicit operator GenericFlags<T>(T src)
    {
        return new GenericFlags<T>(src);
    }

    public void setNull()
    {
        flags = 0;
    }
}