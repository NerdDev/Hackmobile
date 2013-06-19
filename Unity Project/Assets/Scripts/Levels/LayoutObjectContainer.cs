using System.Collections;
using System.Collections.Generic;

abstract public class LayoutObjectContainer : LayoutObject, IEnumerable<LayoutObject> {

    public override GridArray GetArray()
    {
        GridArray ret = new GridArray(GetBounding());
        foreach(LayoutObject obj in this)
        {
            ret.PutAll(obj);
        }
        return ret;
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
			bound.absorb(obj.GetBounding());
		}
		return bound;
	}

}
