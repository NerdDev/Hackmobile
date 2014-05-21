﻿using System;
using UnityEngine;

public class EatItemButton : ItemButton
{
    public override void Initialize()
    {
        base.Initialize();
        OnSingleClick = new Action(() =>
        {
            if (!i.OnGround)
            {
                this.isEnabled = true;
                BigBoss.Player.eatItem(i);
                BigBoss.Gooey.OpenInventoryGUI();
                BigBoss.Gooey.OpenItemMenu(i);
                BigBoss.Gooey.OpenItemActionsMenu(i);
            }
            else
            {
                this.isEnabled = false;
            }
        });
    }
}