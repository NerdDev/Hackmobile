using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XML;

public class Inventory : SortedDictionary<string, InventoryCategory>, IXmlParsable
{
    public void Add(Item i)
    {
        if (this.ContainsKey(i.Type))
        {
            if (this[i.Type] != null)
            {
                this[i.Type].Add(i);
            }
            else
            {
                this[i.Type] = new InventoryCategory(i.Type);
                this[i.Type].Add(i);
            }
        }
        else
        {
            this.Add(i.Type, new InventoryCategory(i.Type));
            this[i.Type].Add(i);
        }
    }

    public bool Remove(Item i)
    {
        if (this.ContainsKey(i.Type))
        {
            return this[i.Type].Remove(i);
        }
        return false;
    }

    public ItemList Get(string category, string item)
    {
        if (this.ContainsKey(category))
        {
            return this[category].Get(item);
        }
        return null; //item isn't there
    }

    public bool Has(string category, string item)
    {
        if (this.ContainsKey(category))
        {
            return this[category].Has(item);
        }
        return false;
    }

    public bool Has(string item)
    {
        foreach (InventoryCategory ic in this.Values)
        {
            if (ic.Has(item))
            {
                return true;
            }
        }
        return false;
    }

    public bool Has(Item i)
    {
        if (this.ContainsKey(i.Type))
        {
            return this[i.Type].Has(i);
        }
        return false;
    }

    public void ParseXML(XMLNode x)
    {
    }
}
