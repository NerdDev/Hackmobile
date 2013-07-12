using System.Collections.Generic;
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
        SetPoint(x_, y_);
    }

    public Value2D(int x_, int y_, T val_) : this(x_, y_)
    {
        val = val_;
    }

    public void SetPoint(int x_, int y_)
    {
        x = x_;
        y = y_;
    }

    public Value2D<T> Shift(Point shift)
    {
        return Shift(shift.x, shift.y);
    }

    public Value2D<T> Unshift(Point shift)
    {
        return Unshift(shift.x, shift.y);
    }

    public Value2D<T> Shift(int shiftx, int shifty)
    {
        return new Value2D<T>(x + shiftx, y + shifty, val);
    }

    public Value2D<T> Unshift(int shiftx, int shifty)
    {
        return Shift(-shiftx, -shifty);
    }

    public override string ToString()
    {
        return "Value2D (" + x + "," + y + ", " + val + ")";
    }

    protected bool Equals(Value2D<T> other)
    {
        return x == other.x && y == other.y && EqualityComparer<T>.Default.Equals(val, other.val);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Value2D<T>) obj);
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hashCode = x;
            hashCode = (hashCode*397) ^ y;
            hashCode = (hashCode*397) ^ EqualityComparer<T>.Default.GetHashCode(val);
            return hashCode;
        }
    }

    public static bool operator ==(Value2D<T> left, Value2D<T> right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Value2D<T> left, Value2D<T> right)
    {
        return !Equals(left, right);
    }
}
