using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class InventoryCategory : Dictionary<int, Item>
{
    public string id = "";

    public InventoryCategory()
    {
    }

    public InventoryCategory(string id)
    {
        this.id = id;
    }

    public void Add(Item i)
    {
        Item item;
        i.OnGround = false;
        int hash = i.GetHash();
        if (!this.TryGetValue(hash, out item))
        {
            this.Add(hash, i);
        }
        else
        {
            item.Add();
            if (i != item)
            {
                i.Destroy();
            }
        }
    }

    public bool Remove(Item i)
    {
        Item item;
        int hash = i.GetHash();
        if (TryGetValue(hash, out item))
        {
            if (item.RemoveItem())
            {
                this.Remove(hash);
            }
            return true;
        }
        return false;
    }

    public Item GetForTransfer(Item i)
    {
        Item item;
        if (TryGetValue(i.GetHash(), out item))
        {
            Item newItem = item.GetForTransfer();
            if (newItem != null)
            {
                return newItem;
            }
        }
        return null;
    }

    public bool Contains(int item)
    {
        Item i;
        if (TryGetValue(item, out i))
            return true;
        return false;
    }

    public bool Contains(Item i)
    {
        Item list;
        if (TryGetValue(i.GetHash(), out list))
            return true;
        return false;
    }

    public bool IsEmpty()
    {
        foreach (Item i in this.Values)
        {
            if (i.Count != 0)
            {
                return false;
            }
        }
        return true;
    }
}
