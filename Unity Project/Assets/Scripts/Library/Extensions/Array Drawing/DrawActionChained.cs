using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DrawActionChained<T> : DrawAction<T>
{
    public DrawAction<T> Chain { get; protected set; }
    private Func<T[,], bool> _initial;
    public override Func<T[,], bool> InitialAction
    {
        get { return _initial; }
        set
        {
            _initial = new Func<T[,], bool>((arr) =>
            {
                if (!value(arr)) return false;
                if (!Chain.InitialAction(arr)) return false;
                return true;
            });
        }
    }
    private Func<T[,], int, int, bool> _unit;
    public override Func<T[,], int, int, bool> UnitAction
    {
        get { return _unit; }
        set
        {
            _unit = new Func<T[,], int, int, bool>((arr, x, y) =>
            {
                if (!value(arr, x, y)) return false;
                if (!Chain.UnitAction(arr, x, y)) return false;
                return true;
            });
        }
    }
    private Func<T[,], int, int, bool> _stroke;
    public override Func<T[,], int, int, bool> StrokeAction
    {
        get { return _stroke; }
        set
        {
            _stroke = new Func<T[,], int, int, bool>((arr, x, y) =>
            {
                if (!value(arr, x, y)) return false;
                if (!Chain.StrokeAction(arr, x, y)) return false;
                return true;
            });
        }
    }
    private Func<T[,], Bounding, bool> _final;
    public override Func<T[,], Bounding, bool> FinalAction
    {
        get { return _final; }
        set
        {
            _final = new Func<T[,], Bounding, bool>((arr, bounds) =>
            {
                if (!value(arr, bounds)) return false;
                if (!Chain.FinalAction(arr, bounds)) return false;
                return true;
            });
        }
    }

    DrawActionChained(DrawAction<T> chain)
    {
        Chain = chain;
        InitialAction = Chain.InitialAction;
        StrokeAction = Chain.StrokeAction;
        UnitAction = Chain.UnitAction;
        FinalAction = Chain.FinalAction;
    }
}
