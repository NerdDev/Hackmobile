using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class MoveTowardsCollision : MonoBehaviour
{
    GameObject target;
    Vector3 targetVector;
    float speed;
    float TimeTillDestruct = 7.5f;
    CharacterController c;

    public void initialize(GameObject target, float speed, Spell OnCollision, IAffectable caster)
    {
        this.target = target;
        targetVector = target.transform.position;
        this.speed = speed;
        TimeTillDestruct += Time.time;

        c = gameObject.GetComponent<CharacterController>();

        //CollisionTrigger col = this.gameObject.AddComponent<CollisionTrigger>();
        //col.spell = OnCollision;
        //col.caster = caster;
        //col.isActive = true;

        StartCoroutine(move());
    }

    IEnumerator move()
    {
        while (enabled)
        {
            float curTime = Time.time;
            Vector3 curLoc = gameObject.transform.position;
            if (target != null) { targetVector = target.transform.position; }
            if (!curLoc.checkXYPosition(targetVector) && curTime < TimeTillDestruct)
            {
                c.MoveStepWise(targetVector, speed);
            }
            yield return null;
        }
    }
}
