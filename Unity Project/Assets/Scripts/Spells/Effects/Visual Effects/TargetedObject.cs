using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetedObject : EffectInstance
{
    GameObject obj;
    protected string startVisual;
    protected string finishVisual;
    protected int speed;

    public override void Init(NPC n)
    {
        Vector3 vector = this.caster.Self.GO.transform.position;
        Vector3 pos = new Vector3(vector.x, vector.y + .5f, vector.z);
        obj = GameObject.Instantiate(Resources.Load(startVisual), pos, Quaternion.identity) as GameObject;
        MoveTowards script = obj.AddComponent<MoveTowards>();
        script.initialize(n.GO, speed, new Action<Vector3>((Vector3 loc) => 
        {
            GameObject go = GameObject.Instantiate(Resources.Load(finishVisual), loc, Quaternion.identity) as GameObject;
            go.AddComponent<TimedDestruction>().init(.5f);
        }));
    }

    protected override void ParseParams(XML.XMLNode x)
    {
        speed = x.SelectInt("speed");
        startVisual = x.SelectString("startvisual");
        finishVisual = x.SelectString("finishvisual");
    }
}
