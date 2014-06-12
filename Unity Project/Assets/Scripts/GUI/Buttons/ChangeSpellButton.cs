using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChangeSpellButton : GUIButton
{
    public override void Initialize()
    {
        base.Initialize();
        
    }

    void Start()
    {
        OnSingleClick = new Action(() =>
        {
            Spells sp = BigBoss.Player.KnownSpells;
            List<Spell> spells = sp.Values.ToList();
            Spell spell = spells.Random(new System.Random());
            BigBoss.Gooey.spellMenu.Set(spell, 0);
        });
    }
}