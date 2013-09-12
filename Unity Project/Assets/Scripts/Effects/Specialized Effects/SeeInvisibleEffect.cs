using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SeeInvisible : EffectInstance
{
    public override void init()
    {
        base.init();
        //--player
        //re-enable rendering on all NPC's that are invisible
        //--npc
        //enable targeting for invisible objects in AI functions
    }

    public override void remove()
    {
        base.apply();
        //--player
        //disable rendering on NPC's that are invisible
        //--npc
        //disable targeting for invisible objects in AI functions
    }

    public override void SetParams()
    {
    }
}
