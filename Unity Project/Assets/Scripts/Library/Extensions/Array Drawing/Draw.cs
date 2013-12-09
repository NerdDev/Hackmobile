using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Draw
{
    public static DrawAction<T> EqualTo<T>(T t)
    {
        return new DrawAction<T>((arr, x, y) =>
        {
            return t.Equals(arr[y, x]);
        });
    }

    public static DrawAction<T> IfThen<T>(DrawActionCall<T> ifCall, DrawActionCall<T> then)
    {
        return new DrawAction<T>((arr, x, y) =>
        {
            if (ifCall(arr, x, y))
                return then(arr, x, y);
            return true;
        });
    }

    public static DrawAction<T> ContainedIn<T>(ICollection<T> col)
    {
        return new DrawAction<T>((arr, x, y) =>
        {
            return col.Contains(arr[y, x]);
        });
    }

    public static DrawAction<T> SetTo<T>(T g)
    {
        return new DrawAction<T>((arr, x, y) =>
        {
            arr[y, x] = g;
            return true;
        });
    }

    public static DrawAction<T> NotEdgeOfArray<T>()
    {
        return new DrawAction<T>((arr, x, y) =>
        {
            if (x <= 0
                || y <= 0
                || y >= arr.GetLength(0) - 1
                || x >= arr.GetLength(1) - 1) return false;
            return true;
        });
    }

    public static DrawAction<T> AddTo<T>(ICollection<T> t)
    {
        return new DrawAction<T>((arr, x, y) =>
            {
                t.Add(arr[y, x]);
                return true;
            });
    }

    public static DrawAction<T> AddTo<T>(ICollection<Value2D<T>> t)
    {
        return new DrawAction<T>((arr, x, y) =>
        {
            t.Add(new Value2D<T>(x, y, arr[y, x]));
            return true;
        });
    }

    public static DrawAction<T> AddTo<T>(MultiMap<T> map)
    {
        return new DrawAction<T>((arr, x, y) =>
        {
            map.Put(arr[y, x], x, y);
            return true;
        });
    }

    public static DrawAction<T> SetTo<T>(T from, T to)
    {
        return new DrawAction<T>((arr, x, y) =>
        {
            if (arr[y, x].Equals(from))
                arr[y, x] = to;
            return true;
        });
    }

    public static DrawAction<T> SetToIfNotEqual<T>(T not, T to)
    {
        return new DrawAction<T>((arr, x, y) =>
        {
            if (!arr[y, x].Equals(not))
                arr[y, x] = to;
            return true;
        });
    }

    public static DrawAction<T> PickRandom<T>(T[,] arr, out RandomPicker<T> picker)
    {
        picker = new RandomPicker<T>(arr);
        DrawActionCall<T> f = picker.DrawingAction;
        return f;
    }

    #region GridType
    public static DrawAction<GridType> CanDrawDoor()
    {
        return new DrawAction<GridType>((arr, x, y) =>
            {
                if (arr[y, x] != GridType.Wall)
                    return false;
                // Include null to work with levelgen placement
                return (arr.Alternates(x, y, GridTypeEnum.WalkableOrNull));
            }
        );
    }
    #endregion
}


