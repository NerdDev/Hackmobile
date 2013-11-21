using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DrawActionChained<T> : DrawActions<T>
{
    public DrawActions<T> Chain { get; protected set; }
    private DrawAction<T> _unit;
    public override DrawAction<T> UnitAction
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
                _unit = new DrawAction<T>((arr, x, y) =>
                {
                    if (!value(arr, x, y)) return false;
                    if (!Chain.UnitAction(arr, x, y)) return false;
                    return true;
                });
            }
        }
    }
    private DrawAction<T> _stroke;
    public override DrawAction<T> StrokeAction
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
                _stroke = new DrawAction<T>((arr, x, y) =>
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
