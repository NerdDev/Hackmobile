using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SquareFinder<T>
{
    T[,] _arr;
    int _width;
    int _height;
    DrawTest<T> _tester;
    int _x = 0;
    int _y = 0;
    bool _tryFlipped;
    int _lowestFail = int.MaxValue;
    Rectangle _scope;
    Func<T[,], int, int, bool> fillTest = null;
    Func<T[,], int, int, bool> strokeTest = null;
    public bool Single { get; set; }

    public SquareFinder(T[,] arr, int width, int height, bool tryFlipped, DrawTest<T> tester, Rectangle scope = null)
    {
        _arr = arr;
        _width = width;
        _height = height;
        _tryFlipped = tryFlipped && width != height;
        _tester = tester;
        _scope = scope;
        if (scope != null)
        {
            _x = scope.Left;
            _y = scope.Bottom;
        }
        fillTest = GetTest(tester.UnitTest);
        if (tester.StrokeTest != null)
            strokeTest = GetTest(tester.StrokeTest);
        Single = false;
    }

    protected Func<T[,], int, int, bool> GetTest(Func<T[,], int, int, bool> userTest)
    {
        return new Func<T[,], int, int, bool>((af, xf, yf) =>
                    { // For each in test square, run user's test
                        if (!userTest(af, xf, yf))
                        { // If desired test fails, stop and record position
                            _lowestFail = Math.Min(_y, _lowestFail);
                            _x = xf + 1; // Move our next test square right of failure.
                            return false;
                        }
                        return true;
                    }
            );
    }

    public List<Rectangle> Find()
    {
        List<Rectangle> ret = new List<Rectangle>();
        if (_width >= _arr.GetLength(1) || _height >= _arr.GetLength(0)) return ret;

        while (_y + _height <= _arr.GetLength(0) && (_scope == null || _y + _height <= _scope.Right))
        { // In range vertically
            if (_tester.InitialTest == null || _tester.InitialTest(_arr))
            { // Passed initial test
                // Try to draw a square that passes the user's test
                if (_arr.DrawSquare(_x, _x + _width - 1, _y, _y + _height - 1, fillTest, strokeTest))
                { // Found a square
                    if (_tester.FinalTest == null || _tester.FinalTest(_arr))
                    {
                        ret.Add(new Rectangle() { Left = _x, Bottom = _y, Right = _x + _width - 1, Top = _y + _height - 1 });
                        if (Single)
                            return ret;
                    }
                    // Move over one
                    _x++;
                }
            }

            // Test to see if next is out of range horizontally
            if (_x + _width > _arr.GetLength(1) && (_scope == null || _x + _width <= _scope.Left))
            { // it is, so reset to left of array and move up
                _x = 0;
                _lowestFail++;
                _y = _lowestFail;
            }
        }

        if (_tryFlipped && ret.Count == 0)
        { // Flip and try again
            _tryFlipped = false;
            int tmp = _width;
            _height = _width;
            _width = _height;
            return Find();
        }
        return ret;
    }
}
