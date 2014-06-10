using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CollisionTrigger : MonoBehaviour
{
    public Spell spell;
    public IAffectable caster;
    public bool isActive = false;

    public void Init(Spell OnCollision, IAffectable caster)
    {
        spell = OnCollision;
        this.caster = caster;
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
                if (target == caster) return;
                spell.Activate(caster, target);
                Destroy();
            }
        }
    }

    public void Destroy()
    {
        Destroy(this.gameObject);
        Destroy(this);
    }
}
