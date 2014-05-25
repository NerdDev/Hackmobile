using System;
using UnityEngine;

public class SpellButton : GUIButton
{
    public UISprite sprite;
    internal Spell spell = null;

    void Start()
    {

        sprite = GetComponent<UISprite>();
        OnSingleClick = new Action(() =>
        {
            if (spell != null)
            {
                BigBoss.Gooey.SetToSpellCasting(spell);
            }
        });
        init();
    }

    internal void init()
    {
        if (sprite == null) sprite = this.GetComponent<UISprite>();
        if (spell == null)
        {
            sprite.enabled = false;
            return;
        }
        sprite.enabled = true;
        if (String.IsNullOrEmpty(spell.Icon))
        {
            this.normalSprite = "questionmark";
        }
        else
        {
            this.normalSprite = spell.Icon;
        }
    }

    public override void Initialize()
    {
        base.Initialize();
        SpellMenu menu = parent.Type<SpellMenu>();
        if (menu != null && !menu.Registered(this))
        {
            menu.Register(this, new Action<Spell, Spell>((oldItem, newItem) =>
            {
                spell = newItem;
                init();
            }));
        }
    }

    public void Fix()
    {
        this.label.MakePixelPerfect();
        this.gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        this.gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
    }
}