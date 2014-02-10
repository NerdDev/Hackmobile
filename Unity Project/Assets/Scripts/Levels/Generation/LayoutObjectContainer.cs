using System.Collections;
using System.Collections.Generic;

public class LayoutObjectContainer : ILayoutObject
{
    public List<ILayoutObject> Objects = new List<ILayoutObject>();

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

    public void Shift(int x, int y)
    {
        foreach (ILayoutObject obj in Objects)
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

    public IEnumerator<Value2D<GridType>> GetEnumerator()
    {
        foreach (ILayoutObject obj in Objects)
        {
            foreach (Value2D<GridType> val in obj)
            {
                yield return val;
            }
        }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    public bool ContainsPoint(Point pt)
    {
        LayoutObject obj;
        return GetObjAt(pt, out obj);
    }

    public bool FindAndConnect(LayoutObject obj1, Point connectPt)
    {
        Point pt = new Value2D<GridType>(connectPt.x + obj1.ShiftP.x, connectPt.y + obj1.ShiftP.y);
        LayoutObject obj;
        if (GetObjAt(pt, out obj))
        {
            obj1.Connect(obj);
            return true;
        }
        return false;
    }

    public bool GetObjAt(Point pt, out LayoutObject layoutObj)
    {
        foreach (ILayoutObject obj in Objects)
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
                if (obj is LayoutObject)
                {
                    layoutObj = (LayoutObject)obj;
                    return true;
                }
                else
                {
                    return ((LayoutObjectContainer)obj).GetObjAt(pt, out layoutObj);
                }
            }
        }
        layoutObj = null;
        return false;
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

    public void ConnectTo(ILayoutObject obj, Point at)
    {
        throw new System.NotImplementedException();
    }

    public List<LayoutObject> Flatten()
    {
        List<LayoutObject> ret = new List<LayoutObject>(Objects.Count);
        foreach (ILayoutObject obj in Objects)
        {
            ret.AddRange(obj.Flatten());
        }
        return ret;
    }
}
