using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class Draw
{
    public static DrawAction<T> And<T>(params DrawAction<T>[] actions)
    {
        return (arr, x, y) =>
        {
            for (int i = 0; i < actions.Length; i++)
            {
                if (!actions[i](arr, x, y)) return false;
            }
            return true;
        };
    }

    public static DrawAction<T> Or<T>(params DrawAction<T>[] actions)
    {
        return (arr, x, y) =>
        {
            for (int i = 0; i < actions.Length; i++)
            {
                if (actions[i](arr, x, y)) return true;
            }
            return false;
        };
    }

    public static DrawAction<T> True<T>()
    {
        return (arr, x, y) => { return true; };
    }

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

    public static DrawAction<T> Exists<T>()
    {
        return (arr, x, y) =>
        {
            return arr.Contains(x, y);
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
                return arr.HasAround(x, y, cornered, call);
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
    public static DrawAction<GenSpace> CanDrawDoor()
    {
        return new DrawAction<GenSpace>((arr, x, y) =>
        {
            GenSpace space;
            if (!arr.TryGetValue(x, y, out space)) return false;
            if (space.GetGridType() != GridType.Wall) return false;
            // Include null to work with levelgen placement
            if (arr.AlternatesSides(x, y, Draw.IsType<GenSpace>(GridType.NULL).Or(Draw.Walkable()))) return true;
            if (arr.HasAround(x, y, false, Draw.Walkable()) && arr.HasAround(x, y, false, Draw.IsType<GenSpace>(GridType.NULL))) return true;
            return false;
        });
    }

    public static DrawAction<GenSpace> Empty()
    {
        return new DrawAction<GenSpace>((arr, x, y) =>
        {
            GenSpace space;
            if (arr.TryGetValue(x, y, out space))
            {
                if (space.Deploys == null || space.Deploys.Count == 0) return true;
            }
            return false;
        });
    }

    public static DrawAction<GenSpace> EmptyAndFloor()
    {
        return new DrawAction<GenSpace>((arr, x, y) =>
        {
            GenSpace space;
            if (arr.TryGetValue(x, y, out space))
            {
                return space.Type == GridType.Floor && (space.Deploys == null || space.Deploys.Count == 0);
            }
            return false;
        });
    }

    public static DrawAction<GenSpace> EmptyFloorNotBlocking()
    {
        return Draw.EmptyAndFloor().And(Draw.Not(Draw.Blocking(Draw.Walkable())));
    }

    public static DrawAction<GenSpace> Walkable()
    {
        return new DrawAction<GenSpace>((arr, x, y) =>
        {
            GenSpace space;
            if (arr.TryGetValue(x, y, out space))
            {
                if (space == null) return false;
                if (!GridTypeEnum.Walkable(space.Type)) return false;
                if (space.Deploys == null) return true;
                foreach (GenDeploy deploy in space.Deploys)
                {
                    if (!deploy.Element.Walkable) return false;
                }
                return true;
            }
            return false;
        });
    }

    public static DrawAction<T> FloorType<T>()
        where T : IGridSpace
    {
        return new DrawAction<T>((arr, x, y) =>
        {
            return GridTypeEnum.FloorType(arr[x, y].GetGridType());
        });
    }

    public static DrawAction<T> WallType<T>()
        where T : IGridSpace
    {
        return new DrawAction<T>((arr, x, y) =>
        {
            return GridTypeEnum.WallType(arr[x, y].GetGridType());
        });
    }

    public static DrawAction<T> IsType<T>(GridType g)
        where T : IGridSpace
    {
        return (arr, x, y) =>
        {
            return arr.IsType(x, y, g);
        };
    }

    public static DrawAction<T> IsNull<T>()
        where T : IGridSpace
    {
        return IsType<T>(GridType.NULL);
    }

    public static DrawAction<T> IsType<T>(params GridType[] g)
        where T : IGridSpace
    {
        return (arr, x, y) =>
        {
            GridType type;
            T space;
            if (arr.TryGetValue(x, y, out space))
            {
                type = space.Type;
            }
            else
            {
                type = GridType.NULL;
            }
            foreach (GridType t in g)
            {
                if (t == type)
                {
                    return true;
                }
            }
            return false;
        };
    }

    public static DrawAction<T> ContainedIn<T>(ICollection<GridType> col)
        where T : IGridSpace
    {
        return (arr, x, y) =>
        {
            return col.Contains(arr[x, y].GetGridType());
        };
    }

    public static bool IsType<T>(this Container2D<T> cont, int x, int y, GridType type)
        where T : IGridSpace
    {
        T space;
        if (cont.TryGetValue(x, y, out space))
        {
            return type.Equals(cont[x, y].GetGridType());
        }
        return type == GridType.NULL;
    }

    public static DrawAction<T> IsTypeThen<T>(GridType item, DrawAction<T> then, bool not = false)
        where T : IGridSpace
    {
        return (arr, x, y) =>
        {
            if (arr.IsType(x, y, item))
                return then(arr, x, y);
            return true;
        };
    }

    #endregion

    #region GenSpace
    public static void SetTo(this Container2D<GenSpace> cont, Point p, GridType type, Theme theme)
    {
        SetTo(cont, p.x, p.y, type, theme);
    }

    public static void SetTo(this Container2D<GenSpace> cont, int x, int y, GridType type, Theme theme)
    {
        cont[x, y] = new GenSpace(type, theme, x, y);
    }

    public static DrawAction<GenSpace> SetTo(GridType type, Theme theme)
    {
        return (arr, x, y) =>
        {
            arr.SetTo(x, y, type, theme);
            return true;
        };
    }

    public static DrawAction<GenSpace> SetTo(Container2D<GenSpace> cont, GridType type, Theme theme)
    {
        return (arr, x, y) =>
        {
            cont.SetTo(x, y, type, theme);
            return true;
        };
    }

    public static DrawAction<GenSpace> CopyTo(Container2D<GenSpace> cont, Point shift = null)
    {
        if (shift == null)
        {
            return (arr, x, y) =>
            {
                GenSpace space;
                if (arr.TryGetValue(x, y, out space))
                {
                    cont.SetTo(x, y, space.Type, space.Theme);
                }
                return true;
            };
        }
        else
        {
            return (arr, x, y) =>
            {
                GenSpace space;
                if (arr.TryGetValue(x, y, out space))
                {
                    cont.SetTo(x + shift.x, y + shift.y, space.Type, space.Theme);
                }
                return true;
            };
        }
    }

    public static DrawAction<GenSpace> MergeIn(ThemeElement element, Theme theme, GridType type = GridType.Floor, bool typeOnlyDefault = true, bool themeOnlyDefault = false)
    {
        return MergeIn(new GenDeploy(element), theme, type, typeOnlyDefault, themeOnlyDefault);
    }

    public static DrawAction<GenSpace> MergeIn(SmartThemeElement elements, System.Random random, Theme theme, GridType type = GridType.Doodad, bool typeOnlyDefault = false, bool themeOnlyDefault = false)
    {
        return (arr, x, y) =>
        {
            MergeIn(arr, x, y, new GenDeploy(elements.Get(random)), theme, type, typeOnlyDefault, themeOnlyDefault);
            return true;
        };
    }

    public static DrawAction<GenSpace> MergeIn<T>(ProbabilityPool<T> elements, System.Random random, Theme theme, GridType type = GridType.Doodad, bool typeOnlyDefault = false, bool themeOnlyDefault = false)
        where T : ThemeElement
    {
        return (arr, x, y) =>
        {
            MergeIn(arr, x, y, new GenDeploy(elements.Get(random)), theme, type, typeOnlyDefault, themeOnlyDefault);
            return true;
        };
    }

    public static DrawAction<GenSpace> MergeIn(GenDeploy deploy, Theme theme, GridType type = GridType.Floor, bool typeOnlyDefault = true, bool themeOnlyDefault = false)
    {
        return (arr, x, y) =>
        {
            MergeIn(arr, x, y, deploy, theme, type, typeOnlyDefault, themeOnlyDefault);
            return true;
        };
    }

    public static void MergeIn(this Container2D<GenSpace> arr, int x, int y, GenDeploy deploy, Theme theme, GridType type = GridType.Floor, bool typeOnlyDefault = true, bool themeOnlyDefault = false)
    {
        GenSpace space;
        if (!arr.TryGetValue(x, y, out space))
        {
            space = new GenSpace(type, theme, x, y);
            arr[x, y] = space;
        }
        else
        {
            if (!themeOnlyDefault)
            {
                space.Theme = theme;
            }
            if (!typeOnlyDefault)
            {
                space.Type = type;
            }
        }
        space.AddDeploy(deploy, x, y);
    }
    #endregion
}


