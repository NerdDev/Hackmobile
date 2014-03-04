using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallVisual : EffectInstance
{
    GameObject fireball;
    protected int speed;

    public override void Init(NPC n)
    {
        Vector3 vector = this.caster.Self.GO.transform.position;
        Vector3 pos = new Vector3(vector.x, vector.y + .5f, vector.z);
        fireball = GameObject.Instantiate(Resources.Load("FX/Fireball"), pos, Quaternion.identity) as GameObject;
        MoveTowards script = fireball.AddComponent<MoveTowards>();
        script.initialize(n.GO, speed, new Action<Vector3>((Vector3 loc) => 
        {
            GameObject go = GameObject.Instantiate(Resources.Load("FX/FireballExplosion"), loc, Quaternion.identity) as GameObject;
            go.AddComponent<TimedDestruction>().init(.5f);
        }));
    }

    protected override void ParseParams(XML.XMLNode x)
    {
        speed = x.SelectInt("speed");
    }
}
