using UnityEngine;
using System.Collections;

public class LevelBuilder : MonoBehaviour {
	
	public GameObject[,] Build(LevelLayout layout, Theme theme)
	{
		GridArray array = layout.GetArray();
		GameObject[,] goArr = new GameObject[array.getWidth(),array.getHeight()];
		foreach (Value2D<GridType> val in array)
        {
            GameObject protoType = theme.Get(val.val);
            if (protoType != null)
            {
                GameObject obj = Instantiate(protoType) as GameObject;
                obj.transform.Translate(new Vector3(val.x, 0, val.y));
                goArr[val.x, val.y] = obj;
            }
		}
		return goArr;
	}
}
