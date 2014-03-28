using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Blindness : EffectInstance
{
    private FOWRevealer fow;
    private Vector2 originalRange;
    public override void Init(NPC n)
    {
        base.Apply(n);
        if (n is Player)
        {
            Player p = n as Player;

            fow = p.GO.GetComponentInChildren<FOWRevealer>();
            originalRange = fow.range;
            fow.range = new Vector2(0f, .3f);
            n.CreateTextMessage("You feel yourself going blind...");
        }
    }

    public override void Remove(NPC n)
    {
        base.Remove(n);
        if (n is Player)
        {
            if (fow != null && originalRange != null)
                fow.range = originalRange;
            n.CreateTextMessage("You feel your vision return.");
        }
    }

    protected override void ParseParams(XML.XMLNode x)
    {
    }
}
