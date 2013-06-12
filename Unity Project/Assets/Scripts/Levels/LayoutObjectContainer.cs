using UnityEngine;
using System.Collections;
using System.Collections.Generic;

abstract public class LayoutObjectContainer : LayoutObject, IEnumerable<LayoutObject> {

    public override MultiMap<GridType> getMap()
    {
        return getBakedMap();
    }

    public override MultiMap<GridType> getBakedMap()
    {
        MultiMap<GridType> ret = new MultiMap<GridType>();
        foreach(LayoutObject obj in this)
        {
            ret.putAll(obj.getBakedMap());
        }
        return ret;
    }
	
    public abstract IEnumerator<LayoutObject> GetEnumerator();

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
	
	public override Bounding getBoundsInternal()
	{
		Bounding bound = new Bounding();
		foreach (LayoutObject obj in this){
			bound.absorb(obj.getBounds());
		}
		return bound;
	}
}
