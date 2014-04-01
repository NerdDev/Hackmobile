using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class TimedAction : MonoBehaviour
{
    float finishTime;
    public float TimeTillAction = 500;
    Action action;

    public void init(float time, Action action)
    {
        finishTime = time + Time.time;
        this.action = action;
    }

    public void Start()
    {
        if (finishTime == 0) finishTime = TimeTillAction + Time.time;
    }

    void Update()
    {
        if (Time.time > finishTime)
        {
            action();
        }
    }
}
