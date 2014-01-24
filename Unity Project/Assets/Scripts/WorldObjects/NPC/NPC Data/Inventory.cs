using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XML;

public class Inventory : SortedDictionary<string, InventoryCategory>, IXmlParsable
{
    public void Add(Item i)
    {
        InventoryCategory ic;
        if (!TryGetValue(i.Type, out ic))
        {
            ic = new InventoryCategory(i.Type);
            Add(i.Type, ic);
        }
        ic.Add(i);
    }

    public bool Remove(Item i)
    {
        InventoryCategory cate;
        if (TryGetValue(i.Type, out cate))
            return cate.Remove(i);
        return false;
    }

    public bool TryGetValue(string category, string item, out Item list)
    {
        InventoryCategory cate;
        if (TryGetValue(category, out cate))
            return cate.TryGetValue(item, out list);
        list = null;
        return false;
    }

    //unused now?
    public bool TryGetFirst(string category, string itemName, out Item item)
    {
        Item list;
        if (TryGetValue(category, itemName, out list))
        { // list has count > 0
            item = list;
            return true;
        }
        item = null;
        return false;
    }

    public bool Contains(string category, string item)
    {
        InventoryCategory cate;
        if (TryGetValue(category, out cate))
            return cate.Contains(item);
        return false;
    }

    public bool Contains(string item)
    {
        foreach (InventoryCategory ic in Values)
            if (ic.Contains(item))
                return true;
        return false;
    }

    public bool Contains(Item i)
    {
        InventoryCategory cata;
        string t = i.Type;
        if (TryGetValue(i.Type, out cata))
            return cata.Contains(i);
        return false;
    }

    public void ParseXML(XMLNode x)
    {
    }

    internal void Add(Item item, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Add(item);
        }
    }

    internal void Remove(Item item, int count)
    {
        for (int i = 0; i < count; i++)
        {
            Remove(item);
        }
    }

    internal bool IsEmpty()
    {
        foreach (InventoryCategory ic in this.Values)
        {
            if (!ic.IsEmpty()) return false;
        }
        return true;
    }
}
