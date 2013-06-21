using UnityEngine;
using System.Collections;

public class GridTypeComparator : Comparator<GridType>
{
    static GridTypeComparator singleton;

    public static GridTypeComparator get()
    {
        if (singleton == null)
        {
            singleton = new GridTypeComparator();
        }
        return singleton;
    }

    public int compare(GridType first, GridType second)
    {
        if (((int)first) == ((int)second))
        {
            return 0;
        }
        else if (((int)first) > ((int)second))
        {
            return 1;
        }
        else
        {
            return -1;
        }
    }
}
