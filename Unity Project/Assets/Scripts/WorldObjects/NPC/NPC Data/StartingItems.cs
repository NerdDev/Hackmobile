using System;
using System.Collections.Generic;
using XML;

public class StartingItems : IXmlParsable
{
    public List<LeveledItemList> lists = new List<LeveledItemList>();

    public void ParseXML(XMLNode x)
    {
        foreach (XMLNode node in x.SelectList("list"))
        {
            lists.Add(node.Select<LeveledItemList>());
        }
    }

    public List<Item> GetItems(ushort level)
    {
        List<Item> picks = new List<Item>();
        foreach (LeveledItemList list in lists)
        {
            picks.AddRange(list.GetItems(level));
        }
        return picks;
    }
}