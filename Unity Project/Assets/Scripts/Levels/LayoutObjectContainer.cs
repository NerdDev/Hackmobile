using System.Collections;
using System.Collections.Generic;

abstract public class LayoutObjectContainer : LayoutObject, IEnumerable<LayoutObject> {

    protected List<LayoutObject> Objects = new List<LayoutObject>(); 

    public void AddObject(LayoutObject obj)
    {
        Objects.Add(obj);
        /// Shift so nothing is in the negative 
        Point shift = obj.GetBounding().GetShiftNonNeg();
        if (!shift.isZero())
        {
            #region DEBUG
            if (DebugManager.logging(DebugManager.Logs.LevelGen))
            {
                DebugManager.w(DebugManager.Logs.LevelGen, "Shifted elements of " + this + " " + shift);
            }
            #endregion
            ShiftAll(shift);
        }
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

    public GridArray GetArray(Bounding bound)
    {
        adjustBounding(bound, false);
        GridArray ret = new GridArray(bound, true);
        foreach (LayoutObject obj in this)
        {
            ret.PutAll(obj, bound);
        }
        return ret;
    }
    
    public override GridArray GetArray()
    {
        return GetArray(GetBounding());
    }
	
	public override GridType[,] GetMinimizedArray(GridArray inArr)
    {
        return inArr.GetArr();
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
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printHeader(DebugManager.Logs.LevelGen, "Get Object At");
        }
        #endregion
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.w(DebugManager.Logs.LevelGen, "Getting object at val " + val);
        }
        #endregion
        foreach (LayoutObject obj in this)
        {
            if (obj.ContainsPoint(val.Unshift(obj.GetShift())))
            {
                #region DEBUG
                if (DebugManager.logging(DebugManager.Logs.LevelGen))
                {
                    DebugManager.w(DebugManager.Logs.LevelGen, "Returning " + obj);
                    DebugManager.printFooter(DebugManager.Logs.LevelGen);
                }
                #endregion
                return obj;
            }
        }
        #region DEBUG
        if (DebugManager.logging(DebugManager.Logs.LevelGen))
        {
            DebugManager.printFooter(DebugManager.Logs.LevelGen);
        }
        #endregion
        return null;
    }
}
