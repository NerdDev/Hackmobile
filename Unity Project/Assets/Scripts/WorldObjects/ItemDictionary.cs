using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XML;

public class ItemDictionary : WODictionary<Item>
{
    public Dictionary<string, List<Item>> Categories { get; protected set; }

    public ItemDictionary()
        : base()
    {
        Categories = new Dictionary<string, List<Item>>();
    }

    public bool Add(Item obj, string category)
    {
        if (Add(obj))
        {
            List<Item> cateList = Categories.GetCreate(category);
            cateList.Add(obj);
            return true;
        }
        return false;
    }

    public override void Parse(XMLNode itemsNode)
    {
        foreach (XMLNode categoryNode in itemsNode)
        {
            foreach (XMLNode itemNode in categoryNode)
            {
                Item i = new Item();
                i.ParseXML(itemNode);
                if (!Add(i, categoryNode.Key) && BigBoss.Debug.logging(Logs.XML))
                {
                    BigBoss.Debug.w(Logs.XML, "Item already existed with name: " + i.Name + " under node " + itemNode);
                }
            }
        }
    }
}
