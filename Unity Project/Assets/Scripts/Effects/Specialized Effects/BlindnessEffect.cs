using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Blindness : EffectInstance
{
    public override void init()
    {
        base.init();
        //--player
        //put fog of war to bare minimum so it's all black
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
