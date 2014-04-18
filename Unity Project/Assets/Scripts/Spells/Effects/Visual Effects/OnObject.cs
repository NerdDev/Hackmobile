using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnObject : EffectInstance
{
    GameObject obj;
    protected string finishVisual;

    public override void Init(NPC n)
    {
        GameObject go = GameObject.Instantiate(Resources.Load(finishVisual), n.GO.transform.position, Quaternion.identity) as GameObject;
        go.AddComponent<TimedDestruction>().init(.5f);
    }

    protected override void ParseParams(XMLNode x)
    {
        finishVisual = x.SelectString("finishvisual");
    }
}
