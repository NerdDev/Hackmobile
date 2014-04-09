using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class GridLocationResults
{
    bool[] arr = new bool[EnumExt.Length<GridLocation>()];
    public int NumSides { get; set; }
    public int NumCorners { get; set; }

    public bool this[GridLocation loc]
    {
        get
        {
            return arr[(int)loc];
        }
        set
        {
            arr[(int)loc] = value;
        }
    }

    public static GridLocationResults operator !(GridLocationResults results)
    {
        GridLocationResults ret = new GridLocationResults();
        for (int i = 0 ; i < results.arr.Length ; i++)
        {
            ret.arr[i] = !results.arr[i];
        }
        return ret;
    }

    public bool GetCorner(out GridLocation loc)
    {
        if (this[GridLocation.TOPRIGHT])
        {
            loc = GridLocation.TOPRIGHT;
            return true;
        }
        if (this[GridLocation.BOTTOMRIGHT])
        {
            loc = GridLocation.BOTTOMRIGHT;
            return true;
        }
        if (this[GridLocation.BOTTOMLEFT])
        {
            loc = GridLocation.BOTTOMLEFT;
            return true;
        }
        if (this[GridLocation.TOPLEFT])
        {
            loc = GridLocation.TOPLEFT;
            return true;
        }
        loc = GridLocation.CENTER;
        return false;
    }

    public bool Cornered(out GridLocation loc, bool withOpposing = false)
    {
        if (this[GridLocation.LEFT] == this[GridLocation.RIGHT])
        {
            loc = GridLocation.RIGHT;
            return false;
        }
        if (this[GridLocation.BOTTOM] == this[GridLocation.TOP])
        {
            loc = GridLocation.RIGHT;
            return false;
        }
        if (this[GridLocation.LEFT])
        {
            if (this[GridLocation.BOTTOM])
            {
                loc = GridLocation.BOTTOMLEFT;
            }
            else
            {
                loc = GridLocation.TOPLEFT;
            }
        }
        else
        {
            if (this[GridLocation.BOTTOM])
            {
                loc = GridLocation.BOTTOMRIGHT;
            }
            else
            {
                loc = GridLocation.TOPRIGHT;
            }
        }
        if (withOpposing && !this[loc.Opposite()])
        {
            loc = GridLocation.RIGHT;
            return false;
        }
        return true;
    }

    public bool AlternatesSides(out GridDirection passDir)
    {
        if (this[GridLocation.LEFT] != this[GridLocation.RIGHT])
        {
            passDir = GridDirection.HORIZ;
            return false;
        }
        if (this[GridLocation.LEFT] == this[GridLocation.TOP])
        {
            passDir = GridDirection.HORIZ;
            return false;
        }
        if (this[GridLocation.LEFT] == this[GridLocation.BOTTOM])
        {
            passDir = GridDirection.HORIZ;
            return false;
        }
        passDir = this[GridLocation.LEFT] ? GridDirection.HORIZ : GridDirection.VERT;
        return true;
    }

    public bool TShape(out GridLocation loc)
    {
        if (this[GridLocation.LEFT] && this[GridLocation.RIGHT])
        {
            if (this[GridLocation.TOP])
            {
                if (!this[GridLocation.BOTTOM])
                {
                    loc = GridLocation.TOP;
                    return true;
                }
            }
            else if (this[GridLocation.BOTTOM])
            {
                if (!this[GridLocation.TOP])
                {
                    loc = GridLocation.BOTTOM;
                    return true;
                }
            }
        }
        else if (this[GridLocation.TOP] && this[GridLocation.BOTTOM])
        {
            if (this[GridLocation.LEFT])
            {
                if (!this[GridLocation.RIGHT])
                {
                    loc = GridLocation.LEFT;
                    return true;
                }
            }
            else if (this[GridLocation.RIGHT])
            {
                if (!this[GridLocation.LEFT])
                {
                    loc = GridLocation.RIGHT;
                    return true;
                }
            }
        }
        loc = GridLocation.BOTTOM;
        return false;
    }
}

