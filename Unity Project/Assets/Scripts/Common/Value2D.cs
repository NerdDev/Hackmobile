using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Point<T> {

    public int x { get; set; }
    public int y { get; set; }
    public T val { get; set; }

    public Point()
    {
    }

    public Point(int x_, int y_) : this()
    {
        SetPoint(x_, y_);
    }

    public Point(int x_, int y_, T val_) : this(x_, y_)
    {
        val = val_;
    }

    public void SetPoint(int x_, int y_)
    {
        x = x_;
        y = y_;
    }

    public Point<T> Shift(Point shift)
    {
        return Shift(shift.x, shift.y);
    }

    public Point<T> Unshift(Point shift)
    {
        return Unshift(shift.x, shift.y);
    }

    public Point<T> Shift(int shiftx, int shifty)
    {
        return new Point<T>(x + shiftx, y + shifty, val);
    }

    public Point<T> Unshift(int shiftx, int shifty)
    {
        return Shift(-shiftx, -shifty);
    }

    public override string ToString()
    {
        return "Value2D (" + x + "," + y + ", " + val + ")";
    }

    protected bool Equals(Point<T> other)
    {
        return x == other.x && y == other.y && EqualityComparer<T>.Default.Equals(val, other.val);
    }

    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((Point<T>) obj);
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

    public static bool operator ==(Point<T> left, Point<T> right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Point<T> left, Point<T> right)
    {
        return !Equals(left, right);
    }

    public static implicit operator Point(Point<T> val)
    {
        return new Point(val.x, val.y);
    }
}
