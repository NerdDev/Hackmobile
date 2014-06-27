using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class MoveInLineCollision : MonoBehaviour
{
    GameObject target;
    Vector3 targetVector;
    bool StopAtTarget = false;
    float speed;
    float TimeTillDestruct = 7.5f;
    Rigidbody r;

    private bool moving;

    public void initialize(Vector3 target, float speed, bool StopAtTarget, IAffectable caster)
    {
        this.targetVector = target;
        this.speed = speed;
        this.StopAtTarget = StopAtTarget;
        TimeTillDestruct += Time.time;

        r = gameObject.GetComponent<Rigidbody>();

        StartCoroutine(move());
    }

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
            if (StopAtTarget)
            {
                if (!curLoc.checkXYPosition(targetVector))
                {
                    r.MoveStepWise(targetVector, speed);
                }
                else
                {
                    r.velocity = Vector3.zero;
                }
            }
            else if (!moving)
            {
                Vector3 heading = new Vector3(targetVector.x - r.transform.position.x, 0f, targetVector.z - r.transform.position.z);
                r.velocity = (heading.normalized * speed);
                Quaternion toRot = Quaternion.LookRotation(heading);
                r.MoveRotation(toRot);
                moving = true;
            }
            yield return null;
        }
    }
}
