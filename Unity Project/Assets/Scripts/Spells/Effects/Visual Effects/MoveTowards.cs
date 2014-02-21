using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class MoveTowards : MonoBehaviour
{
    GameObject target;
    float speed;
    float time;
    Action finishPoint;

    public void initialize(GameObject target, float speed, Action action = null)
    {
        this.target = target;
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
            if (!curLoc.checkXYPosition(target.transform.position))
            {
                gameObject.MoveStepWise(target.transform.position, speed);
            }
            else
            {
                if (finishPoint != null)
                {
                    finishPoint();
                }
                enabled = false;
                Destroy(gameObject);
                Destroy(this);
            }
            yield return null;
        }
    }
}
