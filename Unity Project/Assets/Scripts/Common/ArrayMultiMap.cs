using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//#pragma warning disable 162
//public class ArrayMultiMap<T> : Container2D<T>
//{
//    const bool _debug = false;
//    public int Count { get; private set; }
//    readonly Array2D<bool> _present;
//    readonly Array2D<T> _arr;
//    bool _accurateBounding = false;
//    Bounding bound;
//    public int Width { get { return _arr.Width; } }
//    public int Height { get { return _arr.Height; } }

//    public ArrayMultiMap(int width, int height)
//    {
//        Count = 0;
//        _arr = new Array2D<T>(height, width);
//        _present = new Array2D<bool>(height, width);
//    }

//    protected T Take(int x, int y)
//    {
//        T t = Get(x, y);
//        Remove(x, y);
//        return t;
//    }

//    protected override T Get(int x, int y)
//    {
//        return _arr[x, y];
//    }

//    public override bool InRange(int x, int y)
//    {
//        return _arr.InRange(x, y);
//    }

//    protected override void PutInternal(T val, int x, int y)
//    {
//        _arr[x, y] = val;
//        _present[x, y] = true;
//        _accurateBounding = false;
//        Count++;
//    }

//    public void Remove(int x, int y)
//    {
//        _arr[x, y] = default(T);
//        _present[x, y] = false;
//        _accurateBounding = false;
//        Count++;
//    }

//    public override Bounding GetBounding()
//    {
//        if (!_accurateBounding)
//        {
//            bound = new Bounding();
//            for (int y = 0; y < Height; y++)
//                for (int x = 0; x < Width; x++)
//                    bound.Absorb(x, y);
//            _accurateBounding = true;
//        }
//        return bound;
//    }

//    public override List<Value2D<T>> Random(System.Random random, int amount, int distance = 0, bool take = false)
//    {
//        List<Value2D<T>> ret = new List<Value2D<T>>();
//        Array2D<bool> availableArr = new Array2D<bool>(_present);
//        Array.Copy(_present.GetArr(), availableArr.GetArr(), _present.GetArr().Length);
//        if (Count < amount)
//            amount = Count;
//        List<int> pickedList = random.PickSeveral(amount, Count);
//        if (pickedList.Count == 0) return ret;
//        // Set up by sorting list from biggest to smallest
//        pickedList.Sort();
//        pickedList.Reverse();
//        #region DEBUG
//        if (_debug)
//        {
//            BigBoss.Debug.printHeader("Array Multimap Random amount");
//            availableArr.ToLog("Available options");
//        }
//        #endregion
//        int internalCount = Count;
//        int run = 1;
//        while (pickedList.Count > 0 && internalCount > 0)
//        { // Pick until options are gone, or we have enough
//            internalCount = PickRandom(pickedList, availableArr, take, distance, internalCount, ret);
//            #region DEBUG
//            if (_debug)
//            {
//                availableArr.ToLog("Available options after run " + run++);
//            }
//            #endregion
//            // Pick new random numbers
//            pickedList = random.PickSeveral(pickedList.Count, internalCount);
//        }
//        #region DEBUG
//        if (_debug)
//        {
//            BigBoss.Debug.printFooter("Array Multimap Random amount");
//        }
//        #endregion
//        return ret;
//    }

//    protected int PickRandom(List<int> pickedList, Array2D<bool> availableArr, bool take, int distance, int count, List<Value2D<T>> ret)
//    {
//        int numPassed = 0;
//        int listIndex = pickedList.Count - 1;
//        int internalCount = count;

//        // Walk array and grab picked
//        for (int y = 0; y < Height; y++)
//        {
//            for (int x = 0; x < Width; x++)
//            {
//                if (availableArr[x, y])
//                { // Valid space to analyze
//                    if (pickedList[listIndex] == numPassed)
//                    { // One we picked
//                        Value2D<T> pickedVal = new Value2D<T>(x, y, _arr[x, y]);
//                        ret.Add(pickedVal);
//                        if (take)
//                            Remove(x, y);
//                        if (distance > 0)
//                        { // mark surrounding as not available and decrement our internalCount
//                            availableArr.DrawSquare(x, y, distance, new StrokedAction<bool>()
//                            {
//                                UnitAction = (arr2, x2, y2) =>
//                                {
//                                    if (arr2.InRange(x2, y2) && availableArr[y2, x2])
//                                    {
//                                        availableArr[y2, x2] = false;
//                                        internalCount--;
//                                    }
//                                    return true;
//                                }
//                            });
//                        }
//                        else
//                        { // Just mark taken spot
//                            availableArr[pickedVal.y, pickedVal.x] = false;
//                        }
//                        pickedList.RemoveAt(listIndex);
//                        listIndex--;
//                        if (listIndex < 0)
//                        { // Done picking.  Break
//                            return internalCount;
//                        }
//                        #region DEBUG
//                        if (_debug)
//                        {
//                            availableArr.ToLog("Available after picking " + pickedVal);
//                        }
//                        #endregion
//                    }
//                    numPassed++;
//                }
//            }
//        }
//        return internalCount;
//    }

//    public T Random(System.Random random, bool take = false)
//    {
//        int picked = random.Next(Count);
//        for (int y = 0; y < Height; y++)
//        {
//            for (int x = 0; x < Width; x++)
//            {
//                if (_present[x, y])
//                {
//                    if (picked > 0)
//                        picked--;
//                    else
//                    {
//                        return take ? Take(x, y) : Get(x, y);
//                    }
//                }
//            }
//        }
//        return default(T);
//    }

//    public override IEnumerator<Value2D<T>> GetEnumerator()
//    {
//        for (int y = 0; y < _arr.Height; y++)
//        {
//            for (int x = 0; x < _arr.Width; x++)
//            {
//                if (_present[x, y])
//                    yield return new Value2D<T>(x, y, _arr[x, y]);
//            }
//        }
//    }
//}
//#pragma warning restore 162
