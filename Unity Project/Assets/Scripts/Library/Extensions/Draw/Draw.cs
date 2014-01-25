using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Draw
{
    public static DrawAction<T> EqualTo<T>(T t)
    {
        return (arr, x, y) =>
        {
            return t.Equals(arr[x, y]);
        };
    }

    public static DrawAction<T> If<T>(Predicate<T> predicate)
    {
        return (arr, x, y) =>
            {
                return predicate(arr[x, y]);
            };
    }

    public static DrawAction<T> EqualThen<T>(T item, DrawAction<T> then, bool not = false)
    {
        return (arr, x, y) =>
        {
            if (item.Equals(arr[x, y]) != not)
                return then(arr, x, y);
            return true;
        };
    }

    public static DrawAction<T> ContainedIn<T>(ICollection<T> col)
    {
        return (arr, x, y) =>
        {
            return col.Contains(arr[x, y]);
        };
    }

    public static DrawAction<T> ContainedIn<T>(Container2D<T> coll)
    {
        return (arr, x, y) =>
        {
            return coll.Contains(x, y);
        };
    }

    public static DrawAction<T> Around<T>(bool cornered, DrawAction<T> call)
    {
        return (arr, x, y) =>
            {
                return arr.DrawAround(x, y, cornered, call);
            };
    }

    public static DrawAction<T> HasAround<T>(bool cornered, DrawAction<T> call)
    {
        return (arr, x, y) =>
            {
                return !arr.DrawAround(x, y, cornered, Draw.Not<T>(call));
            };
    }

    public static DrawAction<T> Dir<T>(GridDirection dir, DrawAction<T> call)
    {
        return (arr, x, y) =>
        {
            return arr.DrawDir(x, y, dir, call);
        };
    }

    public static DrawAction<T> Loc<T>(GridLocation loc, DrawAction<T> call)
    {
        return (arr, x, y) =>
        {
            return arr.DrawLocation(x, y, loc, call);
        };
    }

    public static DrawAction<T> SetTo<T>(Container2D<T> container, T g, Point shift = null)
    {
        if (shift == null)
        {
            return (arr, x, y) =>
            {
                container[x, y] = g;
                return true;
            };
        }
        else
        {
            return (arr, x, y) =>
            {
                container[x + shift.x, y + shift.y] = g;
                return true;
            };
        }
    }

    public static DrawAction<T> SetTo<T>(T g)
    {
        return (arr, x, y) =>
        {
            arr[x, y] = g;
            return true;
        };
    }

    public static DrawAction<T> Not<T>(DrawAction<T> call)
    {
        return (arr, x, y) =>
            {
                return !call(arr, x, y);
            };
    }

    public static DrawAction<T> NotEdgeOfArray<T>()
    {
        return new DrawAction<T>((cont, x, y) =>
        {
            if (cont is Array2DRaw<T>)
            {
                Array2DRaw<T> arr = (Array2DRaw<T>)cont;
                if (x <= 0
                    || y <= 0
                    || y >= arr.Height - 1
                    || x >= arr.Width - 1) return false;
                return true;
            }
            return true;
        });
    }

    public static DrawAction<T> AddTo<T>(ICollection<T> t)
    {
        return (arr, x, y) =>
            {
                t.Add(arr[x, y]);
                return true;
            };
    }

    public static DrawAction<T> AddTo<T>(ICollection<Value2D<T>> t)
    {
        return (arr, x, y) =>
        {
            t.Add(new Value2D<T>(x, y, arr[x, y]));
            return true;
        };
    }

    public static DrawAction<T> AddTo<T>(Container2D<T> map, Point shift = null)
    {
        if (shift == null)
        {
            return (arr, x, y) =>
            {
                map[x, y] = arr[x, y];
                return true;
            };
        }
        else
        {
            return (arr, x, y) =>
            {
                map[x + shift.x, y + shift.y] = arr[x, y];
                return true;
            };
        }
    }

    public static DrawAction<T> WithinTo<T>(float dist, Point to)
    {
        return (arr, x, y) =>
        {
            return to.Distance(x, y) <= dist;
        };
    }

    public static DrawAction<T> WithinTo<T>(float dist, int x, int y)
    {
        return WithinTo<T>(dist, new Point(x, y));
    }

    public static DrawAction<T> SetTo<T>(T from, T to)
    {
        return (arr, x, y) =>
        {
            if (arr[x, y].Equals(from))
                arr[x, y] = to;
            return true;
        };
    }

    public static DrawAction<T> SetToIfNotEqual<T>(T not, T to)
    {
        return (arr, x, y) =>
        {
            if (!arr[x, y].Equals(not))
                arr[x, y] = to;
            return true;
        };
    }

    public static DrawAction<T> PickRandom<T>(out RandomPicker<T> picker)
    {
        picker = new RandomPicker<T>();
        DrawAction<T> f = picker.DrawingAction;
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
        return (arr, x, y) =>
        {
            return arr.AlternatesSides(x, y, eval);
        };
    }

    public static DrawAction<T> Cornered<T>(Func<T, bool> eval, bool withOpposing = false)
    {
        return (arr, x, y) =>
        {
            return arr.Cornered(x, y, eval, withOpposing);
        };
    }

    public static DrawAction<T> NotBlocking<T>(Func<T, bool> eval)
    {
        return (arr, x, y) =>
            {
                return !arr.Blocking(x, y, eval);
            };
    }

    public static DrawAction<T> Stop<T>()
    {
        return (arr, x, y) => { return false; };
    }

    public static DrawAction<T> Inside<T>(Bounding bounds)
    {
        return (arr, x, y) =>
        {
            return bounds.Contains(x, y);
        };
    }

    #region GridType
    private static DrawAction<GridType> _canDrawDoor = new DrawAction<GridType>((arr, x, y) =>
    {
        if (arr[x, y] != GridType.Wall)
            return false;
        // Include null to work with levelgen placement
        return (arr.AlternatesSides(x, y, GridTypeEnum.WalkableOrNull));
    });
    public static DrawAction<GridType> CanDrawDoor()
    {
        return _canDrawDoor;
    }

    private static DrawAction<GridType> _walkable = new DrawAction<GridType>((arr, x, y) =>
    {
        return GridTypeEnum.Walkable(arr[x, y]);
    });
    public static DrawAction<GridType> Walkable()
    {
        return _walkable;
    }

    public static DrawAction<GridSpace> IsType(GridType g)
    {
        return new DrawAction<GridSpace>((arr, x, y) =>
        {
            GridSpace space = arr[x, y];
            return space != null && space.Type == g;
        });
    }

    private static DrawAction<GridType> _drawStairs = Draw.EqualTo(GridType.Floor).
        // If not blocking a path
                And(Draw.NotBlocking<GridType>(GridTypeEnum.Walkable)).
        // If there's a floor around
                And(Draw.Around(false, Draw.EqualTo(GridType.Floor)));
    public static DrawAction<GridType> CanDrawStair()
    {
        return _drawStairs;
    }
    #endregion
}


