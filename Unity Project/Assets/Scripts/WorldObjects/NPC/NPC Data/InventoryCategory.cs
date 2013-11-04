using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class InventoryCategory : IEnumerable<ItemList>
{
    public string id;
    Dictionary<string, ItemList> dict = new Dictionary<string, ItemList>();

    public InventoryCategory(string id)
    {
        this.id = id;
    }

    public void Add(Item i)
    {
        dict.GetCreate(i.Name).Add(i);
    }

    public bool Remove(Item i)
    {
        ItemList list;
        if (dict.TryGetValue(i.Name, out list))
            return list.Remove(i);
        return false;
    }

    public bool TryGet(string item, out ItemList list)
    {
        return dict.TryGetValue(item, out list);
    }

    public bool Contains(string item)
    {
        ItemList list;
        if (dict.TryGetValue(item, out list))
            return list.Count > 0;
        return false;
    }

    public bool Contains(Item i)
    {
        ItemList list;
        if (dict.TryGetValue(i.Name, out list))
            return list.Contains(i);
        return false;
    }

    public IEnumerator<ItemList> GetEnumerator()
    {
        return dict.Values.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}
