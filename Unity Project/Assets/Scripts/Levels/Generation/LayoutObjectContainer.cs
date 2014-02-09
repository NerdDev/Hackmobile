using System.Collections;
using System.Collections.Generic;

public class LayoutObjectContainer : IEnumerable<ILayoutObject>, ILayoutObject
{
    protected List<ILayoutObject> Objects = new List<ILayoutObject>();

    public Bounding Bounding
    {
        get
        {
            Bounding bounds = new Bounding();
            foreach (ILayoutObject obj in Objects)
            {
                bounds.Absorb(obj.Bounding);
            }
            return bounds;
        }
    }

    public virtual void AddObject(ILayoutObject obj)
    {
        Objects.Add(obj);
    }

    public void Shift(int x, int y)
    {
        foreach (LayoutObject obj in Objects)
        {
            obj.Shift(x, y);
        }
    }

    public void Shift(Point shift)
    {
        Shift(shift.x, shift.y);
    }

    public void ToLog(Logs log, params string[] customContent)
    {
        GetGrid().ToLog(log, customContent);
    }

    public IEnumerator<ILayoutObject> GetEnumerator()
    {
        return Objects.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    public bool ContainsPoint(Point pt)
    {
        return GetObjAt(pt) != null;
    }

    public void FindAndConnect(LayoutObject obj1, Point connectPt)
    {
        Point pt = new Value2D<GridType>(connectPt.x + obj1.ShiftP.x, connectPt.y + obj1.ShiftP.y);
        obj1.Connect(GetObjAt(pt));
    }

    public LayoutObject GetObjAt(Point pt)
    {
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(Logs.LevelGen, "Get Object At");
        }
        #endregion
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.w(Logs.LevelGen, "Getting object at val " + pt);
        }
        #endregion
        foreach (LayoutObject obj in this)
        {
            if (obj.ContainsPoint(pt))
            {
                #region DEBUG
                if (BigBoss.Debug.logging(Logs.LevelGen))
                {
                    BigBoss.Debug.w(Logs.LevelGen, "Returning " + obj);
                    BigBoss.Debug.printFooter(Logs.LevelGen, "Get Object At");
                }
                #endregion
                return obj;
            }
        }
        #region DEBUG
        if (BigBoss.Debug.logging(Logs.LevelGen))
        {
            BigBoss.Debug.printFooter(Logs.LevelGen, "Get Object At");
        }
        #endregion
        return null;
    }

    public Container2D<GridType> GetGrid()
    {
        MultiMap<GridType> map = new MultiMap<GridType>();
        foreach (ILayoutObject obj in Objects)
        {
            map.PutAll(obj.GetGrid());
        }
        return map;
    }
}
