using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class InventoryCategory : SortedDictionary<string, Item>
{
    public string id;

    public InventoryCategory(string id)
    {
        this.id = id;
    }

    public void Add(Item i)
    {
        Item item;
        if (!this.TryGetValue(i.Name, out item))
        {
            this.Add(i.Name, i);
        }
        else
        {
            item.Add();
            if (i != item)
            {
                i.Destroy();
            }
        }
        this[i.Name].OnGround = false;
    }

    public bool Remove(Item i)
    {
        Item item;
        if (TryGetValue(i.Name, out item))
            item.RemoveItem();
        return false;
    }

    public bool Contains(string item)
    {
        Item i;
        if (TryGetValue(item, out i))
            return true;
        return false;
    }

    public bool Contains(Item i)
    {
        Item list;
        if (TryGetValue(i.Name, out list))
            return true;
        return false;
    }
}
