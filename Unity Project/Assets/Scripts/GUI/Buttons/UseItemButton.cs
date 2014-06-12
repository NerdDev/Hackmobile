using System;
using UnityEngine;

public class UseItemButton : ItemButton
{
    public override void Initialize()
    {
        base.Initialize();
        OnSingleClick = new Action(() =>
        {
            BigBoss.Player.useItem(i);
            BigBoss.Gooey.inventory.Open();
            BigBoss.Gooey.itemMenu.Open(i);
        });
    }
}