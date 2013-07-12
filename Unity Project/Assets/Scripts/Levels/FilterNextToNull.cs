using UnityEngine;
using System.Collections;

public class FilterNextToNull : PassFilter<Value2D<GridType>> {

    readonly GridArray grids;

    public FilterNextToNull(GridArray grids)
    {
        this.grids = grids;
    }

    public bool pass(Value2D<GridType> dir)
    {
        Surrounding<GridType> dirSurround = Surrounding<GridType>.Get(grids, dir);
        Value2D<GridType> nullDir = dirSurround.GetDirWithVal(GridType.NULL);
        return nullDir != null;
    }
}
