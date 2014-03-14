using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class GridLocationResults
{
    bool[] arr = new bool[EnumExt.Length<GridLocation>()];

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

    //public bool this[GridDirection dir]
    //{
    //    get
    //    {
    //        switch (dir)
    //        {
    //            case GridDirection.HORIZ:
    //                return this[GridLocation.LEFT] && this[GridLocation.RIGHT];
    //            case GridDirection.VERT:
    //                break;
    //                return this[GridLocation.LEFT] && this[GridLocation.RIGHT];
    //            case GridDirection.DIAGTLBR:
    //                break;
    //            case GridDirection.DIAGBLTR:
    //                break;
    //            default:
    //                break;
    //        }
    //    }
    //}
}

