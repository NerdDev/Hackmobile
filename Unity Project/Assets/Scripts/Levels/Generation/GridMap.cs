using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GridMap : MultiMap<GridType> {

    public override List<string> ToRowStrings()
    {
        GridType[,] array = GetArr();
        List<string> ret = new List<string>();
        for (int y = array.GetLength(0) - 1; y >= 0; y -= 1)
        {
            string rowStr = "";
            for (int x = 0; x < array.GetLength(1); x += 1)
            {
                rowStr += LayoutObject.getAscii(array[y, x]);
            }
            ret.Add(rowStr);
        }
        return ret;
    }
}
