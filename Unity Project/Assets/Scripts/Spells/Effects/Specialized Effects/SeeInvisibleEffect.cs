using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SeeInvisible : EffectInstance
{
    public override void Init(NPC n)
    {
        //--player
        //re-enable rendering on all NPC's that are invisible
        //--npc
        //enable targeting for invisible objects in AI functions
    }

    public override void Remove(NPC n)
    {
        //--player
        //disable rendering on NPC's that are invisible
        //--npc
        //disable targeting for invisible objects in AI functions
    }

    public override void SetParams()
    {
    }
}
