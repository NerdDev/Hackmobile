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

    public static implicit operator Enum(Flags src)
    {
        return src != null ? src.e : (Enum)Enum.ToObject(src.e.GetType(), src.flags);
    }

    public static implicit operator Flags(Enum src)
    {
        return new Flags(src);
    }

    public void setNull()
    {
        flags = 0;
    }
}