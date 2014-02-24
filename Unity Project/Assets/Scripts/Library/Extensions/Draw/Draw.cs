using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class Draw
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
        return SetTo<T, T>(container, g, shift);
    }

    public static DrawAction<T> SetTo<T, R>(Container2D<R> container, R g, Point shift = null)
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

    public static DrawAction<T> Set<T>(Value2D<T> val)
    {
        return (arr, x, y) =>
        {
            val.val = arr[x, y];
            val.x = x;
            val.y = y;
            return true;
        };
    }

    public static DrawAction<T> Set<T>(Point val)
    {
        return (arr, x, y) =>
        {
            val.x = x;
            val.y = y;
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

    public static DrawAction<T> AddTo<T>(ICollection<Point> t)
    {
        return (arr, x, y) =>
        {
            t.Add(new Point(x, y));
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

    public static DrawAction<T> AddTo<T>(Queue<Point> queue, Point shift = null)
    {
        if (shift == null)
        {
            return (arr, x, y) =>
            {
                queue.Enqueue(new Point(x, y));
                return true;
            };
        }
        else
        {
            return (arr, x, y) =>
            {
                queue.Enqueue(new Point(x + shift.x, y + shift.y));
                return true;
            };
        }
    }

    public static DrawAction<T> AddTo<T>(Queue<Value2D<T>> queue, Point shift = null)
    {
        if (shift == null)
        {
            return (arr, x, y) =>
            {
                queue.Enqueue(new Value2D<T>(x, y, arr[x, y]));
                return true;
            };
        }
        else
        {
            return (arr, x, y) =>
            {
                queue.Enqueue(new Value2D<T>(x + shift.x, y + shift.y, arr[x, y]));
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

    /*
     * Walk edges and if alternates more than twice, it's blocking
     */
    public static DrawAction<T> Blocking<T>(DrawAction<T> action)
    {
        return (arr, x, y) =>
        {
            int count = 0;
            bool status = action(arr, x - 1, y - 1); // Bottom left
            DrawAction<T> func = (arr2, x2, y2) =>
            {
                if (action(arr2, x2, y2) != status)
                {
                    status = !status;
                    return ++count > 2;
                }
                return false;
            };
            if (func(arr, x, y - 1)) return true; // Bottom
            if (func(arr, x + 1, y - 1)) return true; // Bottom right
            if (func(arr, x + 1, y)) return true; // Right
            if (func(arr, x + 1, y + 1)) return true; // Top Right
            if (func(arr, x, y + 1)) return true; // Top
            if (func(arr, x - 1, y + 1)) return true; // Top Left
            if (func(arr, x - 1, y)) return true; // Left
            return false;
        };
    }

    #region GridType
    private static DrawAction<GridSpace> _canDrawDoor = new DrawAction<GridSpace>((arr, x, y) =>
    {
        GridSpace space;
        if (!arr.TryGetValue(x, y, out space)) return false;
        if (space.GetGridType() != GridType.Wall) return false;
        // Include null to work with levelgen placement
        if (arr.AlternatesSides(x, y, Draw.IsType(GridType.NULL).Or(Draw.Walkable()))) return true;
        if (arr.HasAround(x, y, false, Draw.Walkable()) && arr.HasAround(x, y, false, Draw.IsType(GridType.NULL))) return true;
        return false;
    });
    public static DrawAction<GridSpace> CanDrawDoor()
    {
        return _canDrawDoor;
    }

    private static DrawAction<GridSpace> _walkable = new DrawAction<GridSpace>((arr, x, y) =>
    {
        return GridTypeEnum.Walkable(arr[x, y].GetGridType());
    });
    public static DrawAction<GridSpace> Walkable()
    {
        return _walkable;
    }

    private static DrawAction<GridSpace> _floorType = new DrawAction<GridSpace>((arr, x, y) =>
    {
        return GridTypeEnum.FloorType(arr[x, y].GetGridType());
    });
    public static DrawAction<GridSpace> FloorType()
    {
        return _floorType;
    }

    private static DrawAction<GridSpace> _wallType = new DrawAction<GridSpace>((arr, x, y) =>
    {
        return GridTypeEnum.WallType(arr[x, y].GetGridType());
    });
    public static DrawAction<GridSpace> WallType()
    {
        return _wallType;
    }

    private static DrawAction<GridSpace> _drawStairs = Draw.IsType(GridType.Floor).
        // If not blocking a path
                And(Draw.Not(Draw.Blocking(Draw.Walkable()))).
        // If there's a floor around
                And(Draw.Around(false, Draw.IsType(GridType.Floor)));
    public static DrawAction<GridSpace> CanDrawStair()
    {
        return _drawStairs;
    }

    public static DrawAction<GridSpace> IsType(GridType g)
    {
        return (arr, x, y) =>
        {
            return arr.IsType(x, y, g);
        };
    }
    public static DrawAction<GridSpace> ContainedIn(ICollection<GridType> col)
    {
        return (arr, x, y) =>
        {
            return col.Contains(arr[x, y].GetGridType());
        };
    }

    public static DrawAction<GridSpace> SetToIfNotEqual(GridType not, GridType to)
    {
        return (arr, x, y) =>
        {
            if (!arr[x, y].Type.Equals(not))
                SetTo(arr, x, y, to);
            return true;
        };
    }

    public static void SetTo(this Container2D<GridSpace> cont, int x, int y, GridType type)
    {
        GridSpace space;
        if (cont.TryGetValue(x, y, out  space))
        {
            space.Type = type;
        }
        else
        {
            cont[x, y] = new GridSpace(type, x, y);
        }
    }

    public static bool IsType(this Container2D<GridSpace> cont, int x, int y, GridType type)
    {
        GridSpace space;
        if (cont.TryGetValue(x, y, out space))
        {
            return type.Equals(cont[x, y].GetGridType());
        }
        return type == GridType.NULL;
    }

    public static DrawAction<GridSpace> SetTo(GridType from, GridType to)
    {
        return (arr, x, y) =>
        {
            GridSpace space;
            if (arr.TryGetValue(x, y, out space))
            {
                if (arr[x, y].Type.Equals(from))
                    arr.SetTo(x, y, to);
            }
            else if (from == GridType.NULL)
            {
                arr.SetTo(x, y, to);
            }
            return true;
        };
    }

    public static DrawAction<GridSpace> SetTo(GridType g)
    {
        return (arr, x, y) =>
        {
            SetTo(arr, x, y, g);
            return true;
        };
    }

    public static DrawAction<GridSpace> IsTypeThen(GridType item, DrawAction<GridSpace> then, bool not = false)
    {
        return (arr, x, y) =>
        {
            if (arr.IsType(x, y, item))
                return then(arr, x, y);
            return true;
        };
    }

    public static DrawAction<GridSpace> SetTo(Container2D<GridSpace> container, GridType g, Point shift = null)
    {
        if (shift == null)
        {
            return (arr, x, y) =>
            {
                container.SetTo(x, y, g);
                return true;
            };
        }
        else
        {
            return (arr, x, y) =>
            {
                container.SetTo(x + shift.x, y + shift.y, g);
                return true;
            };
        }
    }
    #endregion
}


