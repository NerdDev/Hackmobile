using UnityEngine;
using System.Collections;

public class Value2D<T> {

    public int x { get; set; }
    public int y { get; set; }
    public T val { get; set; }

    public Value2D()
    {
    }

    public Value2D(int x_, int y_) : this()
    {
        setPoint(x_, y_);
    }

    public Value2D(int x_, int y_, T val_) : this(x_, y_)
    {
        val = val_;
    }

    public void setPoint(int x_, int y_)
    {
        x = x_;
        y = y_;
    }

    public override string ToString()
    {
        return "Value2D (" + x + "," + y + ", " + val + ")";
    }
}
