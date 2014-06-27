using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/*
 * Targeter meant to target only selected objects
 */
public class LineTargeter : Targeter
{
    public override TargetingStyle Style { get { return TargetingStyle.TargetLocation; } }
    public override byte MaxTargets { get { return 1; } set { } }

    GameObject obj;
    protected string visual;
    protected Spell OnCollision;
    protected float TimedDestruction;
    protected int TimedTurns;
    protected int speed;
    protected bool StopAtTarget;

    public override void Activate(SpellCastInfo castInfo)
    {
        HashSet<Vector3> spaces = GetLocationTargets(castInfo);
        foreach (Vector3 space in spaces)
        {
            Vector3 pos = new Vector3(space.x, space.y + 1.5f, space.z);

            Vector3 startPos = castInfo.Caster.Self.GO.transform.position;
            startPos.y = pos.y;

            if (speed != 0) //moving object?
            {
                obj = GameObject.Instantiate(Resources.Load(visual), startPos, Quaternion.identity) as GameObject;
                MoveInLineCollision script = obj.AddComponent<MoveInLineCollision>();
                script.initialize(pos, speed, StopAtTarget, castInfo.Caster);
            }
            else //stick it on the target
            {
                obj = GameObject.Instantiate(Resources.Load(visual), pos, Quaternion.identity) as GameObject;
            }

            if (!Mathf.Approximately(TimedDestruction, 0)) //has a timed portion to it?
            {
                obj.AddMissingComponent<TimedDestruction>().init(TimedDestruction);
            }
            else
            {
                obj.AddMissingComponent<TimedDestruction>().init(12);
            }

            if (TimedTurns > 0)
            {
                //add a turn based script here
            }

            if (OnCollision != null) //any collision based spell on the object?
            {
                CollisionTrigger col = obj.AddComponent<CollisionTrigger>();
                col.Init(OnCollision, castInfo.Caster);
            }
        }
    }

    public override HashSet<Vector3> GetLocationTargets(SpellCastInfo castInfo)
    {
        return new HashSet<Vector3>(castInfo.TargetLocations);
    }

    public override int GetHash()
    {
        int hash = 5;
        hash += Style.GetHashCode() * 13;
        hash += MaxTargets.GetHashCode() * 3;
        hash += visual.GetHash() * 5;
        hash += OnCollision.GetHash() * 7;
        hash += TimedDestruction.GetHashCode() * 45;
        hash += TimedTurns.GetHashCode() * 9;
        hash += speed.GetHashCode() * 11;
        hash += StopAtTarget.GetHashCode() * 7;
        return base.GetHash() + hash;
    }

    public override void ParseXML(XMLNode x)
    {
        base.ParseXML(x);
        visual = x.SelectString("visual");
        OnCollision = x.SelectSpell<TargetedObjects>("OnCollision");
        TimedDestruction = x.SelectFloat("destructiontime", 0);
        TimedTurns = x.SelectInt("turntime", 0);
        StopAtTarget = x.SelectBool("StopAtTarget", true);
        speed = x.SelectInt("speed", 0);
    }
}
