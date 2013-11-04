using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class NPCInstance : WOWrapper
{
    void Update()
    {
        if (WO != null)
        {
            this.WO.Update();
        }
    }

    void FixedUpdate()
    {
        this.WO.FixedUpdate();
    }
}
