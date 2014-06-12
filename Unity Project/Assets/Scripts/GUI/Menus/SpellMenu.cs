using System;
using System.Collections.Generic;
using UnityEngine;

public class SpellMenu : GUIMenu
{
    public Registerable<Spell>[] SpellBar = new Registerable<Spell>[8];

    public GUIButton CastButton;
    public GUIButton CancelButton;

    HashSet<SpellButton> registeredObjects = new HashSet<SpellButton>();

    void Start()
    {
        SpellBar.Initialize();
    }

    public void Display()
    {
        base.Display();
    }

    public void Register(SpellButton button, Action<Spell, Spell> action)
    {
        for (int i = 0; i < SpellBar.Length; i++)
        {
            if (SpellBar[i] == null) SpellBar[i] = new Registerable<Spell>();
            if (SpellBar[i].AreObjectsRegistered())
            {
                SpellBar[i].Register(button, action);
                button.GetComponent<UIKeyBinding>().keyCode = keys[i];
                registeredObjects.Add(button);
                break;
            }
        }
    }

    public void Register(SpellButton button, Action<Spell, Spell> action, int arrLoc)
    {
        if (arrLoc >= 0 && arrLoc < 8)
        {
            SpellBar[arrLoc].UnregisterAll();
            SpellBar[arrLoc].Register(button, action);
            button.GetComponent<UIKeyBinding>().keyCode = KeyCode.Alpha1;
            registeredObjects.Add(button);
        }
    }

    public void Set(Spell spell, int loc)
    {
        if (loc >= 0 && loc < 8)
        {
            if (SpellBar[loc] == null) SpellBar[loc] = new Registerable<Spell>();
            SpellBar[loc].Set(spell);
        }
    }

    public bool Registered(SpellButton button)
    {
        return registeredObjects.Contains(button);
    }

    public void ToggleCastButton(bool on)
    {
        if (CastButton != null && CastButton.isEnabled != on) CastButton.isEnabled = on;
    }

    public void ToggleCancelButton(bool on)
    {
        if (CancelButton != null && CancelButton.isEnabled != on) CancelButton.isEnabled = on;
    }

    internal KeyCode[] keys = new KeyCode[] {
        KeyCode.Alpha1,
        KeyCode.Alpha2,
        KeyCode.Alpha3,
        KeyCode.Alpha4,
        KeyCode.Alpha5,
        KeyCode.Alpha6,
        KeyCode.Alpha7,
        KeyCode.Alpha8,
    };
}