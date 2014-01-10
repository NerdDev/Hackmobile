using System.Collections;
using System.Collections.Generic;

public abstract class LayoutObjectContainer : LayoutObject, IEnumerable<LayoutObject>
{
    protected List<LayoutObject> Objects = new List<LayoutObject>();

    public virtual void AddObject(LayoutObject obj)
    {
        Objects.Add(obj);
    }

    public void RemoveObject(LayoutObject obj)
    {
        Objects.Remove(obj);
    }

    public void ShiftAll(int x, int y)
    {
        foreach (LayoutObject obj in Objects)
        {
            obj.Shift(x, y);
        }
    }

    public void ShiftAll(Point shift)
    {
        ShiftAll(shift.x, shift.y);
    }

    protected Container2D<GridType> _baked;
    public override Container2D<GridType> Grids
    {
        get
        {
            if (_baked != null) return _baked;
            MultiMap<GridType> map = new MultiMap<GridType>();
            foreach (LayoutObject obj in this)
            {
                map.PutAll(obj.Grids, obj.ShiftP);
            }
            return map;
        }
        protected set
        {
        }
    }

    public override void Bake()
    {
        _baked = Grids;
    }

    public IEnumerator<LayoutObject> GetEnumerator()
    {
        return Objects.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    public override bool ContainsPoint(Point pt)
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
            pt.Unshift(obj.ShiftP);
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
}
