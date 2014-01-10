using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class Array2DRaw<T> : Container2D<T>
{
    T[,] _arr;

    public Array2DRaw(T[,] arr)
    {
        _arr = arr;
    }

    public override T this[int x, int y] { get { return _arr[y, x]; } set { _arr[y, x] = value; } }

    public override bool TryGetValue(int x, int y, out T val)
    {
        if (InRange(x, y))
        {
            val = _arr[y, x];
            return true;
        }
        val = default(T);
        return false;
    }

    public override int Count
    {
        get { throw new NotImplementedException(); }
    }

    public override Bounding Bounding
    {
        get { throw new NotImplementedException(); }
    }

    public override Array2D<T> Array
    {
        get { throw new NotImplementedException(); }
    }

    public override bool Contains(int x, int y)
    {
        throw new NotImplementedException();
    }

    public override bool InRange(int x, int y)
    {
        return y < _arr.GetLength(0)
            && x < _arr.GetLength(1)
            && y >= 0
            && x >= 0;
    }

    public override void Clear()
    {
        throw new NotImplementedException();
    }

    public override Array2DRaw<T> RawArray(out Point shift)
    {
        shift = new Point();
        return this;
    }

    public override bool Remove(int x, int y)
    {
        throw new NotImplementedException();
    }

    public override IEnumerator<Value2D<T>> GetEnumerator()
    {
        throw new NotImplementedException();
    }

    public override bool DrawAll(DrawActionCall<T> call)
    {
        throw new NotImplementedException();
    }
}
