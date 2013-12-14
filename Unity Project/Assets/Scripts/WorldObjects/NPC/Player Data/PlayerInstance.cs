using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class PlayerInstance : NPCInstance, IManager
{
    public bool Initialized { get; set; }
    public void Initialize()
    {
        this.SetTo(new Player());
        WO.Init();
    }
}
