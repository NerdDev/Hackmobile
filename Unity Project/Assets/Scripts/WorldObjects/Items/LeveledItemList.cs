using System;
using System.Collections.Generic;

public class LeveledItemList : LeveledPool<ItemHolder>, IXmlParsable, INamed
{
    public List<string> refs = new List<string>();
    internal bool refsResolved = false;

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
            if (refs.Count > 0)
            {
                UnityEngine.Debug.Log(refs.Count) ;
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
        return base.Get(random, amount, level);
    }

	public void ParseXML(XML.XMLNode node)
	{
		Name = node.SelectString ("name");

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
}