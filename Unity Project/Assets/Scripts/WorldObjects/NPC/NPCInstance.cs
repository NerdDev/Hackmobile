using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class NPCInstance : WOWrapper
{
    void Update()
    {
        if (WO == null) return;
        WO.Update();
    }

    void FixedUpdate()
    {
        if (WO != null)
        this.WO.FixedUpdate();
    }
}
