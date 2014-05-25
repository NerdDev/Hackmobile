using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemMenu : GUIMenu
{
    public Registerable<Item> item =  new Registerable<Item>();

    public void Display(Item item)
    {
        this.item.Set(item);
        base.Display();
    }

    public void Open(Item i)
    {
        Display(i);
        if (BigBoss.Gooey.spellMenu.isActive) BigBoss.Gooey.spellMenu.Close();
        if (BigBoss.Gooey.text.isActive) BigBoss.Gooey.text.Close();
    }

    public override void Open()
    {
        base.Open();
        if (BigBoss.Gooey.spellMenu.isActive) BigBoss.Gooey.spellMenu.Close();
        if (BigBoss.Gooey.text.isActive) BigBoss.Gooey.text.Close();
    }

    public override void Close()
    {
        base.Close();
        if (!BigBoss.Gooey.spellMenu.isActive) BigBoss.Gooey.spellMenu.Open();
        if (!BigBoss.Gooey.text.isActive) BigBoss.Gooey.text.Open();
    }
}