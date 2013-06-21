using System.Collections;
using System.Collections.Generic;

abstract public class LayoutObjectContainer : LayoutObject, IEnumerable<LayoutObject> {

    public override GridArray GetArray()
    {
		Bounding bound = GetBoundingInternal();
        GridArray ret = new GridArray(bound, true);
        foreach(LayoutObject obj in this)
        {
            ret.PutAll(obj, bound);
        }
        return ret;
    }
	
	public override GridType[,] GetMinimizedArray()
    {
        return GetArray().GetArr();
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
