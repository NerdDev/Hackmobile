using UnityEngine;
using System.Collections;

public class LevelBuilder : MonoBehaviour {

    public GameObject[,] Build(GridArray array, Theme theme)
    {
        GameObject holder = new GameObject("Level Block Holder");
        GameObject[,] goArr = new GameObject[array.getWidth(), array.getHeight()];
        foreach (Value2D<GridType> val in array)
        {
            GameObject protoType = theme.Get(val.val);
            if (protoType != null)
            {
                GameObject obj = Instantiate(protoType) as GameObject;
                GridSpace grid = obj.AddComponent<GridSpace>();
                grid.coords = new Point(val.x, val.y);
                obj.transform.parent = holder.transform;
                obj.transform.Translate(new Vector3(val.x, 0, val.y));
                goArr[val.x, val.y] = obj;
            }
        }
        return goArr;
    }

	public GameObject[,] Build(LevelLayout layout, Theme theme)
	{
        return Build(layout.GetArray(), theme);
	}
}
