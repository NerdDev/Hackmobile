using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class DrawTestChained<T> : DrawTest<T>
{
    public DrawTest<T> Chain { get; protected set; }
    private Func<T[,], bool> _initial;
    public override Func<T[,], bool> InitialTest
    {
        get { return _initial; }
        set
        {
            _initial = new Func<T[,], bool>((arr) =>
            {
                if (!value(arr)) return false;
                if (!Chain.InitialTest(arr)) return false;
                return true;
            });
        }
    }
    private Func<T[,], int, int, bool> _unit;
    public override Func<T[,], int, int, bool> UnitTest
    {
        get { return _unit; }
        set
        {
            _unit = new Func<T[,], int, int, bool>((arr, x, y) =>
            {
                if (!value(arr, x, y)) return false;
                if (!Chain.UnitTest(arr, x, y)) return false;
                return true;
            });
        }
    }
    private Func<T[,], int, int, bool> _stroke;
    public override Func<T[,], int, int, bool> StrokeTest
    {
        get { return _stroke; }
        set
        {
            _stroke = new Func<T[,], int, int, bool>((arr, x, y) =>
            {
                if (!value(arr, x, y)) return false;
                if (!Chain.StrokeTest(arr, x, y)) return false;
                return true;
            });
        }
    }
    private Func<T[,], Bounding, bool> _final;
    public override Func<T[,], Bounding, bool> FinalTest
    {
        get { return _final; }
        set
        {
            _final = new Func<T[,], Bounding, bool>((arr, bounds) =>
            {
                if (!value(arr, bounds)) return false;
                if (!Chain.FinalTest(arr, bounds)) return false;
                return true;
            });
        }
    }

    DrawTestChained(DrawTest<T> chain)
    {
        Chain = chain;
    }
}
