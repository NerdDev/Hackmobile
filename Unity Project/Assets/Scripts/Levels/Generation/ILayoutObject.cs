using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public interface ILayoutObject : IEnumerable<Value2D<GridType>>
{
    Bounding Bounding { get; }
    bool ContainsPoint(Point pt);
    void Shift(int x, int y);
    Container2D<GridType> GetGrid();
    List<LayoutObject> Flatten();
    void ToLog(Logs log, params String[] customContent);
}

public static class ILayoutObjectExt
{
    public static void CenterOn(this ILayoutObject obj1, ILayoutObject rhs)
    {
        Point center = obj1.Bounding.GetCenter();
        Point centerRhs = obj1.Bounding.GetCenter();
        obj1.Shift(centerRhs.x - center.x, centerRhs.y - center.y);
    }

    public static void ShiftOutside(this ILayoutObject obj, IEnumerable<ILayoutObject> rhs, Point dir)
    {
        ILayoutObject intersect;
        Point hint;
        while (obj.Intersects(rhs, out intersect, out hint))
        {
            obj.ShiftOutside(intersect, dir, hint, true, true);
        }
    }

    public static void ShiftOutside(this ILayoutObject obj, ILayoutObject rhs, Point dir, Point hint, bool rough, bool finalShift)
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
            MultiMap<GridType> tmp = new MultiMap<GridType>();
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
                MultiMap<GridType> tmp = new MultiMap<GridType>();
                tmp.PutAll(rhs.GetGrid());
                tmp.PutAll(obj.GetGrid());
                tmp.ToLog(Logs.LevelGen, "After shifting");
            }
            #endregion
        }
        if (finalShift)
            obj.Shift(dir.x, dir.y);
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printFooter(Logs.LevelGen, "Shift Outside " + obj.ToString());
        }
        #endregion
    }

    public static bool Intersects(this ILayoutObject obj, IEnumerable<ILayoutObject> rhs, out ILayoutObject intersect, out Point at)
    {
        foreach (ILayoutObject l in rhs)
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

    public static bool Intersects(this ILayoutObject obj, ILayoutObject rhs, Point hint, out Point at)
    {
        if (hint != null && rhs.ContainsPoint(hint))
        {
            at = hint;
            return true;
        }
        foreach (Value2D<GridType> val in obj)
        {
            if (rhs.ContainsPoint(val))
            {
                at = val;
                return true;
            }
        }
        at = null;
        return false;
    }

    public static bool ConnectTo(this ILayoutObject obj1, Point pt1, ILayoutObject obj2, Point pt2, out LayoutObject retObj1, out LayoutObject retObj2)
    {
        if (obj1 is LayoutObjectContainer)
        {
            if (!((LayoutObjectContainer)obj1).GetObjAt(pt1, out retObj1))
            {
                retObj2 = null;
                return false;
            }
        }
        else
        {
            retObj1 = (LayoutObject)obj1;
        }

        if (obj2 is LayoutObjectContainer)
        {
            if (!((LayoutObjectContainer)obj2).GetObjAt(pt2, out retObj2))
            {
                return false;
            }
        }
        else
        {
            retObj2 = (LayoutObject)obj2;
        }
        retObj1.Connect(retObj2);
        retObj2.Connect(retObj1);
        return true;
    }
}