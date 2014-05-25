using System;
using UnityEngine;
using System.Collections.Generic;

public class ItemChest : MonoBehaviour
{

    internal GridSpace Location
    {
        get
        {
            Vector2 currentLoc = new Vector2(gameObject.transform.position.x.Round(), gameObject.transform.position.z.Round());
            return BigBoss.Levels.Level[currentLoc.x.ToInt(), currentLoc.y.ToInt()];
        }
    }

    void Start()
    {
        Location._chest = this;
		LeveledItemList itemList = BigBoss.Objects.LeveledItems.GetPrototype ("potions");
		ItemHolder ih = itemList.Get (new System.Random (), 1, BigBoss.Player.Level)[0];
		Item i = ih.Get ();
		//Location.inventory.Add (i);
    }

    public void init()
    {
        this.transform.localPosition = new Vector3(Location.X + .3f, 0f, Location.Y + .3f);
        this.transform.Rotate(Vector3.up, 45f);
    }

    void OnMouseDown()
    {
        if (CheckDistance())
        {
            BigBoss.Gooey.ground.Open(this);
        }
    }

    internal bool CheckDistance()
    {
        if (Location.Equals(BigBoss.Player.GridSpace))
        {
            return true;
        }
        Value2D<GridSpace> space;
        GridSpace playerSpace = BigBoss.Player.GridSpace;
        return BigBoss.Levels.Level.Array.GetPointAround(playerSpace.X, playerSpace.Y, true, (arr, x, y) =>
        {
            return Location.X == x && Location.Y == y;
        }, out space);
    }
}
