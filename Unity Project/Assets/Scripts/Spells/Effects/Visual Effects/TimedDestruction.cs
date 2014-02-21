using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class TimedDestruction : MonoBehaviour
{
    float startTime;
    float finishTime;
    float time;

    public void init(float time)
    {
        finishTime = time + Time.time;
    }

    void Update()
    {
        if (Time.time > finishTime)
        {
            Destroy(this.gameObject);
            Destroy(this);
        }
    }
}
