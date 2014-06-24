using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

/*
 * Targeter meant to target only selected objects
 */
public class WallTargeter : Targeter
{
    public override TargetingStyle Style { get { return TargetingStyle.TargetLocation; } }
    public override byte MaxTargets { get { return 1; } set { } }

    // Unique WallTargeter Variables
    protected string visual;
    protected Spell OnCollision;
    protected Vector3 Size;
    protected Vector3 Rotation;
    protected Vector3 Position;
    protected bool RotateTowardsCaster;
    protected int turns;
    protected int rate;

    public override void Activate(SpellCastInfo castInfo)
    {
        HashSet<Vector3> spaces = GetLocationTargets(castInfo);
        foreach (Vector3 space in spaces)
        {
            Quaternion rotationTowardsCaster = Quaternion.identity;
            if (RotateTowardsCaster)
            {
                rotationTowardsCaster = Quaternion.LookRotation(castInfo.Caster.Self.GO.transform.position - space);
                Vector3 euler = rotationTowardsCaster.eulerAngles;
                euler += Rotation;
                rotationTowardsCaster = Quaternion.Euler(euler);
            }
            else
            {
                Vector3 euler = rotationTowardsCaster.eulerAngles;
                euler += Rotation;
                rotationTowardsCaster = Quaternion.Euler(euler);
            }

            Vector3 pos = space;
            pos += Position;

            GameObject go = GameObject.Instantiate(Resources.Load(visual), pos, rotationTowardsCaster) as GameObject;
            go.transform.localScale = Size;

            TimedCollisionTrigger col = go.AddComponent<TimedCollisionTrigger>();
            col.Init(OnCollision, castInfo.Caster, turns, rate, true, false);
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
        return base.GetHash() + hash;
    }

    public override void ParseXML(XMLNode x)
    {
        base.ParseXML(x);
        turns = x.SelectInt("turns");
        rate = x.SelectInt("rate");
        RotateTowardsCaster = x.SelectBool("RotateTowardsCaster", true);
        visual = x.SelectString("visual");
        Size.x = x.SelectFloat("sx", 1);
        Size.y = x.SelectFloat("sy", 1);
        Size.z = x.SelectFloat("sz", 1);
        Rotation.x = x.SelectFloat("rx", 0);
        Rotation.y = x.SelectFloat("ry", 0);
        Rotation.z = x.SelectFloat("rz", 0);
        Position.x = x.SelectFloat("px", 0);
        Position.y = x.SelectFloat("py", 0);
        Position.z = x.SelectFloat("pz", 0);
        OnCollision = x.SelectSpell<TargetedObjects>("OnCollision");
    }
}
