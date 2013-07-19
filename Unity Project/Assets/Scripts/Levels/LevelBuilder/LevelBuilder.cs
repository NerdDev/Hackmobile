using UnityEngine;
using System.Collections;

public class LevelBuilder : MonoBehaviour {

    public void Build(GridSpace[,] array, Theme theme)
    {
        GameObject holder = new GameObject("Level Block Holder");
        for (int y = 0; y < array.GetLength(0); y++)
        {
            for (int x = 0; x < array.GetLength(0); x++)
            {
                GridSpace space = array[y, x];
                if (space == null) continue;
                GameObject protoType = theme.Get(array[y,x].Type);
                if (protoType == null) continue;
                space.Block = Instantiate(protoType) as GameObject;
                space.Block.transform.parent = holder.transform;
                space.Block.transform.Translate(new Vector3(x, 0, y));
            }
        }
    }
}
