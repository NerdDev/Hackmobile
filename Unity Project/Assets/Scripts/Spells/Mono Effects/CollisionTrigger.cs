﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class CollisionTrigger : MonoBehaviour
{
    public Spell effect;
    public IAffectable caster;
    public bool isActive = false;

    void OnTriggerEnter(Collider other)
    {
        if (!isActive) return;
        NPCInstance wrapper = other.gameObject.GetComponent<NPCInstance>();
        if (wrapper != null)
        {
            if (wrapper.WO is IAffectable)
            {
                IAffectable target = wrapper.WO as IAffectable;
                if (target == caster) return;
                effect.Activate(caster, target);
                Destroy(this.gameObject);
            }
        }
    }
}
