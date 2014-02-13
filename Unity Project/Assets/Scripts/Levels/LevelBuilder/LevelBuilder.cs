using UnityEngine;
using System.Collections;

public class LevelBuilder : MonoBehaviour
{
    private GameObject holder;

    public Theme Theme { get; set; }

    private int count = 0;
    private int otherCount = 0;
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
        GameObject obj = Instantiate(protoType, new Vector3(x, protoType.transform.position.y, y), Quaternion.identity) as GameObject;
        obj.transform.parent = holder.transform;
        space.Block = obj;
        count++;
        otherCount++;
        if (otherCount > 300)
        {
            System.GC.Collect();
            otherCount = 0;
        }
        if (count > 20)
        {
            count = 0;
            Combine();
        }
        return obj;
    }

    public void Combine()
    {
        StaticBatchingUtility.Combine(holder);
    }
}
