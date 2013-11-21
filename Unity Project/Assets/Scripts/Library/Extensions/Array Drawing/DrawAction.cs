using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DrawActions<T>
{
    public virtual DrawAction<T> UnitAction { get; set; }
    public virtual DrawAction<T> StrokeAction { get; set; }

    public DrawActions()
    {
    }

    public static implicit operator DrawActions<T>(DrawAction<T> normalFunc)
    {
        return new DrawActions<T>() { UnitAction = normalFunc };
    }

    public static implicit operator DrawAction<T>(DrawActions<T> actions)
    {
        return actions.UnitAction;
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
