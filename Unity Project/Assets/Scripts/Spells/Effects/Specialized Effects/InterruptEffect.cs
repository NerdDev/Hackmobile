using System;
using UnityEngine;

public class InterruptEffect : EffectInstance
{
    public int interruptTime;

    public override void Init(NPC n)
    {
        n.Interrupt(interruptTime);
    }

    protected override void ParseParams(XMLNode node)
    {
        interruptTime = node.SelectInt("time");
    }
}
