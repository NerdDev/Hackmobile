using System.Collections;
using System.Collections.Generic;

abstract public class LayoutObjectContainer : LayoutObject, IEnumerable<LayoutObject> {

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
	
    public abstract IEnumerator<LayoutObject> GetEnumerator();

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    protected override Bounding GetBoundingInternal()
	{
		Bounding bound = new Bounding();
		foreach (LayoutObject obj in this){
			Bounding objBound = obj.GetBounding();
			if (objBound.isValid())
			{
				bound.absorb(objBound);
			}
		}
		return bound;
	}

}
