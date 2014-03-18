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

    public static GridLocation Opposite(this GridLocation loc)
    {
        switch (loc)
        {
            case GridLocation.TOP:
                return GridLocation.BOTTOM;
            case GridLocation.BOTTOM:
                return GridLocation.TOP;
            case GridLocation.LEFT:
                return GridLocation.RIGHT;
            case GridLocation.RIGHT:
                return GridLocation.LEFT;
            case GridLocation.TOPRIGHT:
                return GridLocation.BOTTOMLEFT;
            case GridLocation.BOTTOMRIGHT:
                return GridLocation.TOPLEFT;
            case GridLocation.TOPLEFT:
                return GridLocation.BOTTOMRIGHT;
            case GridLocation.BOTTOMLEFT:
            default:
                return GridLocation.TOPRIGHT;
        }
    }

    public static GridLocation CounterClockwise(this GridLocation loc)
    {
        switch (loc)
        {
            case GridLocation.TOP:
                return GridLocation.TOPLEFT;
            case GridLocation.BOTTOM:
                return GridLocation.BOTTOMRIGHT;
            case GridLocation.LEFT:
                return GridLocation.BOTTOMLEFT;
            case GridLocation.RIGHT:
                return GridLocation.TOPRIGHT;
            case GridLocation.TOPRIGHT:
                return GridLocation.TOP;
            case GridLocation.BOTTOMRIGHT:
                return GridLocation.RIGHT;
            case GridLocation.TOPLEFT:
                return GridLocation.LEFT;
            case GridLocation.BOTTOMLEFT:
            default:
                return GridLocation.BOTTOM;
        }
    }

    public static GridLocation Clockwise(this GridLocation loc)
    {
        switch (loc)
        {
            case GridLocation.TOP:
                return GridLocation.TOPRIGHT;
            case GridLocation.BOTTOM:
                return GridLocation.BOTTOMLEFT;
            case GridLocation.LEFT:
                return GridLocation.TOPLEFT;
            case GridLocation.RIGHT:
                return GridLocation.BOTTOMRIGHT;
            case GridLocation.TOPRIGHT:
                return GridLocation.RIGHT;
            case GridLocation.BOTTOMRIGHT:
                return GridLocation.BOTTOM;
            case GridLocation.TOPLEFT:
                return GridLocation.TOP;
            case GridLocation.BOTTOMLEFT:
            default:
                return GridLocation.LEFT;
        }
    }
    
    public static GridLocation Clockwise90(this GridLocation loc)
    {
        switch (loc)
        {
            case GridLocation.TOP:
                return GridLocation.RIGHT;
            case GridLocation.BOTTOM:
                return GridLocation.LEFT;
            case GridLocation.LEFT:
                return GridLocation.TOP;
            case GridLocation.RIGHT:
                return GridLocation.BOTTOM;
            case GridLocation.TOPRIGHT:
                return GridLocation.BOTTOMRIGHT;
            case GridLocation.BOTTOMRIGHT:
                return GridLocation.BOTTOMLEFT;
            case GridLocation.TOPLEFT:
                return GridLocation.TOPRIGHT;
            case GridLocation.BOTTOMLEFT:
            default:
                return GridLocation.TOPLEFT;
        }
    }

    public static GridLocation CounterClockwise90(this GridLocation loc)
    {
        switch (loc)
        {
            case GridLocation.TOP:
                return GridLocation.LEFT;
            case GridLocation.BOTTOM:
                return GridLocation.RIGHT;
            case GridLocation.LEFT:
                return GridLocation.BOTTOM;
            case GridLocation.RIGHT:
                return GridLocation.TOP;
            case GridLocation.TOPRIGHT:
                return GridLocation.TOPLEFT;
            case GridLocation.BOTTOMRIGHT:
                return GridLocation.TOPRIGHT;
            case GridLocation.TOPLEFT:
                return GridLocation.BOTTOMLEFT;
            case GridLocation.BOTTOMLEFT:
            default:
                return GridLocation.BOTTOMRIGHT;
        }
    }
}