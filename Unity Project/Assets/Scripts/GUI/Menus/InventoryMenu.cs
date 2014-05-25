using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryMenu : GUIMenu
{
    public override void Close()
    {
        base.Close();
        if (BigBoss.Gooey.itemMenu.isActive) BigBoss.Gooey.itemMenu.Close();
    }
}