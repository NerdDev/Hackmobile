﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class TimedDestruction : MonoBehaviour
{
    float startTime;
    float finishTime;
    public float TimeTillDestruction = 500;

    public void init(float time)
    {
        finishTime = time + Time.time;
    }

    public void Start()
    {
        if (finishTime == 0) finishTime = TimeTillDestruction + Time.time;
    }

    void Update()
    {
        if (Time.time > finishTime)
        {
            if (this.gameObject != null) Destroy(this.gameObject);
            Destroy(this);
        }
    }
}