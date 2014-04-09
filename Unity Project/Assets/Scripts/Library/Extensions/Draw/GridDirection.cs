using UnityEngine;
using System.Collections;

public enum GridDirection
{
    HORIZ,
    VERT,
    DIAGTLBR,
    DIAGBLTR
}

public static class GridDirectionExt
{
    public static bool PartOf(this GridLocation loc, GridDirection dir)
    {
        switch (loc)
        {
            case GridLocation.TOP:
                return (dir == GridDirection.VERT);
            case GridLocation.BOTTOM:
                return (dir == GridDirection.VERT);
            case GridLocation.LEFT:
                return (dir == GridDirection.HORIZ);
            case GridLocation.RIGHT:
                return (dir == GridDirection.HORIZ);
        }
        return false;
    }

    public static GridDirection Clockwise(this GridDirection dir)
    {
        switch (dir)
        {
            case GridDirection.HORIZ:
                return GridDirection.DIAGTLBR;
            case GridDirection.VERT:
                return GridDirection.DIAGBLTR;
            case GridDirection.DIAGTLBR:
                return GridDirection.VERT;
            case GridDirection.DIAGBLTR:
            default:
                return GridDirection.HORIZ;
        }
    }

    public static GridDirection CounterClockwise(this GridDirection dir)
    {
        switch (dir)
        {
            case GridDirection.HORIZ:
                return GridDirection.DIAGBLTR;
            case GridDirection.VERT:
                return GridDirection.DIAGTLBR;
            case GridDirection.DIAGTLBR:
                return GridDirection.HORIZ;
            case GridDirection.DIAGBLTR:
            default:
                return GridDirection.VERT;
        }
    }

    public static GridDirection Rotate90(this GridDirection dir)
    {
        switch (dir)
        {
            case GridDirection.HORIZ:
                return GridDirection.VERT;
            case GridDirection.VERT:
                return GridDirection.HORIZ;
            case GridDirection.DIAGTLBR:
                return GridDirection.DIAGBLTR;
            case GridDirection.DIAGBLTR:
            default:
                return GridDirection.DIAGTLBR;
        }
    }
}
