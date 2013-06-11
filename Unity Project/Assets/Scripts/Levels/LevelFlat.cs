using UnityEngine;
using System.Collections;

public class LevelFlat : Level {

    MultiMap<GridInstance> grids = new MultiMap<GridInstance>();

    public override GridInstance get(int x, int y)
    {
        GridInstance val;
        if (grids.TryGetValue(x, y, out val))
        {
            return val;
        }
        return null;
    }

    public void put(GridInstance box, int x, int y)
    {
        grids.put(box, x, y);
    }

    public void putAll(MultiMap<GridInstance> rhs)
    {
        grids.putAll(rhs);
    }

    public override MultiMap<GridInstance> getFlat()
    {
        return grids;
    }
}
