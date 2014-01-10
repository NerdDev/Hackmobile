using System;
using UnityEngine;
using System.Collections.Generic;

class ItemChest : MonoBehaviour
{
    public GridSpace Location;
    public List<Item> items = new List<Item>();

    public void init()
    {
        Vector3 blockPos = Location.Block.transform.position;
        this.transform.localPosition = new Vector3(blockPos.x, blockPos.y + .5f, blockPos.z);
    }

    void OnMouseDown()
    {
        if (CheckDistance())
        {
            BigBoss.Gooey.GenerateGroundItems(this);
        }
    }

    internal bool CheckDistance()
    {
        PathTree path = new PathTree(Location, BigBoss.Player.GridSpace);
        List<PathNode> nodes = path.getPath();
        if (nodes.Count == 2)
        {
            return true;
        }
        return false;
    }

    public bool Remove(Item item)
    {
        return Location.RemoveFromChest(item);
    }
}
