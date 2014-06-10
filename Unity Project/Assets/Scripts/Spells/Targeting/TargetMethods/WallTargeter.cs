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
    protected int Width;
    protected int turns;
    protected int rate;

    public override void Activate(SpellCastInfo castInfo)
    {
        HashSet<Vector3> spaces = GetLocationTargets(castInfo);
        foreach (Vector3 space in spaces)
        {
            Quaternion rotationTowardsCaster = Quaternion.LookRotation(castInfo.Caster.Self.GO.transform.position - space);
            Vector3 euler = rotationTowardsCaster.eulerAngles;
            euler.x -= 90;
            rotationTowardsCaster = Quaternion.Euler(euler);

            Vector3 pos = space;
            pos.y += 1;

            GameObject go = GameObject.Instantiate(Resources.Load(visual), pos, rotationTowardsCaster) as GameObject;
            go.transform.localScale = new Vector3(Width, .5f, .5f);

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
        visual = x.SelectString("visual");
        Width = x.SelectInt("width");
        OnCollision = x.SelectSpell<TargetedObjects>("OnCollision");
    }
}
