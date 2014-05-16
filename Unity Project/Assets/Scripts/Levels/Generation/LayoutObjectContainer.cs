using System.Collections;
using System.Collections.Generic;

public class LayoutObjectContainer<T> : ILayoutObject<T>
    where T : IGridType
{
    public List<ILayoutObject<T>> Objects = new List<ILayoutObject<T>>();

    public Bounding Bounding
    {
        get
        {
            Bounding bounds = new Bounding();
            foreach (ILayoutObject<T> obj in Objects)
            {
                bounds.Absorb(obj.Bounding);
            }
            return bounds;
        }
    }

    public void Shift(int x, int y)
    {
        foreach (ILayoutObject<T> obj in Objects)
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

    public IEnumerator<Value2D<T>> GetEnumerator()
    {
        foreach (ILayoutObject<T> obj in Objects)
        {
            foreach (var val in obj)
            {
                yield return val;
            }
        }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    public bool Contains(int x, int y)
    {
        LayoutObject<T> obj;
        return GetObjAt(x, y, out obj);
    }

    public bool FindAndConnect(LayoutObject<T> obj1, Point connectPt)
    {
        Point pt = new Value2D<GridType>(connectPt.x + obj1.ShiftP.x, connectPt.y + obj1.ShiftP.y);
        LayoutObject<T> obj;
        if (GetObjAt(pt.x, pt.y, out obj))
        {
            obj1.Connect(obj);
            return true;
        }
        return false;
    }

    public bool GetObjAt(int x, int y, out LayoutObject<T> layoutObj)
    {
        foreach (ILayoutObject<T> obj in Objects)
        {
            if (obj.Contains(x, y))
            {
                if (obj is LayoutObject<T>)
                {
                    layoutObj = (LayoutObject<T>)obj;
                    return true;
                }
                else
                {
                    return ((LayoutObjectContainer<T>)obj).GetObjAt(x, y, out layoutObj);
                }
            }
        }
        layoutObj = null;
        return false;
    }

    public Container2D<T> GetGrid()
    {
        var map = new MultiMap<T>();
        foreach (ILayoutObject<T> obj in Objects)
        {
            map.PutAll(obj.GetGrid());
        }
        return map;
    }

    public void ConnectTo(ILayoutObject<T> obj, Point at)
    {
        throw new System.NotImplementedException();
    }

    public List<LayoutObject<T>> Flatten()
    {
        List<LayoutObject<T>> ret = new List<LayoutObject<T>>(Objects.Count);
        foreach (ILayoutObject<T> obj in Objects)
        {
            ret.AddRange(obj.Flatten());
        }
        return ret;
    }
}
