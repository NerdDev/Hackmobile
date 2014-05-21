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
}