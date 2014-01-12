using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class Draw
{
    public static DrawAction<T> EqualTo<T>(T t)
    {
        return new DrawAction<T>((arr, x, y) =>
        {
            return t.Equals(arr[x, y]);
        });
    }

    public static DrawAction<T> If<T>(Predicate<T> predicate)
    {
        return new DrawAction<T>((arr, x, y) =>
            {
                return predicate(arr[x, y]);
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

    public static DrawAction<T> EqualThen<T>(T item, DrawActionCall<T> then, bool not = false)
    {
        return new DrawAction<T>((arr, x, y) =>
        {
            if (item.Equals(arr[x, y]) != not)
                return then(arr, x, y);
            return true;
        });
    }

    public static DrawAction<T> ContainedIn<T>(ICollection<T> col)
    {
        return new DrawAction<T>((arr, x, y) =>
        {
            return col.Contains(arr[x, y]);
        });
    }

    public static DrawAction<T> Around<T>(bool cornered, DrawAction<T> call)
    {
        return new DrawAction<T>((arr, x, y) =>
            {
                return arr.DrawAround(x, y, cornered, call);
            });
    }

    public static DrawAction<T> HasAround<T>(bool cornered, DrawAction<T> call)
    {
        return new DrawAction<T>((arr, x, y) =>
            {
                return !arr.DrawAround(x, y, cornered, Draw.Not<T>(call.Call));
            });
    }

    public static DrawAction<T> Dir<T>(GridDirection dir, DrawAction<T> call)
    {
        return new DrawAction<T>((arr, x, y) =>
        {
            return arr.DrawDir(x, y, dir, call);
        });
    }

    public static DrawAction<T> Loc<T>(GridLocation loc, DrawAction<T> call)
    {
        return new DrawAction<T>((arr, x, y) =>
        {
            return arr.DrawLocation(x, y, loc, call);
        });
    }

    public static DrawAction<T> SetTo<T>(T g)
    {
        return new DrawAction<T>((arr, x, y) =>
        {
            arr[x, y] = g;
            return true;
        });
    }

    public static DrawAction<T> Not<T>(DrawAction<T> call)
    {
        return new DrawAction<T>((arr, x, y) =>
            {
                return !call.Call(arr, x, y);
            });
    }

    public static DrawAction<T> And<T>(this DrawAction<T> call1, DrawAction<T> call2)
    {
        return new DrawAction<T>((arr, x, y) =>
        {
            return call1.Call(arr, x, y) && call2.Call(arr, x, y);
        });
    }

    public static DrawAction<T> AndNot<T>(this DrawAction<T> call1, DrawAction<T> call2)
    {
        return new DrawAction<T>((arr, x, y) =>
        {
            return call1.Call(arr, x, y) && !call2.Call(arr, x, y);
        });
    }

    public static DrawAction<T> Or<T>(this DrawAction<T> call1, DrawAction<T> call2)
    {
        return new DrawAction<T>((arr, x, y) =>
        {
            return call1.Call(arr, x, y) || call2.Call(arr, x, y);
        });
    }

    public static DrawAction<T> OrNot<T>(this DrawAction<T> call1, DrawAction<T> call2)
    {
        return new DrawAction<T>((arr, x, y) =>
        {
            return call1.Call(arr, x, y) || !call2.Call(arr, x, y);
        });
    }

    public static DrawAction<T> NotEdgeOfArray<T>()
    {
        return new DrawAction<T>((cont, x, y) =>
        {
            if (cont is Array2DRaw<T>)
            {
                Array2DRaw<T> arr = (Array2DRaw<T>) cont;
                if (x <= 0
                    || y <= 0
                    || y >= arr.Height - 1
                    || x >= arr.Width - 1) return false;
                return true;
            }
            return false;
        });
    }

    public static DrawAction<T> AddTo<T>(ICollection<T> t)
    {
        return new DrawAction<T>((arr, x, y) =>
            {
                t.Add(arr[x, y]);
                return true;
            });
    }

    public static DrawAction<T> AddTo<T>(ICollection<Value2D<T>> t)
    {
        return new DrawAction<T>((arr, x, y) =>
        {
            t.Add(new Value2D<T>(x, y, arr[x, y]));
            return true;
        });
    }

    public static DrawAction<T> AddTo<T>(Container2D<T> map, Point shift = null)
    {
        if (shift == null)
        {
            return new DrawAction<T>((arr, x, y) =>
            {
                map[x, y] = arr[x, y];
                return true;
            });
        }
        else
        {
            return new DrawAction<T>((arr, x, y) =>
            {
                map[x + shift.x, y + shift.y] = arr[x, y];
                return true;
            });
        }
    }

    public static DrawAction<T> SetTo<T>(T from, T to)
    {
        return new DrawAction<T>((arr, x, y) =>
        {
            if (arr[x, y].Equals(from))
                arr[x, y] = to;
            return true;
        });
    }

    public static DrawAction<T> SetToIfNotEqual<T>(T not, T to)
    {
        return new DrawAction<T>((arr, x, y) =>
        {
            if (!arr[x, y].Equals(not))
                arr[x, y] = to;
            return true;
        });
    }

    public static DrawAction<T> PickRandom<T>(out RandomPicker<T> picker)
    {
        picker = new RandomPicker<T>();
        DrawActionCall<T> f = picker.DrawingAction;
        return f;
    }

    public static DrawAction<T> Count<T>(out Counter counter)
    {
        counter = new Counter();
        return counter.Action<T>();
    }

    public static DrawAction<T> Count<T>(Counter counter)
    {
        return counter.Action<T>();
    }

    public static DrawAction<T> AlternatesSides<T>(Func<T, bool> eval)
    {
        return new DrawAction<T>((arr, x, y) =>
        {
            return arr.AlternatesSides(x, y, eval);
        });
    }

    public static DrawAction<T> Cornered<T>(Func<T, bool> eval, bool withOpposing = false)
    {
        return new DrawAction<T>((arr, x, y) =>
        {
            return arr.Cornered(x, y, eval, withOpposing);
        });
    }

    public static DrawAction<T> NotBlocking<T>(Func<T, bool> eval)
    {
        return new DrawAction<T>((arr, x, y) =>
            {
                return !arr.Blocking(x, y, eval);
            }
        );
    }
    #region GridType
    public static DrawAction<GridType> CanDrawDoor()
    {
        return new DrawAction<GridType>((arr, x, y) =>
            {
                if (arr[x, y] != GridType.Wall)
                    return false;
                // Include null to work with levelgen placement
                return (arr.AlternatesSides(x, y, GridTypeEnum.WalkableOrNull));
            }
        );
    }

    public static DrawAction<GridType> Walkable()
    {
        return new DrawAction<GridType>((arr, x, y) =>
            {
                return GridTypeEnum.Walkable(arr[x, y]);
            });
    }
    #endregion
}


