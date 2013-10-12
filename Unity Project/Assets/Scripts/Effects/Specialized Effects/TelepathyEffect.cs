using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Telepathy : EffectInstance
{
    public override void Init(NPC n)
    {
        //--player
        //attach FOW revealers to sentient NPC's within a certain distance so they can be seen
        //--npc
        //affects targeting, no effect in here?
    }

    public override void Remove(NPC n)
    {
        //reverse init()
    }

    public override void SetParams()
    {
    }
}
