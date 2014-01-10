using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public delegate bool DrawActionCall<T>(Container2D<T> arr, int x, int y);
public class DrawAction<T>
{
    public DrawActionCall<T> Call { get; protected set; }

    public DrawAction(DrawActionCall<T> call)
    {
        Call = call;
    }

    public static implicit operator DrawAction<T>(DrawActionCall<T> call)
    {
        return new DrawAction<T>(call);
    }

    public static implicit operator DrawActionCall<T>(DrawAction<T> action)
    {
        return action.Call;
    }

    public DrawAction<T> And(DrawAction<T> rhs)
    {
        return And(rhs.Call);
    }

    public DrawAction<T> And(params DrawActionCall<T>[] rhs)
    {
        if (rhs.Length == 0) return this;
        if (rhs.Length == 1) return And(rhs[0]);
        return new DrawAction<T>((arr, x, y) =>
        {
            if (!Call(arr, x, y)) return false;
            for (int i = 0; i < rhs.Length; i++)
                if (!rhs[i](arr, x, y)) return false;
            return true;
        });
    }

    public DrawAction<T> And(DrawActionCall<T> rhs)
    {
        return new DrawAction<T>((arr, x, y) =>
        {
            if (!Call(arr, x, y)) return false;
            return rhs(arr, x, y);
        });
    }

    public DrawAction<T> Then(DrawActionCall<T> rhs)
    {
        return new DrawAction<T>((arr, x, y) =>
            {
                Call(arr, x, y);
                return rhs(arr, x, y);
            });
    }

    public DrawAction<T> IfThen(DrawActionCall<T> rhs)
    {
        return new DrawAction<T>((arr, x, y) =>
            {
                if (Call(arr, x, y))
                    return rhs(arr, x, y);
                return true;
            });
    }
}
