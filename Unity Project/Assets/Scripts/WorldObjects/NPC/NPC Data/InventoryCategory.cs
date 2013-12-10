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
        if (this.ContainsKey(i.Name))
        {
            if (this[i.Name] != null)
            {
                this[i.Name].Add(i);
            }
            else
            {
                this[i.Name] = new ItemList(i.Name);
                this[i.Name].Add(i);
            }
        }
        else
        {
            this.Add(i.Name, new ItemList(i.Name));
            this[i.Name].Add(i);
        }
        this[i.Name].onGround = false;
    }

    public bool Remove(Item i)
    {
        if (this.ContainsKey(i.Name))
        {
            return this[i.Name].Remove(i);
        }
        return false;
    }

    public ItemList Get(string item)
    {
        if (this.ContainsKey(item))
        {
            if (this[item] != null && this[item].Count > 0)
            {
                return this[item];
            }
        }
        return null; //item isn't there
    }

    public bool Has(string item)
    {
        if (this.ContainsKey(item))
        {
            if (this[item].Count > 0)
            {
                return true;
            }
        }
        return false;
    }

    public bool Has(Item i)
    {
        if (this.ContainsKey(i.Name))
        {
            return this[i.Name].Contains(i);
        }
        return false;
    }
}