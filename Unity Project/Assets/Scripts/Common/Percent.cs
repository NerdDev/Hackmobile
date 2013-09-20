using System;

public class Percent
{
    public float Float
    {
        get;
        protected set
        {
            if (value < 0)
                Float = 0;
            else if (value > 1)
                Float = 1;
            else
                Float = value;
        }
    }
    public int Int
    {
        get
        {
            return ((int)Math.Round(Float * 100));
        }
    }

    public Percent(int num)
    {
        Float = num;
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
