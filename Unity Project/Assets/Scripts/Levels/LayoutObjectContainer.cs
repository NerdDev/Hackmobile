using System.Collections;
using System.Collections.Generic;

abstract public class LayoutObjectContainer : LayoutObject, IEnumerable<LayoutObject> {

    public override GridMap getMap()
    {
        return getBakedMap();
    }

    public override GridMap getBakedMap()
    {
        Bounding bounds = getBoundsInternal();
        GridMap ret = new GridMap(bounds.width(), bounds.height());
        foreach(LayoutObject obj in this)
        {
            ret.putAll(obj.getBakedMap(), shiftP);
        }
        return ret;
    }
	
    public abstract IEnumerator<LayoutObject> GetEnumerator();

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }

    protected override Bounding getBoundsInternal()
	{
		Bounding bound = new Bounding();
		foreach (LayoutObject obj in this){
			bound.absorb(obj.getBounds());
		}
		return bound;
	}
}
