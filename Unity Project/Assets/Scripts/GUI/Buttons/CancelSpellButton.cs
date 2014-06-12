using System;
using UnityEngine;

public class CancelSpellButton : GUIButton
{
    public override void Initialize()
    {
        base.Initialize();
    }

    void Start()
    {
        OnSingleClick = new Action(() =>
        {
            BigBoss.Gooey.CancelSpell(true);
        });
    }
}