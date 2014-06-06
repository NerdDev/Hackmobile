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

    void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;
        Debug.Log("Collision occurred.");
        Debug.Log(other.gameObject.name);
        NPCInstance wrapper = other.gameObject.GetComponent<NPCInstance>();
        if (wrapper != null)
        {
            if (wrapper.WO is IAffectable)
            {
                IAffectable target = wrapper.WO as IAffectable;
                if (target == caster) return;
                spell.Activate(caster, target);
                Destroy(this.gameObject);
            }
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (!isActive) return;
        Debug.Log("1Collision occurred.");
        Debug.Log(other.gameObject.name);
        NPCInstance wrapper = other.gameObject.GetComponent<NPCInstance>();
        if (wrapper != null)
        {
            if (wrapper.WO is IAffectable)
            {
                IAffectable target = wrapper.WO as IAffectable;
                if (target == caster) return;
                spell.Activate(caster, target);
                Destroy(this.gameObject);
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (!isActive) return;
        NPCInstance wrapper = other.gameObject.GetComponent<NPCInstance>();
        if (wrapper != null)
        {
            if (wrapper.WO is IAffectable)
            {
                IAffectable target = wrapper.WO as IAffectable;
                if (target == caster) return;
                spell.Activate(caster, target);
                Destroy(this.gameObject);
            }
        }
    }
}
