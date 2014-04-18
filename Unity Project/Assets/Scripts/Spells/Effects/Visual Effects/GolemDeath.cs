
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GolemDeath : EffectInstance
{
    public override void Init(NPC n)
    {
        n.JustUnregister();
        n.GO.GetComponent<Collider>().enabled = false;
        n.GO.AddComponent<TimedAction>().init(.2f, new Action(() => { n.JustDestroy(); }));
    }

    protected override void ParseParams(XMLNode x)
    {
    }
}
