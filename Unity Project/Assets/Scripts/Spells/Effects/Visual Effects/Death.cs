using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : EffectInstance
{
    float speed;
    static int deathState = Animator.StringToHash("Base Layer.death");

    public override void Init(NPC n)
    {
        n.JustUnregister();
        n.GO.GetComponent<Collider>().enabled = false;
        n.animator.Play(deathState);
        n.GO.AddComponent<TimedAction>().init(speed, new Action(() => { n.JustDestroy(); }));
    }

    protected override void ParseParams(XMLNode x)
    {
        speed = x.SelectFloat("speed");
    }
}
