using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class InventoryCategory : SortedDictionary<string, ItemList>
{
    public string id;

    public InventoryCategory(string id)
    {
        this.id = id;
    }

    public void Add(Item i)
    {
        ItemList list;
        if (!this.TryGetValue(i.Name, out list))
        {
            list = new ItemList(i.Name);
            this.Add(i.Name, list);
        }
        list.Add(i);
        this[i.Name].onGround = false;
    }

    public bool Remove(Item i)
    {
        ItemList list;
        if (TryGetValue(i.Name, out list))
            return list.Remove(i);
        return false;
    }

    public bool Contains(string item)
    {
        ItemList list;
        if (TryGetValue(item, out list))
            return list.Count > 0;
        return false;
    }

    public bool Contains(Item i)
    {
        ItemList list;
        if (TryGetValue(i.Name, out list))
            return list.Contains(i);
        return false;
    }
}
