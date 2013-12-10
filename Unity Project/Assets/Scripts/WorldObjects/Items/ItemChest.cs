using System;
using UnityEngine;
using System.Collections.Generic;

class ItemChest : MonoBehaviour
{
    public GridSpace Location { get; set; }
    public List<Item> items = new List<Item>();

    public void init()
    {
        Vector3 blockPos = Location.Block.transform.position;
        this.transform.localPosition = new Vector3(blockPos.x, blockPos.y + .5f, blockPos.z);
    }

    void OnMouseDown()
    {
        PathTree path = new PathTree(Location, BigBoss.Player.Location);
        List<PathNode> nodes = path.getPath();
        if (nodes.Count == 2)
        {
            BigBoss.Gooey.GenerateGroundItems(this);
        }
    }
}
