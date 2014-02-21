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
        Vector3 pos = new Vector3(BigBoss.Player.GO.transform.position.x, BigBoss.Player.GO.transform.position.y + .5f, BigBoss.Player.GO.transform.position.z);
        fireball = GameObject.Instantiate(Resources.Load("FX/Fireball"), pos, Quaternion.identity) as GameObject;
        MoveTowards script = fireball.AddComponent<MoveTowards>();
        script.initialize(n.GO, speed, new Action(() => 
        {
            (GameObject.Instantiate(Resources.Load("FX/FireballExplosion"), n.GO.transform.position, Quaternion.identity) as GameObject).AddComponent<TimedDestruction>().init(.5f);
        }));
    }

    protected override void ParseParams(XML.XMLNode x)
    {
        speed = x.SelectInt("speed");
    }
}
