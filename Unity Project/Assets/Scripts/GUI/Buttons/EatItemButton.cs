using System;
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
                BigBoss.Gooey.inventory.Open();
                BigBoss.Gooey.itemMenu.Open(i);
            }
            else
            {
                this.isEnabled = false;
            }
        });
    }
}