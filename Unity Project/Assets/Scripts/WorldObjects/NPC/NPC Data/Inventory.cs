using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XML;

public class Inventory : SortedDictionary<string, InventoryCategory>, IXmlParsable
{
    int weight = 0;

    public void Add(Item i)
    {
        InventoryCategory ic;
        if (!TryGetValue(i.Type, out ic))
        {
            ic = new InventoryCategory(i.Type);
            Add(i.Type, ic);
        }
        ic.Add(i);
        weight += i.stats.Weight;
    }

    public bool Remove(Item i)
    {
        InventoryCategory cate;
        if (TryGetValue(i.Type, out cate))
        {
            if (cate.Remove(i))
            {
                weight -= i.stats.Weight;
            }
            if (cate.Count == 0) this.Remove(cate.id);
        }
        return false;
    }

    internal Item GetForTransfer(Item i)
    {
        InventoryCategory cate;
        if (TryGetValue(i.Type, out cate))
        {
            Item newItem = cate.GetForTransfer(i);
            if (newItem != null)
            {
                return newItem;
            }
        }
        return null;
    }

    public bool TransferTo(Item i, Inventory inv, int count = 1)
    {
        Item newItem = GetForTransfer(i);
        if (newItem != null)
        {
            Remove(i, count);
            inv.Add(newItem, count);
            return true;
        }
        return false;
    }

    public bool TransferTo(Item i, GridSpace space, int count = 1)
    {
        count = CountCheck(i, count);
        Item newItem = GetForTransfer(i);
        if (newItem != null)
        {
            Remove(i, count);
            space.Put(newItem, count);
            return true;
        }
        return false;
    }

    public bool TransferFrom(Item i, GridSpace space, int count = 1)
    {
        count = CountCheck(i, count);
        Item newItem = space.inventory.GetForTransfer(i);
        if (newItem != null)
        {
            space.Remove(i, count);
            this.Add(newItem, count);
            return true;
        }
        return false;
    }

    private int CountCheck(Item i, int count)
    {
        if (i.Count < count)
        {
            count = i.Count;
        }
        return count;
    }

    public bool TryGetValue(string category, string item, out Item list)
    {
        InventoryCategory cate;
        if (TryGetValue(category, out cate))
            return cate.TryGetValue(item, out list);
        list = null;
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
