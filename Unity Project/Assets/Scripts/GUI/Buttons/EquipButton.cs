using System;
using UnityEngine;

public class EquipButton : ItemButton
{
    public override void Initialize()
    {
        base.Initialize();
        if (!i.itemFlags[ItemFlags.IS_EQUIPPED])
        {
            this.Text = "Equip Item";
            OnSingleClick = new Action(() =>
            {
                if (i.OnGround == true)
                {
                    BigBoss.Player.Inventory.Add(i);
                    BigBoss.Player.GridSpace.Remove(i);
                    BigBoss.Gooey.ground.Open();
                }
                BigBoss.Player.equipItem(i);
                BigBoss.Gooey.inventory.Open();
                BigBoss.Gooey.itemMenu.Open(i);
            });
        }
        else
        {
            this.Text = "Unequip Item";
            OnSingleClick = new Action(() =>
            {
                if (i != null)
                {
                    BigBoss.Player.unequipItem(i);
                    BigBoss.Gooey.inventory.Open();
                    BigBoss.Gooey.itemMenu.Open(i);
                }
            });
        }
    }
}