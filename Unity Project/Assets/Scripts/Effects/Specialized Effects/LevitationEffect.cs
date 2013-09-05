﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class LevitationEffect : EffectInstance
{
    Integer strength;

    public override void init()
    {
        base.init();
        n.verticalMove(strength);
    }

    public override void remove()
    {
        base.apply();
        npc.verticalMove(-strength);
    }
}