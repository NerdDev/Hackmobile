using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class NPCInstance : WOWrapper
{
    //display values
    public bool DestroyThisNPC = false;
    public int TurnPoints;

    void Start()
    {
        WO.Start();
    }

    void Update()
    {
        if (WO == null) return;
        WO.Update();

        if (DestroyThisNPC)
        {
            WO.Destroy();
        }
        else
        {
            TurnPoints = WO.CurrentPoints;
        }
    }
    
    void FixedUpdate()
    {
        if (WO != null)
            this.WO.FixedUpdate();
    }
}
