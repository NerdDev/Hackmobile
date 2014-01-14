using UnityEngine;
using System.Collections;

public class LevelBuilder : MonoBehaviour
{
    private GameObject holder;

    public Theme Theme { get; set; }

    public GameObject Build(Value2D<GridSpace> val)
    {
        return Build(val.val, val.x, val.y);
    }

    public GameObject Build(GridSpace space, int x, int y)
    {
        if (holder == null)
        {
            holder = new GameObject("Level Block Holder");   
        }
        if (space == null) return null;
        GameObject protoType = Theme.Get(space.Type);
        if (protoType == null) return null;
        GameObject obj = Instantiate(protoType) as GameObject;
        obj.transform.parent = holder.transform;
        obj.transform.Translate(new Vector3(x, 0, y));
        space.Block = obj;
        return obj;
    }
}
