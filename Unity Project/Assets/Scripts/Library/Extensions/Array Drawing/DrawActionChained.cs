using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DrawActionChained<T> : DrawActions<T>
{
    public DrawActions<T> Chain { get; protected set; }
    private Func<T[,], int, int, bool> _unit;
    public override Func<T[,], int, int, bool> UnitAction
    {
        get { return _unit; }
        set
        {
            if (value == null) return;
            if (Chain.UnitAction == null)
            {
                _unit = value;
            }
            else
            {
                _unit = new Func<T[,], int, int, bool>((arr, x, y) =>
                {
                    if (!value(arr, x, y)) return false;
                    if (!Chain.UnitAction(arr, x, y)) return false;
                    return true;
                });
            }
        }
    }
    private Func<T[,], int, int, bool> _stroke;
    public override Func<T[,], int, int, bool> StrokeAction
    {
        get { return _stroke; }
        set
        {
            if (value == null) return;
            if (Chain.StrokeAction == null)
            {
                _stroke = value;
            }
            else
            {
                _stroke = new Func<T[,], int, int, bool>((arr, x, y) =>
                {
                    if (!value(arr, x, y)) return false;
                    if (!Chain.StrokeAction(arr, x, y)) return false;
                    return true;
                });
            }
        }
    }

    public DrawActionChained(DrawActions<T> chain)
    {
        Chain = chain;
        StrokeAction = Chain.StrokeAction;
        UnitAction = Chain.UnitAction;
    }
}
