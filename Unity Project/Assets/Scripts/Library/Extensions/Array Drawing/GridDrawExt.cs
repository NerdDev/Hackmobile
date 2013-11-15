using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class GridTypeDrawExt
{
    public static bool CanDrawDoor(this GridType[,] arr, int x, int y)
    {
        GridType t = arr[y, x];
        if (t != GridType.Wall)
            return false;
        Surrounding<GridType> surr = new Surrounding<GridType>(arr);
        surr.Focus(x, y);
        return (surr.Alternates(GridTypeEnum.Walkable));
    }

    public static bool DrawDoor(this GridType[,] arr, int x, int y)
    {
        if (CanDrawDoor(arr, x, y))
        {
            arr[y, x] = GridType.Door;
            return true;
        }
        return false;
    }
}
