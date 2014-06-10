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
    Rigidbody r;

    public void initialize(GameObject target, float speed, IAffectable caster)
    {
        this.target = target;
        targetVector = target.transform.position;
        this.speed = speed;
        TimeTillDestruct += Time.time;

        r = gameObject.GetComponent<Rigidbody>();

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
                r.MoveStepWise(targetVector, speed);
            }
            else
            {
                r.velocity = Vector3.zero;
            }
            yield return null;
        }
    }
}
