using System;

public class Percent
{
    float f;
    public float Float
    {
        get
        {
            return f;
        }
        protected set
        {
            if (value < 0)
                f = 0;
            else if (value > 1)
                f = 1;
            else
                f = value;
        }
    }
    public int Int
    {
        get
        {
            return ((int)Math.Round(Float * 100));
        }
        set
        {
            Float = ((float) value) / 100;
        }
    }

    public Percent(int num)
    {
        Int = num;
    }

    public Percent(float f)
    {
        Float = f;
    }

    public Percent(double d)
    {
        Float = (float) d;
    }

    public static implicit operator Percent(int num)
    {
        return new Percent(num);
    }

    public static implicit operator Percent(float num)
    {
        return new Percent(num);
    }

    public static implicit operator Percent(double num)
    {
        return new Percent(num);
    }
}
