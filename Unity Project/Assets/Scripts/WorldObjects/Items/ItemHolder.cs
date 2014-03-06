using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ItemHolder
{
    public string id;
    public int count;

	public ItemHolder(string item, int count)
    {
        this.id = item;
        this.count = count;
    }

	public Item Get() {
		return BigBoss.Objects.Items.Instantiate (id);
	}
}
