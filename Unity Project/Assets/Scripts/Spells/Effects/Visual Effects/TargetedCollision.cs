using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetedCollision : EffectInstance
{
    GameObject obj;
    protected string visual;
    protected Spell OnCollision;
    protected float TimedDestruction;
    protected int TimedTurns;
    protected int speed;

    public override void Init(NPC n)
    {
        Vector3 vector = this.caster.Self.GO.transform.position;
        Vector3 pos = new Vector3(vector.x, vector.y + .5f, vector.z);

        if (speed != 0) //moving object?
        {
            obj = GameObject.Instantiate(Resources.Load(visual), pos, Quaternion.identity) as GameObject;
            MoveTowardsCollision script = obj.AddComponent<MoveTowardsCollision>();
            script.initialize(n.GO, speed, OnCollision, this.caster);
        }
        else //stick it on the target
        {
            obj = GameObject.Instantiate(Resources.Load(visual), n.GO.transform.position, Quaternion.identity) as GameObject;
        }

        if (!Mathf.Approximately(TimedDestruction, 0)) //has a timed portion to it?
        {
            obj.AddComponent<TimedDestruction>().init(TimedDestruction);
        }

        if (TimedTurns > 0)
        {
            //add a turn based script here
        }

        if (OnCollision != null) //any collision based spell on the object?
        {
            CollisionTrigger col = obj.AddComponent<CollisionTrigger>();
            col.effect = OnCollision;
            col.caster = caster;
            col.isActive = true;
        }
    }

    protected override void ParseParams(XMLNode x)
    {
        speed = x.SelectInt("speed");
        TimedDestruction = x.SelectFloat("destructiontime", 0f);
        TimedTurns = x.SelectInt("turntime", 0);
        visual = x.SelectString("visual");
        OnCollision = x.Select<Spell>("OnCollision");
    }
}
