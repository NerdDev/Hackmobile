using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XML;

public class Spells : SortedDictionary<string, Spell>, IXmlParsable
{
    public void ParseXML(XML.XMLNode x)
    {
        foreach (XMLNode node in x.SelectList("spell"))
        {
            string spellName = node.SelectString("name");
            Spell s = node.Select<Spell>();
            this.Add(spellName, s);
            if (!BigBoss.Objects.PlayerSpells.ContainsKey(spellName))
            {
                BigBoss.Objects.PlayerSpells.Add(spellName, s);
            }
        }
    }
}
