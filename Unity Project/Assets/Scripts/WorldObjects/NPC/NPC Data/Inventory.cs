using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XML;

public class Inventory : IEnumerable<InventoryCategory>, IXmlParsable
{
    Dictionary<string, InventoryCategory> dict = new Dictionary<string, InventoryCategory>();

    public void Add(Item i)
    {
        dict.GetCreate(i.Type).Add(i);
    }

    public bool Remove(Item i)
    {
        InventoryCategory cate;
        if (dict.TryGetValue(i.Type, out cate))
            return cate.Remove(i);
        return false;
    }

    public bool TryGet(string name, out InventoryCategory category)
    {
        return dict.TryGetValue(name, out category);
    }

    public bool TryGet(string category, string item, out ItemList list)
    {
        InventoryCategory cate;
        if (dict.TryGetValue(category, out cate))
            return cate.TryGet(item, out list);
        list = null;
        return false;
    }

    public bool TryGetFirst(string category, string itemName, out Item item)
    {
        ItemList list;
        if (TryGet(category, itemName, out list))
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
        if (dict.TryGetValue(category, out cate))
            return cate.Contains(item);
        return false;
    }

    public bool Contains(string item)
    {
        foreach (InventoryCategory ic in dict.Values)
            if (ic.Contains(item))
                return true;
        return false;
    }

    public bool Contains(Item i)
    {
        InventoryCategory cata;
        if (dict.TryGetValue(i.Type, out cata))
            return cata.Contains(i);
        return false;
    }

    public void ParseXML(XMLNode x)
    {
    }

    public IEnumerator<InventoryCategory> GetEnumerator()
    {
        return dict.Values.GetEnumerator();
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return this.GetEnumerator();
    }
}
