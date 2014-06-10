using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class TimedCollisionTrigger : MonoBehaviour, PassesTurns
{
    public Spell spell;
    public IAffectable caster;
    public bool destroy = true;
    public bool isActive = false;
    public bool affectCaster = false;
    ulong startTurn = 0;
    ulong finishTurn = 0;
    bool canActivate = false;

    public Dictionary<IAffectable, bool> Activations = new Dictionary<IAffectable, bool>();

    void Update()
    {
        if (IsActive && BigBoss.Time.CurrentTurn > finishTurn)
        {
            Destroy();
        }
    }

    public void Init(Spell OnCollision, IAffectable caster, int turns, int rate, bool affectCaster, bool destroyOnCollision)
    {
        this.Rate = rate;
        this.startTurn = BigBoss.Time.CurrentTurn;
        this.finishTurn = startTurn + (ulong)turns;
        IsActive = true;
        BigBoss.Time.Register(this);
        this.affectCaster = affectCaster;
        spell = OnCollision;
        this.caster = caster;
        this.destroy = destroyOnCollision;
        isActive = true;
    }
    void OnTriggerEnter(Collider other)
    {
        GameObject GO = other.gameObject;
        Activate(GO);
    }

    void OnCollisionEnter(Collision other)
    {
        GameObject GO = other.gameObject;
        Activate(GO);
    }

    void OnTriggerStay(Collider other)
    {
        GameObject GO = other.gameObject;
        Activate(GO);
    }

    private void Activate(GameObject GO)
    {
        if (!isActive) return;
        NPCInstance wrapper = GO.GetComponent<NPCInstance>();
        if (wrapper != null)
        {
            if (wrapper.WO is IAffectable)
            {
                IAffectable target = wrapper.WO as IAffectable;
                if (!affectCaster && target == caster) return;
                if (!Activations.ContainsKey(target)) Activations.Add(target, true);
                if (Activations.ContainsKey(target) && Activations[target])
                {
                    spell.Activate(caster, target);
                    Activations[target] = false;
                    if (destroy)
                    {
                        Destroy();
                    }
                }
            }
        }
    }

    public void Destroy()
    {
        if (IsActive) BigBoss.Time.Remove(this);
        Destroy(this.gameObject);
        Destroy(this);
    }

    public void UpdateTurn()
    {
        foreach (IAffectable affectable in Activations.Keys.ToList())
        {
            Activations[affectable] = true;
        }
    }

    public bool IsActive
    {
        get;
        set;
    }

    public int TurnID
    {
        get;
        set;
    }

    public int Rate
    {
        get;
        set;
    }
}
