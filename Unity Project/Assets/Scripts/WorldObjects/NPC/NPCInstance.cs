using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class NPCInstance : WOWrapper
{
    public bool Active;
    void Update()
    {
        if (WO == null) return;
        WO.Update();
        Active = WO.IsActive;
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
