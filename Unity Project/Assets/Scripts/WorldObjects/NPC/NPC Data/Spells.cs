using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XML;

public class Spells : Dictionary<string, Spell>, IXmlParsable
{
    public void ParseXML(XML.XMLNode x)
    {
        foreach (XMLNode node in x.SelectList("spell"))
        {
            string spellName = node.SelectString("name");
            Spell s = node.Select<Spell>();
            this.Add(spellName, s);
            if (!BigBoss.Objects.PlayerSpells.ContainsKey(spellName)) //this is givin' us every spell for the time being, should be removed when Player is properly parsed
            {
                BigBoss.Objects.PlayerSpells.Add(spellName, s);
            }
        }
    }
}
