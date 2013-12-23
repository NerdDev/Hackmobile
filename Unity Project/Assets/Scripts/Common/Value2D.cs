using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class Value2D<T> : Point {

    public T val { get; set; }
    
    public Value2D()
        : base()
    {
    }

    public Value2D(int x, int y)
        : base(x, y)
    {
    }

    public Value2D(float x, float y)
        : base(x, y)
    {
    }
	
	public Value2D(Vector2 vect) 
        : base(vect)
	{
	}
	
	public Value2D (Point rhs) 
        : base(rhs)
	{
			
	}

    public Value2D(int x_, int y_, T val_) : this(x_, y_)
    {
        val = val_;
    }

    public Value2D(Value2D<T> rhs)
        : base(rhs)
    {
        val = rhs.val;
    }

    public override string ToString()
    {
        return "Value2D (" + x + "," + y + ", " + val + ")";
    }

    public override bool Equals(object obj)
    {
        Value2D<T> rhs = obj as Value2D<T>;
        if (rhs == null) return false;
        if (x != rhs.x || y != rhs.y) return false;
        if (val == null) return rhs.val == null;
        if (rhs.val == null) return false;
        return val.Equals(rhs.val);
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
