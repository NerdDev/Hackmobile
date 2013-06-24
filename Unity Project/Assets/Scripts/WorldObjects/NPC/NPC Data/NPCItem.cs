using System;
using XML;

public class NPCItem
{
    string itemName;
    public string Name { get { return itemName; } set { this.itemName = value; } }
    int chance;
    public int Chance { get { return chance; } set { this.chance = value; } }
    int count;
    public int Count { get { return count; } set { this.count = value; } }

    public void parseXML(XMLNode x)
    {
        this.Name = x.SelectString("itemname");
        this.Chance = x.SelectInt("chance");
        this.Count = x.SelectInt("count");
    }
}