using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class PlayerInstance : WOWrapper, IManager
{
    public void Initialize()
    {
        WO = new Player();
    }
}
