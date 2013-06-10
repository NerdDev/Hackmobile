using UnityEngine;
using System.Collections;

public class LevelFlat : Level {

    MultiMap<GridBox> grids = new MultiMap<GridBox>();

    public override GridBox get(int x, int y)
    {
        GridBox val;
        if (grids.TryGetValue(x, y, out val))
        {
            return val;
        }
        return null;
    }

    public void put(GridBox box, int x, int y)
    {
        grids.put(box, x, y);
    }

    public void putAll(MultiMap<GridBox> rhs)
    {
        grids.putAll(rhs);
    }

    public override MultiMap<GridBox> getFlat()
    {
        return grids;
    }
}
