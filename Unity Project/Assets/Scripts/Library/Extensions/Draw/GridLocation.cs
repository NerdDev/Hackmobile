using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum GridLocation
{
    CENTER,
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
    public static bool IsCorner(this GridLocation loc)
    {
        switch (loc)
        {
            case GridLocation.TOPRIGHT:
            case GridLocation.BOTTOMRIGHT:
            case GridLocation.TOPLEFT:
            case GridLocation.BOTTOMLEFT:
                return true;
            default:
                return false;
        }
    }

    private static GridLocation[] dirs = new[] { GridLocation.TOP, GridLocation.RIGHT, GridLocation.BOTTOM, GridLocation.LEFT };
    public static GridLocation[] Dirs()
    {
        return dirs;
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
                return GridLocation.TOPRIGHT;
            default:
                return GridLocation.CENTER;
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
                return GridLocation.BOTTOM;
            default:
                return GridLocation.CENTER;
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
                return GridLocation.LEFT;
            default:
                return GridLocation.CENTER;
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
                return GridLocation.TOPLEFT;
            default:
                return GridLocation.CENTER;
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
                return GridLocation.BOTTOMRIGHT;
            default:
                return GridLocation.CENTER;
        }
    }

    public static GridLocation Merge(this GridLocation loc, GridLocation rhs)
    {
        switch (loc)
        {
            case GridLocation.TOP:
                switch (rhs)
                {
                    case GridLocation.TOP:
                        return GridLocation.TOP;
                    case GridLocation.LEFT:
                        return GridLocation.TOPLEFT;
                    case GridLocation.RIGHT:
                        return GridLocation.TOPRIGHT;
                }
                break;
            case GridLocation.BOTTOM:
                switch (rhs)
                {
                    case GridLocation.BOTTOM:
                        return GridLocation.BOTTOM;
                    case GridLocation.LEFT:
                        return GridLocation.BOTTOMLEFT;
                    case GridLocation.RIGHT:
                        return GridLocation.BOTTOMRIGHT;
                }
                break;
            case GridLocation.LEFT:
                switch (rhs)
                {
                    case GridLocation.TOP:
                        return GridLocation.TOPLEFT;
                    case GridLocation.BOTTOM:
                        return GridLocation.BOTTOMLEFT;
                    case GridLocation.LEFT:
                        return GridLocation.LEFT;
                }
                break;
            case GridLocation.RIGHT:
                switch (rhs)
                {
                    case GridLocation.TOP:
                        return GridLocation.TOPRIGHT;
                    case GridLocation.BOTTOM:
                        return GridLocation.BOTTOMRIGHT;
                    case GridLocation.RIGHT:
                        return GridLocation.CENTER;
                }
                break;
            case GridLocation.TOPRIGHT:
                switch (rhs)
                {
                    case GridLocation.TOPRIGHT:
                        return GridLocation.TOPRIGHT;
                    case GridLocation.BOTTOMRIGHT:
                        return GridLocation.RIGHT;
                    case GridLocation.TOPLEFT:
                        return GridLocation.TOP;
                }
                break;
            case GridLocation.BOTTOMRIGHT:
                switch (rhs)
                {
                    case GridLocation.TOPRIGHT:
                        return GridLocation.RIGHT;
                    case GridLocation.BOTTOMRIGHT:
                        return GridLocation.BOTTOMRIGHT;
                    case GridLocation.BOTTOMLEFT:
                        return GridLocation.BOTTOM;
                }
                break;
            case GridLocation.TOPLEFT:
                switch (rhs)
                {
                    case GridLocation.TOPRIGHT:
                        return GridLocation.TOP;
                    case GridLocation.TOPLEFT:
                        return GridLocation.TOPLEFT;
                    case GridLocation.BOTTOMLEFT:
                        return GridLocation.LEFT;
                }
                break;
            case GridLocation.BOTTOMLEFT:
                switch (rhs)
                {
                    case GridLocation.BOTTOMRIGHT:
                        return GridLocation.BOTTOM;
                    case GridLocation.TOPLEFT:
                        return GridLocation.LEFT;
                    case GridLocation.BOTTOMLEFT:
                        return GridLocation.BOTTOMLEFT;
                }
                break;
        }
        return GridLocation.CENTER;
    }

    public static void Modify(this GridLocation loc, ref int x, ref int y)
    {
        switch (loc)
        {
            case GridLocation.TOP:
                y++;
                break;
            case GridLocation.BOTTOM:
                y--;
                break;
            case GridLocation.LEFT:
                x--;
                break;
            case GridLocation.RIGHT:
                x++;
                break;
            case GridLocation.TOPRIGHT:
                x++;
                y++;
                break;
            case GridLocation.BOTTOMRIGHT:
                x++;
                y--;
                break;
            case GridLocation.TOPLEFT:
                x--;
                y++;
                break;
            case GridLocation.BOTTOMLEFT:
                x--;
                y--;
                break;
            case GridLocation.CENTER:
            default:
                break;
        }
    }

    public static void Modify(this GridLocation loc, int x, int y, out int xOut, out int yOut)
    {
        loc.Modify(ref x, ref y);
        xOut = x;
        yOut = y;
    }

    public static GridLocation GetGridLocation(this float angle)
    {
        angle = angle % 360f;
        if (angle < 202.5f)
        {
            if (angle < 112.5f)
            {
                if (angle < 67.5)
                {
                    if (angle < 22.5)
                    {
                        return GridLocation.TOP;
                    }
                    return GridLocation.TOPRIGHT;
                }
                return GridLocation.RIGHT;
            }
            else if (angle < 157.5f)
            {
                return GridLocation.BOTTOMRIGHT;
            }
            return GridLocation.BOTTOM;
        }
        else
        {
            if (angle < 292.5f)
            {
                if (angle < 247.5)
                {
                    return GridLocation.BOTTOMLEFT;
                }
                return GridLocation.LEFT;
            }
            else if (angle < 337.5)
            {
                return GridLocation.TOPLEFT;
            }
            return GridLocation.TOP;
        }
    }
}