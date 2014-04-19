using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class NPCInstance : WOWrapper
{
    public bool DestroyThisNPC = false;

    void Update()
    {
        if (WO == null) return;
        WO.Update();

        if (DestroyThisNPC)
        {
            WO.Destroy();
        }
    }

    void FixedUpdate()
    {
        if (WO != null)
        this.WO.FixedUpdate();
    }
}
