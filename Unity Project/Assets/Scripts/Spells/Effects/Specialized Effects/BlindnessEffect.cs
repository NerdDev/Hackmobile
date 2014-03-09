using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Blindness : EffectInstance
{
    public override void Init(NPC n)
    {
        base.Apply(n);
        if (n is Player)
        {
            Player p = n as Player;

            FOWRevealer rev = p.GO.GetComponent<FOWRevealer>();
            rev.range = new Vector2(1.2f, 1.2f);
            n.CreateTextPop("You feel yourself going blind...");
        }
    }

    public override void Remove(NPC n)
    {
        base.Remove(n);
        if (n is Player)
        {
            Player p = n as Player;

            FOWRevealer rev = p.GO.GetComponent<FOWRevealer>();
            rev.range = new Vector2(4f, 4f);
            n.CreateTextPop("You feel your vision return.");
        }
    }

    protected override void ParseParams(XML.XMLNode x)
    {
    }
}
