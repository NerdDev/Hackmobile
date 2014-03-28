using System;
using System.Collections.Generic;

public class LeveledItemList : LeveledPool<ItemHolder>, IXmlParsable, INamed
{
    public List<string> refs = new List<string>();
    internal bool refsResolved = false;
    private int minNumItems;
    private int maxNumItems;

	public string Name {
				get;
				set;
	}

	public LeveledItemList() {
		levelCurve = new LeveledCurve((playerLevel, itemLevel) =>
				{
						double diff = Math.Abs (itemLevel - playerLevel);
						// Return the probability multiplier we want for this current itemLevel
						if (diff > 10) {
								return -1;
						}
						return Math.Pow (2, -diff);
				});
	}

	public LeveledItemList(LeveledCurve curve)
	{
		this.levelCurve = curve;
	}

    public override List<ItemHolder> Get(Random random, int amount, ushort level)
    {
        if (!refsResolved)
        {
            ResolveListReferences();
        }
        return base.Get(random, amount, level);
    }

    public override List<ItemHolder> Get(Random random, int min, int max, ushort level)
    {
        if (!refsResolved)
        {
            ResolveListReferences();
        }
        return base.Get(random, min, max, level);
    }

    public List<ItemHolder> Get(ushort level)
    {
        if (!refsResolved)
        {
            ResolveListReferences();
        }
        return base.Get(new System.Random(), minNumItems, maxNumItems, level);
    }

    private void ResolveListReferences()
    {
        if (refs.Count > 0)
        {
            UnityEngine.Debug.Log(refs.Count);
            UnityEngine.Debug.Log(refs[0]);
        }
        foreach (string str in refs)
        {
            if (BigBoss.Objects.LeveledItems.ContainsKey(str))
            {
                this.AddAll(BigBoss.Objects.LeveledItems[str]);
            }
        }
        refsResolved = true;
    }

	public void ParseXML(XML.XMLNode node)
	{
		Name = node.SelectString ("name");
        minNumItems = node.SelectInt("min", 0);
        maxNumItems = node.SelectInt("max", 1);

		foreach (XML.XMLNode x in node.SelectList("entry")) {
			string item = x.SelectString("item");
			double probability = x.SelectDouble("probability");
			int count = x.SelectInt("count");
			int level = x.SelectInt ("level");
			ItemHolder ih = new ItemHolder(item, count);
			Add(ih, probability, false, (ushort) level);
		}
        foreach (XML.XMLNode x in node.SelectList("ref"))
        {
            refs.Add(x.SelectString("name"));
        }
	}

    public List<Item> GetItems(ushort level)
    {
        List<Item> picks = new List<Item>();
        List<ItemHolder> holders = Get(level);
        foreach (ItemHolder holder in holders)
        {
            picks.Add(holder.Get());
        }
        return picks;
    }
}