using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XML;

public class Spells : IXmlParsable
{
    public Dictionary<string, Spell> spells = new Dictionary<string, Spell>();

    public IEnumerable Keys
    {
        get { return spells.Keys; }
    }

    public bool ContainsKey(string key)
    {
        return spells.ContainsKey(key);
    }

    public Spell this[string key]
    {
        get { return spells[key]; }
    }

    public void Add(string key, Spell spell)
    {
        spells.Add(key, spell);
    }

    public void ParseXML(XML.XMLNode x)
    {
        foreach (XMLNode node in x.SelectList("spell"))
        {
            string spellName = node.SelectString("name");
            Spell s = node.Select<Spell>();
            spells.Add(spellName, s);
            if (!BigBoss.Objects.PlayerSpells.ContainsKey(spellName)) //this is givin' us every spell for the time being, should be removed when Player is properly parsed
            {
                BigBoss.Objects.PlayerSpells.Add(spellName, s);
            }
        }
    }
}
