using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class SeeInvisibleEffect : EffectInstance
{
    public override void init()
    {
        base.init();
    }

    public override void remove()
    {
        base.apply();
    }

    public override void SetParams()
    {
    }
}
