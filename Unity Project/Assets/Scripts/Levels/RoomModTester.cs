﻿using UnityEngine;
using System.Collections;
using System;

public class RoomModTester : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        System.Random rand = new System.Random();
        for (int i = 0; i < 100; i++)
        {
            try
            {
                LayoutObject obj = new LayoutObject("Room Test " + i);
                RoomSpec spec = new RoomSpec(obj, 1, null, rand);
                TRoomMod troom = new TRoomMod();
                troom.Modify(spec);
                spec.Grids.ToLog(Logs.LevelGenMain, "Test TRoom " + i);
            }
            catch (Exception ex)
            {
                BigBoss.Debug.w(Logs.LevelGenMain, "Exception " + ex);
            }
        }
    }
}