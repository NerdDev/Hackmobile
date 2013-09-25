using System.Collections;
using System.Collections.Generic;

abstract public class LayoutObjectContainer : LayoutObject, IEnumerable<LayoutObject> {

    protected List<LayoutObject> Objects = new List<LayoutObject>(); 

    public virtual void AddObject(LayoutObject obj, int buffer)
    {
        Objects.Add(obj);
        /// Shift so nothing is in the negative 
        Bounding bounds = obj.GetBounding();
        Point shift = bounds.GetShiftNonNeg(buffer);
        if (!shift.isZero())
        {
            #region DEBUG
            if (BigBoss.Debug.logging(DebugManager.Logs.LevelGen))
            {
                BigBoss.Debug.w(DebugManager.Logs.LevelGen, "Shifted elements of " + this + " " + shift);
            }
            #endregion
            ShiftAll(shift);
        }
    }

    public virtual void AddObject(LayoutObject obj)
    {
        AddObject(obj, LevelGenerator.layoutMargin);
    }

    public void RemoveObject(LayoutObject obj)
    {
        Objects.Remove(obj);
    }

    public void ShiftAll(int x, int y)
    {
        foreach (LayoutObject obj in Objects)
        {
            obj.shift(x, y);
        }   
    }

    public void ShiftAll(Point shift)
    {
        ShiftAll(shift.x, shift.y);
    }

    public GridArray GetArray(Bounding bound, bool minimize)
    {
        adjustBounding(bound, false);
        GridArray ret = new GridArray(bound, minimize);
        foreach (LayoutObject obj in this)
        {
            if (minimize)
                ret.PutAll(obj, bound);
            else
                ret.PutAll(obj);
        }
        return ret;
    }

    public override GridArray GetArray()
    {
        GridArray ret = new GridArray(GetBounding(), false);
        foreach (LayoutObject obj in this)
        {
            ret.PutAll(obj);
        }
        return ret;
    }

    public override GridArray GetPrintArray()
    {
        GridArray ret = new GridArray(GetBounding(), false);
        foreach (LayoutObject obj in this)
        {
            ret.PutAll(obj.GetPrintArray(), obj.GetShift());
        }
        return ret;
    }

    public GridArray GetArray(int buffer)
    {
        var bounds = GetBounding();
        bounds.expand(buffer);
        return GetArray(bounds, false);
    }
	
    public IEnumerator<LayoutObject> GetEnumerator()
    {
        return Objects.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    protected override Bounding GetBoundingUnshifted()
	{
		Bounding bound = new Bounding();
		foreach (LayoutObject obj in this){
			Bounding objBound = obj.GetBounding();
			if (objBound.IsValid())
			{
				bound.absorb(objBound);
			}
		}
		return bound;
	}


    public override bool ContainsPoint(Value2D<GridType> val)
    {
        return GetObjAt(val) != null;
    }

    public void FindAndConnect(LayoutObject obj1, Value2D<GridType> connectPt)
    {
        obj1.Connect(GetObjAt(obj1.ShiftValue(connectPt)));
    }

    public LayoutObject GetObjAt(Value2D<GridType> val)
    {
        #region DEBUG
        if (BigBoss.Debug.logging(DebugManager.Logs.LevelGen))
        {
            BigBoss.Debug.printHeader(DebugManager.Logs.LevelGen, "Get Object At");
        }
        #endregion
        #region DEBUG
        if (BigBoss.Debug.logging(DebugManager.Logs.LevelGen))
        {
            BigBoss.Debug.w(DebugManager.Logs.LevelGen, "Getting object at val " + val);
        }
        #endregion
        foreach (LayoutObject obj in this)
        {
            if (obj.ContainsPoint(val.Unshift(obj.GetShift())))
            {
                #region DEBUG
                if (BigBoss.Debug.logging(DebugManager.Logs.LevelGen))
                {
                    BigBoss.Debug.w(DebugManager.Logs.LevelGen, "Returning " + obj);
                    BigBoss.Debug.printFooter(DebugManager.Logs.LevelGen);
                }
                #endregion
                return obj;
            }
        }
        #region DEBUG
        if (BigBoss.Debug.logging(DebugManager.Logs.LevelGen))
        {
            BigBoss.Debug.printFooter(DebugManager.Logs.LevelGen);
        }
        #endregion
        return null;
    }
}