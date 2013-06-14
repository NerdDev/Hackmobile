using System.Collections;
using System.Collections.Generic;

abstract public class LayoutObjectContainer : LayoutObject, IEnumerable<LayoutObject> {

    public override GridMap getMap()
    {
        return getBakedMap();
    }

    public override GridMap getBakedMap()
    {
        GridMap ret = new GridMap(getBounds());
        foreach(LayoutObject obj in this)
        {
			GridMap bakedObj = obj.getBakedMap();
            ret.putAll(bakedObj, shiftP);
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
