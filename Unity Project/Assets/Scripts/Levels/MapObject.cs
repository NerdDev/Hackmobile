using UnityEngine;
using System.Collections;
using System.Collections.Generic;

abstract public class MapObject {

    abstract public GridInstance get(int x, int y);

    abstract public MultiMap<GridInstance> getFlat();

    protected string toString()
    {
        MultiMap<GridInstance> grids = getFlat();
        string ret = "";
        foreach (SortedDictionary<int, GridInstance> row in grids.getRows())
        {
            foreach (GridInstance box in row.Values)
            {
                ret += box.ascii;
            }
            ret += "\n";
        }
        return ret;
    }

    public void toLog(DebugManager.Logs log)
    {
        MultiMap<GridInstance> grids = getFlat();
        foreach (SortedDictionary<int, GridInstance> row in grids.getRows())
        {
            string rowStr = "";
            foreach (GridInstance box in row.Values)
            {
                rowStr += box.ascii;
            }
            DebugManager.w(log, rowStr);
        }
        DebugManager.w(log, "");
    }

}
