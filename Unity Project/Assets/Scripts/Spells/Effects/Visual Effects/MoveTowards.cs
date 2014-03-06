using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class MoveTowards : MonoBehaviour
{
    GameObject target;
    Vector3 targetVector;
    float speed;
    float time;
    Action<Vector3> finishPoint;

    public void initialize(GameObject target, float speed, Action<Vector3> action = null)
    {
        this.target = target;
        targetVector = target.transform.position;
        this.speed = speed;
        time = Time.time;
        finishPoint = action;
        StartCoroutine(move());
    }

    IEnumerator move()
    {
        while (enabled)
        {
            Vector3 curLoc = gameObject.transform.position;
            if (target != null) { targetVector = target.transform.position; }
            if (!curLoc.checkXYPosition(targetVector))
            {
                gameObject.MoveStepWise(targetVector, speed);
            }
            else
            {
                if (finishPoint != null)
                {
                    finishPoint(targetVector);
                }
                enabled = false;
                Destroy(gameObject);
                Destroy(this);
            }
            yield return null;
        }
    }
}
