using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public delegate bool DrawActionCall<T>(T[,] arr, int x, int y);
public class DrawAction<T>
{
    public DrawActionCall<T> Call { get; protected set; }

    public DrawAction(DrawActionCall<T> call)
    {
        Call = call;
    }

    public DrawAction(DrawActionCall<T> call, params DrawActionCall<T>[] calls)
    {
        if (calls.Length == 0) 
        {
            Call = call;
        }
        else
        {
            DrawAction<T> tmp = call;
            Call = tmp.Then(calls);
        }
    }

    public static implicit operator DrawAction<T>(DrawActionCall<T> call)
    {
        return new DrawAction<T>(call);
    }

    public static implicit operator DrawActionCall<T>(DrawAction<T> action)
    {
        return action.Call;
    }

    public DrawAction<T> Then(DrawAction<T> rhs)
    {
        return Then(rhs.Call);
    }

    public DrawAction<T> Then(params DrawAction<T>[] rhs)
    {
        if (rhs.Length == 0) return this;
        DrawActionCall<T>[] arr = new DrawActionCall<T>[rhs.Length];
        for (int i = 0; i < rhs.Length; i++)
            arr[i] = rhs[i].Call;
        return Then(arr);
    }

    public DrawAction<T> Then(DrawActionCall<T> rhs)
    {
        if (rhs == null) return this;
        return new DrawAction<T>((arr, x, y) =>
        {
            if (!Call(arr, x, y)) return false;
            return rhs(arr, x, y);
        });
    }

    public DrawAction<T> Then(params DrawActionCall<T>[] rhs)
    {
        if (rhs.Length == 0) return this;
        if (rhs.Length == 1) return Then(rhs[0]);
        return new DrawAction<T>((arr, x, y) =>
        {
            if (!Call(arr, x, y)) return false;
            for (int i = 0; i < rhs.Length; i++)
                if (!rhs[i](arr, x, y)) return false;
            return true;
        });
    }
}
