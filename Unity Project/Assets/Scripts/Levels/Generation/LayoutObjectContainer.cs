using System.Collections;
using System.Collections.Generic;

public abstract class LayoutObjectContainer : IEnumerable<LayoutObject>
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

    public void ToLog(Logs log, params string[] customContent)
    {
        Bake().ToLog(log, customContent);
    }

    public LayoutObject Bake()
    {
        LayoutObject obj = new LayoutObject();
        foreach (LayoutObject rhs in this)
        {
            obj.PutAll(rhs.Grids, rhs.ShiftP);
        }
        return obj;
    }

    public IEnumerator<LayoutObject> GetEnumerator()
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
}
