using System;
using UnityEngine;

public class CastSpellButton : GUIButton
{
    public override void Initialize()
    {
        base.Initialize();
    }

    void Start()
    {
        OnSingleClick = new Action(() =>
        {
            BigBoss.Gooey.CastSpell();
        });
    }
}