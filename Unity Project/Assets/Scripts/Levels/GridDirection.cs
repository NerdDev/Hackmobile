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
            case GridLocation.UP:
                return (dir == GridDirection.VERT);
            case GridLocation.DOWN:
                return (dir == GridDirection.VERT);
            case GridLocation.LEFT:
                return (dir == GridDirection.HORIZ);
            case GridLocation.RIGHT:
                return (dir == GridDirection.HORIZ);
        }
        return false;
    }
}
