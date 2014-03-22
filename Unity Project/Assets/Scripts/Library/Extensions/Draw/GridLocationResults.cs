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

    public static GridLocationResults operator !(GridLocationResults results)
    {
        GridLocationResults ret = new GridLocationResults();
        for (int i = 0 ; i < results.arr.Length ; i++)
        {
            ret.arr[i] = !results.arr[i];
        }
        return ret;
    }
}

