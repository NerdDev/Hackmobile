using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Telepathy : EffectInstance
{
    public override void init()
    {
        base.init();
        //--player
        //attach FOW revealers to sentient NPC's within a certain distance so they can be seen
        //--npc
        //affects targeting, no effect in here?
    }

    public override void remove()
    {
        base.apply();
        //reverse init()
    }

    public override void SetParams()
    {
    }
}
