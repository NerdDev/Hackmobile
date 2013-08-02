using UnityEngine;
using System.Collections;

public class FilterNextToNull : PassFilter<Value2D<GridType>>
{
    private Surrounding<GridType> surround;

    public FilterNextToNull(GridArray grids)
    {
        this.surround = new Surrounding<GridType>(grids);
    }

    public bool pass(Value2D<GridType> dir)
    {
        surround.Load(dir);
        Value2D<GridType> nullDir = surround.GetDirWithVal(true, GridType.NULL);
        return nullDir != null;
    }
}
