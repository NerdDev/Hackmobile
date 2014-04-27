using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class NPCInstance : WOWrapper
{
    public bool DestroyThisNPC = false;

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
    }
    public float TimeToMove;
    void FixedUpdate()
    {
        if (WO != null)
            this.WO.FixedUpdate();
    }
}
