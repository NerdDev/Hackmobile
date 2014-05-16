using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface ILayoutObject<T> : IEnumerable<Value2D<T>>
    where T : IGridType
{
    Bounding Bounding { get; }
    bool Contains(int x, int y);
    void Shift(int x, int y);
    Container2D<T> GetGrid();
    List<LayoutObject<T>> Flatten();
    void ToLog(Logs log, params String[] customContent);
}

public static class ILayoutObjectExt
{
    public static void CenterOn<T>(this ILayoutObject<T> obj1, ILayoutObject<T> rhs)
    where T : IGridType
    {
        Point center = obj1.Bounding.GetCenter();
        Point centerRhs = obj1.Bounding.GetCenter();
        obj1.Shift(centerRhs.x - center.x, centerRhs.y - center.y);
    }

    public static void ShiftOutside<T>(this ILayoutObject<T> obj, IEnumerable<ILayoutObject<T>> rhs, Point dir)
    where T : IGridType
    {
        ILayoutObject<T> intersect;
        Point hint;
        while (obj.Intersects(rhs, out intersect, out hint))
        {
            obj.ShiftOutside(intersect, dir, hint, true, true);
        }
    }

    public static void ShiftOutside<T>(this ILayoutObject<T> obj, ILayoutObject<T> rhs, Point dir, Point hint, bool rough, bool finalShift)
    where T : IGridType
    {
        Point reducBase = dir.Reduce();
        Point reduc = new Point(reducBase);
        int xShift, yShift;
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Shift Outside " + obj.ToString());
            BigBoss.Debug.w(Logs.LevelGen, "Shifting outside of " + rhs.ToString());
            BigBoss.Debug.w(Logs.LevelGen, "Shift " + dir + "   Reduc shift: " + reduc);
            BigBoss.Debug.w(Logs.LevelGen, "Bounds: " + obj.Bounding + "  RHS bounds: " + rhs.Bounding);
            var tmp = new MultiMap<T>();
            tmp.PutAll(rhs.GetGrid());
            tmp.PutAll(obj.GetGrid());
            tmp.ToLog(Logs.LevelGen, "Before shifting");
        }
        #endregion
        Point at;
        while (obj.Intersects(rhs, hint, out at))
        {
            if (rough)
            {
                obj.Shift(reduc.x, reduc.y);
                at.Shift(reduc);
                hint = at;
            }
            else
            {
                reduc.Take(out xShift, out yShift);
                obj.Shift(xShift, yShift);
                if (reduc.isZero())
                {
                    reduc = new Point(reducBase);
                }
                at.Shift(xShift, yShift);
                hint = at;
            }
            #region DEBUG
            if (BigBoss.Debug.Flag(DebugManager.DebugFlag.FineSteps) && BigBoss.Debug.logging(Logs.LevelGen))
            {
                BigBoss.Debug.w(Logs.LevelGen, "Intersected at " + at);
                var tmp = new MultiMap<T>();
                tmp.PutAll(rhs.GetGrid());
                tmp.PutAll(obj.GetGrid());
                tmp.ToLog(Logs.LevelGen, "After shifting");
            }
            #endregion
        }
        if (finalShift)
        {
            obj.Shift(dir.x, dir.y);
        }
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printFooter(Logs.LevelGen, "Shift Outside " + obj.ToString());
        }
        #endregion
    }

    public static bool Intersects<T>(this ILayoutObject<T> obj, IEnumerable<ILayoutObject<T>> rhs, out ILayoutObject<T> intersect, out Point at)
    where T : IGridType
    {
        foreach (ILayoutObject<T> l in rhs)
        {
            if (obj.Intersects(l, null, out at))
            {
                intersect = l;
                return true;
            }
        }
        intersect = null;
        at = null;
        return false;
    }

    public static bool Intersects<T>(this ILayoutObject<T> obj, ILayoutObject<T> rhs, Point hint, out Point at)
    where T : IGridType
    {
        if (hint != null && rhs.Contains(hint.x, hint.y))
        {
            at = hint;
            return true;
        }
        foreach (Value2D<T> val in obj)
        {
            if (rhs.Contains(val.x, val.y))
            {
                at = val;
                return true;
            }
        }
        at = null;
        return false;
    }

    public static bool ConnectTo<T>(this ILayoutObject<T> obj1, Point pt1, ILayoutObject<T> obj2, Point pt2, out LayoutObject<T> retObj1, out LayoutObject<T> retObj2)
    where T : IGridType
    {
        if (obj1 is LayoutObjectContainer<T>)
        {
            if (!((LayoutObjectContainer<T>)obj1).GetObjAt(pt1.x, pt1.y, out retObj1))
            {
                retObj2 = null;
                return false;
            }
        }
        else
        {
            retObj1 = (LayoutObject<T>)obj1;
        }

        if (obj2 is LayoutObjectContainer<T>)
        {
            if (!((LayoutObjectContainer<T>)obj2).GetObjAt(pt2.x, pt2.y, out retObj2))
            {
                return false;
            }
        }
        else
        {
            retObj2 = (LayoutObject<T>)obj2;
        }
        retObj1.Connect(retObj2);
        retObj2.Connect(retObj1);
        return true;
    }
}