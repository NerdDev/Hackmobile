using System;

public class LeveledItemList : LeveledPool<ItemHolder>, IXmlParsable, INamed
{
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
	}
}