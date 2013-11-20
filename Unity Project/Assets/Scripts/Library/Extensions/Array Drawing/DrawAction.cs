using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DrawActions<T>
{
    public virtual Func<T[,], int, int, bool> UnitAction { get; set; }
    public virtual Func<T[,], int, int, bool> StrokeAction { get; set; }

    public DrawActions()
    {
    }

    public static implicit operator DrawActions<T>(Func<T[,], int, int, bool> normalFunc)
    {
        return new DrawActions<T>() { UnitAction = normalFunc };
    }

    public static DrawActionChained<T> operator +(DrawActions<T> c1, DrawActions<T> c2)
    {
        return new DrawActionChained<T>(c1)
            {
                UnitAction = c2.UnitAction,
                StrokeAction = c2.StrokeAction
            };
    }
}
