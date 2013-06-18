using System;

/**
 * Helper class for dealing with flags.
 * 
 * Usage:
 *  Flags fl = new Flags(SomeEnum.DEFAULT); // use the 0 value or somesuch with all bits cleared.
 *  fl[SomeEnum.VALUE1] = true; //sets the value1 bit to true.
 *  if (fl[SomeEnum.VALUE2]) { } //checks if the bit is true or not.
 *  
 * All checks can be done with multiple bits, ie;
 *  fl[SomeEnum.VALUE1 | SomeEnum.VALUE2] = true; //sets both value1 and value2 to true.
 * 
 */
public class Flags
{
    long flags;
    Enum e;

    public Flags(Enum e)
    {
        this.e = e;
        flags = Convert.ToInt64(e);
    }

    public bool this[Enum index]
    {
        get
        {
            return (this.flags & conv(index)) == conv(index);
        }
        set
        {
            if (value)
            {
                this.flags |= conv(index);
            }
            else
            {
                this.flags &= ~conv(index);
            }
        }
    }

    private static long conv(Enum index)
    {
        return Convert.ToInt64(index);
    }

    public static implicit operator Enum(Flags src)
    {
        return src != null ? src.e : (Enum) Enum.ToObject(src.e.GetType(), src.flags);
    }

    public static implicit operator Flags(Enum src)
    {
        return new Flags(src);
    }

    internal void set(Flags flags)
    {
        this.flags = flags.flags;
        this.e = flags.e;
    }
}