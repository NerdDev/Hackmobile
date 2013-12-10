using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class NPCInstance : WOWrapper
{
    public bool active;
    void Update()
    {
        if (WO != null)
        {
            this.WO.Update();
            active = WO.IsActive;
        }
    }

    void FixedUpdate()
    {
        this.WO.FixedUpdate();
    }

    void OnMouseDown()
    {
        WO.OnClick();
    }
}
