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
    public TargetingStyle Style { get { return TargetingStyle.TargetLocation; } }
    public byte MaxTargets { get { return 1; } set { } }

    // Unique WallTargeter Variables
    protected string visual;
    protected Spell OnCollision;
    protected int Width;

    public override void Activate(SpellCastInfo castInfo)
    {
        Debug.Log(visual);
        Debug.Log(Width);
        Debug.Log(OnCollision.Dump());
        HashSet<GridSpace> spaces = GetGridTargets(castInfo);
        foreach (GridSpace space in spaces)
        {
            Quaternion rotationTowardsCaster = Quaternion.FromToRotation(castInfo.Caster.Self.GO.transform.position, space.Blocks[0].transform.position);
            GameObject go = GameObject.Instantiate(Resources.Load(visual), space.Blocks[0].transform.position, rotationTowardsCaster) as GameObject;
            go.transform.localScale = new Vector3(.5f, Width, .5f);

            CollisionTrigger col = go.AddComponent<CollisionTrigger>();
            col.caster = castInfo.Caster;
            col.spell = OnCollision;
            col.isActive = true;
        }
    }

    public override HashSet<GridSpace> GetGridTargets(SpellCastInfo castInfo)
    {
        return new HashSet<GridSpace>(castInfo.TargetSpaces);
    }

    public int GetHash()
    {
        int hash = 5;
        hash += Style.GetHashCode() * 13;
        hash += MaxTargets.GetHashCode() * 3;
        return base.GetHash() + hash;
    }

    public override void ParseXML(XMLNode x)
    {
        base.ParseXML(x);
        visual = x.SelectString("visual");
        Width = x.SelectInt("width");
        OnCollision = x.Select<Spell>("spell");
    }
}
