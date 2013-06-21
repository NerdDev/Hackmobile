using UnityEngine;
using System.Collections;

public class LevelBuilder : MonoBehaviour {
	
	public GameObject[,] Build(LevelLayout layout, Theme theme)
	{
		GridArray array = layout.GetArray();
		GameObject[,] goArr = new GameObject[array.getWidth(),array.getHeight()];
		foreach (Value2D<GridType> val in array)
        {
            GameObject protoType = null;
            GameObject obj = null;
            switch (val.val)
            {
                case GridType.Floor:
                    protoType = theme.floor;
                    break;
                default:
                    break;
            }
            if (protoType != null)
            {
                obj = Instantiate(protoType) as GameObject;
                obj.transform.position = new Vector3(val.x, 0, val.y);
            }
            goArr[val.x, val.y] = obj;
		}
		return goArr;
	}
}
