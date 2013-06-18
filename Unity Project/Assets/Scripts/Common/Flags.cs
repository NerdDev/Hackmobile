using System;

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

    public Enum Value
    {
        get { return (Enum)Enum.ToObject(e.GetType(), flags); }
        set
        {
            e = value;
            this.flags = conv(value);
        }
    }

    public static implicit operator Enum(Flags src)
    {
        return src != null ? src.Value : (Enum)Enum.ToObject(src.e.GetType(), src.flags);
    }

    public static implicit operator Flags(Enum src)
    {
        return new Flags(src);
    }

    internal void set(Flags flags)
    {
        this.flags = flags.flags;
        this.e = (Enum) flags.MemberwiseClone();
    }
}