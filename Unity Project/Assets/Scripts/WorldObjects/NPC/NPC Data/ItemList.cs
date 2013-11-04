using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ItemList : List<Item>
{
    //in case we want more methods here

    public string id; //item name contained in the list
    public ItemList(string id)
    {
        this.id = id;
    }
}
