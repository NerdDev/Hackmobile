using System;
using UnityEngine;

public class OpenInventoryButton : GUIButton
{
    public override void Initialize()
    {
        base.Initialize();
        OnSingleClick = new Action(() =>
        {
            BigBoss.Gooey.inventory.Open();
        });
    }
}