using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Invisibility : EffectInstance
{
    public override void Init(NPC n)
    {
        //--npc
        //disable rendering on object
        //--player
        //apply shader to player, NPC's should check for invisibility in their AI functions, not here
    }

    public override void Apply(NPC n)
    {
        //check if Player has SeeInvisible and enable rendering if so, disable if otherwise
    }

    public override void Remove(NPC n)
    {
        //reverse init()
    }

    protected override void ParseParams(XML.XMLNode x)
    {
    }
}
