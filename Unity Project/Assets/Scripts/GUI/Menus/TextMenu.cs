using System;
using System.Collections.Generic;
using UnityEngine;

public class TextMenu : GUIMenu
{
    public override void Open()
    {
        base.Open();
        if (BigBoss.Gooey.itemMenu.isActive) BigBoss.Gooey.itemMenu.Close();
    }
}