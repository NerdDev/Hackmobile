using System;
using System.Collections.Generic;
using UnityEngine;

public class GroundMenu : GUIMenu
{
    public ItemChest chest;

    public void Display(ItemChest chest)
    {
        this.chest = chest;
        base.Display();
    }

    public void Open(ItemChest chest)
    {
        Display(chest);
    }
}