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

    public SquareFinder(Container2D<T> arr, int width, int height, bool tryFlipped, StrokedAction<T> tester, Bounding scope = null)
    {
        _arr = arr;
        _width = width;
        _height = height;
        _tryFlipped = tryFlipped && width != height;
        _tester = tester;
        _scope = scope;
        if (scope == null) scope = arr.Bounding;
    }

    public List<Bounding> Find()
    {
        List<Bounding> ret = new List<Bounding>();
        if (_width >= _arr.Width || _height >= _arr.Height) return ret;

        if (_tester.StrokeAction == null)
        {
            FindSkip(ret);
        }
        else if (_tester.UnitAction != null && _width > 3 && _height > 3)
        { // Find non-stroke options, then test stroke
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
                b.expand(1);
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
        int _x = _scope.XMin;
        int _y = _scope.YMin;
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
        int _x = _scope.XMin;
        int _y = _scope.YMin;
        int _lastPassedX = int.MaxValue;
        int _lastTestedX = _scope.XMin;
        int _lastPassedY = _scope.YMin;
        int xShift = _arr.Bounding.XMin;
        int xMax = Math.Min(_arr.Bounding.XMax - _width, _scope == null ? _scope.XMax - _width : int.MaxValue);
        int yMax = Math.Min(_arr.Bounding.YMax - _height, _scope == null ? _scope.YMax - _height : int.MaxValue);
        while (_y <= yMax)
        { // In range vertically
            BigBoss.Debug.w(Logs.LevelGenMain, "Testing " + _x + " " + _y);
            // Try to draw a square that passes the user's test
            if (_arr.DrawSquare(_x, _x + _width - 1, _y, _y + _height - 1, _tester))
            { // Found a square
                if (_lastPassedX == _lastTestedX)
                { // Draw all previous squares
                    for (int _tmpX = _lastPassedX; _tmpX <= _x; _tmpX++)
                    {
                        BigBoss.Debug.w(Logs.LevelGenMain, "Accepting " + _tmpX + " " + _y);
                        ret.Add(new Bounding() { XMin = _tmpX, YMin = _y, XMax = _tmpX + _width - 1, YMax = _y + _height - 1 });
                    }
                }
                else
                {
                    for (int _tmpX = _lastTestedX + 1; _tmpX <= _x - 1; _tmpX++)
                    {
                        BigBoss.Debug.w(Logs.LevelGenMain, "Not last pass Testing " + _tmpX + " " + _y);
                        if (_arr.DrawSquare(_tmpX, _tmpX + _width - 1, _y, _y + _height - 1, _tester))
                        {
                            BigBoss.Debug.w(Logs.LevelGenMain, "Not last pass Accepting " + _tmpX + " " + _y);
                            ret.Add(new Bounding() { XMin = _tmpX, YMin = _y, XMax = _tmpX + _width - 1, YMax = _y + _height - 1 });
                            _lastPassedX = _x;
                        }
                    }
                }
                // Set last passed to cur square
                _lastPassedX = _x;
                _lastTestedX = _x;
            }
            else
            { // Square failed, test previous squares
                for (int _tmpX = _lastTestedX + 1; _tmpX <= _x - 1; _tmpX++)
                {
                    BigBoss.Debug.w(Logs.LevelGenMain, "Fail Testing " + _tmpX + " " + _y);
                    if (_arr.DrawSquare(_tmpX, _tmpX + _width - 1, _y, _y + _height - 1, _tester))
                    {
                        BigBoss.Debug.w(Logs.LevelGenMain, "Fail Accepting " + _tmpX + " " + _y);
                        ret.Add(new Bounding() { XMin = _tmpX, YMin = _y, XMax = _tmpX + _width - 1, YMax = _y + _height - 1 });
                        _lastPassedX = _x;
                    }
                }
                _lastTestedX = _x;
            }
            // Skip to next full untested square
            _x += _width;

            // Test to see if next is out of range horizontally
            if (_x > xMax)
            {
                // Roll back and check the last few skipped boxes
                for (int _tmpX = _lastTestedX + 1; _tmpX <= xMax; _tmpX++)
                {
                    BigBoss.Debug.w(Logs.LevelGenMain, "End Testing " + _tmpX + " " + _y);
                    if (_arr.DrawSquare(_tmpX, _tmpX + _width - 1, _y, _y + _height - 1, _tester))
                    {
                        BigBoss.Debug.w(Logs.LevelGenMain, "End Accepting " + _tmpX + " " + _y);
                        ret.Add(new Bounding() { XMin = _tmpX, YMin = _y, XMax = _tmpX + _width - 1, YMax = _y + _height - 1 });
                    }
                }
                // go up one
                _x = _scope.XMin;
                _lastPassedY = _y;
                _lastPassedX = int.MaxValue;
                _lastTestedX = _scope.XMin;
                _y++;
            }
        }
    }
}
