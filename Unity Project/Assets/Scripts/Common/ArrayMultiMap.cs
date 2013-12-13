using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#pragma warning disable 162
public class ArrayMultiMap<T> : Container2D<T>
{
    const bool _debug = false;
    public int Count { get; private set; }
    readonly bool[,] _present;
    readonly T[,] _arr;
    bool _accurateBounding = false;
    Bounding bound;
    public int Width { get { return _arr.GetLength(1); } }
    public int Height { get { return _arr.GetLength(0); } }

    public ArrayMultiMap(int width, int height)
    {
        Count = 0;
        _arr = new T[height, width];
        _present = new bool[height, width];
    }

    protected T Take(int x, int y)
    {
        T t = Get(x, y);
        Remove(x, y);
        return t;
    }

    protected override T Get(int x, int y)
    {
        return _arr[y, x];
    }

    public override bool InRange(int x, int y)
    {
        return _arr.InRange(x, y);
    }

    protected override void PutInternal(T val, int x, int y)
    {
        _arr[y, x] = val;
        _present[y, x] = true;
        _accurateBounding = false;
        Count++;
    }

    public void Remove(int x, int y)
    {
        _arr[y, x] = default(T);
        _present[y, x] = false;
        _accurateBounding = false;
        Count++;
    }

    public override Bounding GetBounding()
    {
        if (!_accurateBounding)
        {
            bound = new Bounding();
            for (int y = 0; y < Height; y++)
                for (int x = 0; x < Width; x++)
                    bound.absorb(x, y);
            _accurateBounding = true;
        }
        return bound;
    }

    public override T[,] GetArr()
    {
        return _arr;
    }

    public override List<Value2D<T>> Random(System.Random random, int amount, int distance = 0, bool take = false)
    {
        List<Value2D<T>> ret = new List<Value2D<T>>();
        bool[,] availableArr = new bool[Height, Width];
        Array.Copy(_present, availableArr, _present.Length);
        if (Count < amount)
            amount = Count;
        List<int> pickedList = random.PickSeveral(amount, Count);
        if (pickedList.Count == 0) return ret;
        // Set up by sorting list from biggest to smallest
        pickedList.Sort();
        pickedList.Reverse();
        #region DEBUG
        if (_debug)
        {
            BigBoss.Debug.printHeader("Array Multimap Random amount");
            availableArr.ToLog("Available options");
        }
        #endregion
        int internalCount = Count;
        int run = 1;
        while (pickedList.Count > 0 && internalCount > 0)
        { // Pick until options are gone, or we have enough
            internalCount = PickRandom(pickedList, availableArr, take, distance, internalCount, ret);
            #region DEBUG
            if (_debug)
            {
                availableArr.ToLog("Available options after run " + run++);
            }
            #endregion
            // Pick new random numbers
            pickedList = random.PickSeveral(pickedList.Count, internalCount);
        }
        #region DEBUG
        if (_debug)
        {
            BigBoss.Debug.printFooter();
        }
        #endregion
        return ret;
    }

    protected int PickRandom(List<int> pickedList, bool[,] availableArr, bool take, int distance, int count, List<Value2D<T>> ret)
    {
        int numPassed = 0;
        int listIndex = pickedList.Count - 1;
        int internalCount = count;

        // Walk array and grab picked
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (availableArr[y, x])
                { // Valid space to analyze
                    if (pickedList[listIndex] == numPassed)
                    { // One we picked
                        Value2D<T> pickedVal = new Value2D<T>(x, y, _arr[y, x]);
                        ret.Add(pickedVal);
                        if (take)
                            Remove(x, y);
                        if (distance > 0)
                        { // mark surrounding as not available and decrement our internalCount
                            availableArr.DrawSquare(x, y, distance, new StrokedAction<bool>()
                            {
                                UnitAction = (arr2, x2, y2) =>
                                {
                                    if (arr2.InRange(x2, y2) && availableArr[y2, x2])
                                    {
                                        availableArr[y2, x2] = false;
                                        internalCount--;
                                    }
                                    return true;
                                }
                            });
                        }
                        else
                        { // Just mark taken spot
                            availableArr[pickedVal.y, pickedVal.x] = false;
                        }
                        pickedList.RemoveAt(listIndex);
                        listIndex--;
                        if (listIndex < 0)
                        { // Done picking.  Break
                            #region DEBUG
                            if (_debug)
                            {
                                BigBoss.Debug.printFooter();
                            }
                            #endregion
                            return internalCount;
                        }
                        #region DEBUG
                        if (_debug)
                        {
                            availableArr.ToLog("Available after picking " + pickedVal);
                        }
                        #endregion
                    }
                    numPassed++;
                }
            }
        }
        return internalCount;
    }

    public T Random(System.Random random, bool take = false)
    {
        int picked = random.Next(Count);
        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                if (_present[y, x])
                {
                    if (picked > 0)
                        picked--;
                    else
                    {
                        return take ? Take(x, y) : Get(x, y);
                    }
                }
            }
        }
        return default(T);
    }

    public override IEnumerator<Value2D<T>> GetEnumerator()
    {
        for (int y = 0; y < _arr.GetLength(0); y++)
        {
            for (int x = 0; x < _arr.GetLength(1); x++)
            {
                if (_present[y,x])
                    yield return new Value2D<T>(x, y, _arr[y,x]);
            }
        }
    }
}
#pragma warning restore 162
