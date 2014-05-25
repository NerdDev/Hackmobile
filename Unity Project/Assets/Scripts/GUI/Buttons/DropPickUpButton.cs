using System;
using UnityEngine;

public class DropPickUpButton : ItemButton
{
    public override void Initialize()
    {
        base.Initialize();
        if (i.OnGround)
        {
            this.Text = "Pick Up Item";
            OnSingleClick = new Action(() =>
            {
                BigBoss.Gooey.pickUpItem(i);
                BigBoss.Gooey.ground.Open();
                BigBoss.Gooey.inventory.Open();
                BigBoss.Gooey.itemMenu.Open(i);
            });
        }
        else
        {
            this.Text = "Drop Item";
            OnSingleClick = new Action(() =>
            {
                BigBoss.Gooey.dropItem(i);
                BigBoss.Gooey.ground.Open();
                BigBoss.Gooey.inventory.Open();
                BigBoss.Gooey.itemMenu.Open(i);
            });
        }
    }
}