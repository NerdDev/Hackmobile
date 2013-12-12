using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XML;

public class Inventory : SortedDictionary<string, InventoryCategory>, IXmlParsable
{
    public void Add(Item i)
    {
        this.GetCreate(i.Type, i.Type).Add(i);
    }

    public bool Remove(Item i)
    {
        InventoryCategory cate;
        if (TryGetValue(i.Type, out cate))
            return cate.Remove(i);
        return false;
    }

    public bool TryGetValue(string category, string item, out ItemList list)
    {
        InventoryCategory cate;
        if (TryGetValue(category, out cate))
            return cate.TryGetValue(item, out list);
        list = null;
        return false;
    }

    public bool TryGetFirst(string category, string itemName, out Item item)
    {
        ItemList list;
        if (TryGetValue(category, itemName, out list))
        { // list has count > 0
            item = list[0];
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
}
