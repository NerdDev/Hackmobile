using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class SquareFinder<T>
{
    Container2D<T> _arr;
    int _width;
    int _height;
    StrokedAction<T> _tester;
    bool _tryFlipped;
    Bounding _scope;

    // Used internally
    int _x;
    int _y;
    int _lastTestedX;
    int _lastPassedX;
    int _lastTestedY;
    int xShift;
    bool[] previousLastPassedX;
    bool[] _lastPassedXArr;
    Counter counter = new Counter();

    public SquareFinder(Container2D<T> arr, int width, int height, bool tryFlipped, StrokedAction<T> tester, Bounding scope = null)
    {
        _arr = arr;
        _width = width;
        _height = height;
        _tryFlipped = tryFlipped && width != height;
        _tester = tester;
        _scope = scope;
        if (scope == null) scope = arr.Bounding;
        if (_tester.StrokeAction != null)
            _tester.StrokeAction = Draw.Count<T>(counter).And(_tester.StrokeAction);
        if (_tester.UnitAction != null)
            _tester.UnitAction = Draw.Count<T>(counter).And(_tester.UnitAction);
    }

    public List<Bounding> Find()
    {
        List<Bounding> ret = new List<Bounding>();
        if (_width >= _arr.Width || _height >= _arr.Height) return ret;

        // Initialize
        _x = _scope.XMin;
        _y = _scope.YMin;
        _lastPassedX = int.MaxValue;
        _lastTestedX = _scope.XMin;
        _lastTestedY = _scope.YMin;
        xShift = _arr.Bounding.XMin;
        _lastPassedXArr = new bool[_arr.Width];

        if (_tester.StrokeAction == null && _width > 3 && _height > 3)
        { // Just unit
            FindSkip(ret);
        }
        else if (_tester.StrokeAction != null && _tester.UnitAction != null && _width > 5 && _height > 5)
        { // Unit and stroke with optimization
            // Find non-stroke options, then test stroke
            DrawAction<T> stroke = _tester.StrokeAction;
            _tester.StrokeAction = null;
            _width -= 2;
            _height -= 2;
            FindSkip(ret);
            _width += 2;
            _height += 2;
            _tester.StrokeAction = stroke;
            List<Bounding> retTmp = new List<Bounding>(ret);
            ret.Clear();
            StrokedAction<T> strokeTest = new StrokedAction<T>() { StrokeAction = _tester.StrokeAction };
            foreach (Bounding b in retTmp)
            {
                b.Expand(1);
                if (_arr.DrawSquare(b.XMin, b.XMax, b.YMin, b.YMax, strokeTest))
                {
                    ret.Add(b);
                }
            }
        }
        else
        {
            FindNormal(ret);
        }

        if (_tryFlipped && ret.Count == 0)
        { // Flip and try again
            _tryFlipped = false;
            int tmp = _width;
            _width = _height;
            _height = tmp;
            return Find();
        }
        return ret;
    }

    protected void FindNormal(List<Bounding> ret)
    {
        while (_y <= _scope.YMax)
        { // In range vertically
            // Try to draw a square that passes the user's test
            if (_arr.DrawSquare(_x, _x + _width - 1, _y, _y + _height - 1, _tester))
            { // Found a square
                Bounding b = new Bounding() { XMin = _x, YMin = _y, XMax = _x + _width - 1, YMax = _y + _height - 1 };
                ret.Add(b);
            }
            // Move over one
            _x++;

            // Test to see if next is out of range horizontally
            if (_x > _scope.XMax)
            { // it is, so reset to left of array and move up
                _x = _scope.XMin;
                _y++;
            }
        }
    }

    protected void FindSkip(List<Bounding> ret)
    {
        while (_y <= _scope.YMax)
        { // In range vertically
            // Try to draw a square that passes the user's test
            if (_arr.DrawSquare(_x, _x + _width - 1, _y, _y + _height - 1, _tester))
            { // Found a square
                if (_lastPassedX == _lastTestedX)
                { // Accept all previous squares
                    for (int _tmpX = _lastTestedX + 1; _tmpX <= _x; _tmpX++)
                    { // Accept horizontal
                        ret.Add(new Bounding() { XMin = _tmpX, YMin = _y, XMax = _tmpX + _width - 1, YMax = _y + _height - 1 });
                        HandleYSquares(ret, _tmpX, true);
                        _lastPassedXArr[_tmpX - xShift] = true;
                    }
                }
                else
                { // Last test wasn't passed, so we cannot assume previous passed
                    TestXSquares(ret, _x - 1);
                    ret.Add(new Bounding() { XMin = _x, YMin = _y, XMax = _x + _width - 1, YMax = _y + _height - 1 });
                    HandleYSquares(ret, _x, true);
                    _lastPassedXArr[_x - xShift] = true;
                }
                _lastPassedX = _x;
            }
            else
            { // Square failed, test previous squares
                TestXSquares(ret, _x - 1);
                HandleYSquares(ret, _x, false);
            }
            // Skip to next full untested square
            _lastTestedX = _x;
            _x += _width;

            // Test to see if next is out of range horizontally
            if (_x > _scope.XMax)
            {
                // Roll back and check the last few skipped boxes
                TestXSquares(ret, _scope.XMax);
                // go up one
                _x = _scope.XMin;
                _lastPassedX = int.MaxValue;
                _lastTestedX = _scope.XMin;
                _lastTestedY = _y;
                _y += _width;
            }
        }
    }

    protected void TestXSquares(List<Bounding> ret, int to)
    {
        for (int _tmpX = _lastTestedX + 1; _tmpX <= to; _tmpX++)
        {
            if (_arr.DrawSquare(_tmpX, _tmpX + _width - 1, _y, _y + _height - 1, _tester))
            {
                ret.Add(new Bounding() { XMin = _tmpX, YMin = _y, XMax = _tmpX + _width - 1, YMax = _y + _height - 1 });
                HandleYSquares(ret, _tmpX, true);
                _lastPassedX = _x;
                _lastPassedXArr[_tmpX - xShift] = true;
            }
            else
            {
                HandleYSquares(ret, _tmpX, false);
            }
        }
    }

    public void HandleYSquares(List<Bounding> ret, int atX, bool xPassed)
    {
        for (int _tmpY = _lastTestedY + 1; _tmpY <= _y - 1; _tmpY++)
        {
            if ((xPassed && _lastPassedXArr[atX - xShift]) || _arr.DrawSquare(atX, atX + _width - 1, _tmpY, _tmpY + _height - 1, _tester))
            {
                ret.Add(new Bounding() { XMin = atX, YMin = _tmpY, XMax = atX + _width - 1, YMax = _tmpY + _height - 1 });
            }
        }
    }
}
