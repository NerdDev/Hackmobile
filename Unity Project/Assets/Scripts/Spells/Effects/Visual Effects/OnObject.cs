using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnObject : EffectInstance
{
    GameObject obj;
    protected Spell OnCollision; //used for prefabs that have collision-based effects
    protected float TimedDestruction;
    protected string finishVisual;

    public override void Init(NPC n)
    {
        GameObject go = GameObject.Instantiate(Resources.Load(finishVisual), n.GO.transform.position, Quaternion.identity) as GameObject;
        if (OnCollision != null)
        {
            CollisionTrigger col = go.AddComponent<CollisionTrigger>();
            col.spell = OnCollision;
            col.caster = caster;
            col.isActive = true;
        }
        go.AddComponent<TimedDestruction>().init(.75f);
    }

    protected override void ParseParams(XMLNode x)
    {
        finishVisual = x.SelectString("visual");
    }
}
