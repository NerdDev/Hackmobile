using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GridLocation {
    TOP,
    BOTTOM,
    LEFT,
    RIGHT,
    TOPRIGHT,
    BOTTOMRIGHT,
    TOPLEFT,
    BOTTOMLEFT
}

public static class GridLocationExt
{
    public static IEnumerable<GridLocation> Dirs()
    {
        yield return GridLocation.TOP;
        yield return GridLocation.RIGHT;
        yield return GridLocation.BOTTOM;
        yield return GridLocation.LEFT;
    }
}